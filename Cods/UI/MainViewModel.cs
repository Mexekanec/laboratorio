using Laboratorio.Cods.Circut;
using Laboratorio.Cods.Commands;
using Laboratorio.Cods.Components;
using Laboratorio.Cods.Interface;
using Laboratorio.Cods.UI;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
{
    // Все необходимые сервисы и менеджеры
    private readonly CommandHistoryManager _historyManager;
    private readonly IComponentSettingsService _settingsService;
    private readonly IComponentNamingService _namingService;
    private readonly SchematicManager _schematicManager;

    private Point _lastMousePosition;
    private SchematicComponent _ghostComponent;

    // Команды
    public System.Windows.Input.ICommand PlaceComponentCommand { get; private set; }
    public System.Windows.Input.ICommand SelectComponentCommand { get; private set; }
    public System.Windows.Input.ICommand MouseMoveCommand { get; private set; }
    public System.Windows.Input.ICommand UndoCommand { get; private set; }
    public System.Windows.Input.ICommand RedoCommand { get; private set; }

    // Публичные свойства для привязки
    public IComponentSettingsService SettingsService => _settingsService;
    public SchematicManager SchematicManager => _schematicManager;
    public CommandHistoryManager HistoryManager => _historyManager;

    public SchematicComponent GhostComponent
    {
        get => _ghostComponent;
        set
        {
            _ghostComponent = value;
            OnPropertyChanged();
        }
    }

    public bool CanUndo => _historyManager?.CanUndo ?? false;
    public bool CanRedo => _historyManager?.CanRedo ?? false;

    public string UndoDescription => _historyManager?.CanUndo == true ?
        $"Undo: {_historyManager.UndoStack.Peek().Description}" : "Undo";

    public string RedoDescription => _historyManager?.CanRedo == true ?
        $"Redo: {_historyManager.RedoStack.Peek().Description}" : "Redo";

    public string CurrentComponentDescription =>
        $"{_settingsService.SelectedComponentType}: {GetComponentValue()}";

    // Конструктор
    public MainViewModel()
    {
        // Инициализация сервисов
        _historyManager = new CommandHistoryManager();
        _settingsService = new ComponentSettingsService();
        _namingService = new ComponentNamingService();

        // Создаем SchematicManager с зависимостями
        _schematicManager = new SchematicManager(
            _historyManager,
            _namingService,
            gridWidth: 125,
            gridHeight: 125,
            pixelOffset: 50.0
        );

        // Подписываемся на события
        _settingsService.SettingsChanged += OnSettingsChanged;
        _historyManager.HistoryChanged += OnHistoryChanged;
        _schematicManager.SchemeChanged += OnSchemeChanged;

        InitializeCommands();
    }

    // Альтернативный конструктор с внедрением зависимостей
    public MainViewModel(
        CommandHistoryManager historyManager,
        IComponentSettingsService settingsService,
        IComponentNamingService namingService,
        SchematicManager schematicManager)
    {
        _historyManager = historyManager;
        _settingsService = settingsService;
        _namingService = namingService;
        _schematicManager = schematicManager;

        _settingsService.SettingsChanged += OnSettingsChanged;
        _historyManager.HistoryChanged += OnHistoryChanged;
        _schematicManager.SchemeChanged += OnSchemeChanged;

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        PlaceComponentCommand = new RelayCommand(ExecutePlaceComponent, CanExecutePlaceComponent);
        SelectComponentCommand = new RelayCommand(ExecuteSelectComponent);
        MouseMoveCommand = new RelayCommand(ExecuteMouseMove);
        UndoCommand = new RelayCommand(ExecuteUndo, _ => CanUndo);
        RedoCommand = new RelayCommand(ExecuteRedo, _ => CanRedo);
    }

    private void ExecuteMouseMove(object parameter)
    {
        if (parameter is Point mousePos)
        {
            _lastMousePosition = mousePos;
            UpdateGhostComponent(mousePos);
        }
    }

    private bool CanExecutePlaceComponent(object parameter)
    {
        return _settingsService.SelectedComponentType != ComponentType.NONE;
    }

    private void ExecutePlaceComponent(object parameter)
    {
        if (parameter is Point mousePos)
        {
            var grid = _schematicManager.Grid;
            int gridX = (int)(mousePos.X / grid.PixelOffset);
            int gridY = (int)(mousePos.Y / grid.PixelOffset);

            _schematicManager.PlaceComponent(
                _settingsService.SelectedComponentType,
                gridX,
                gridY,
                _settingsService.PlacementDirection,
                GetComponentValue()
            );
        }
    }

    private void ExecuteSelectComponent(object parameter)
    {
        if (parameter is ComponentType componentType)
        {
            _settingsService.SelectedComponentType = componentType;
        }
        else if (parameter is string typeString)
        {
            if (Enum.TryParse(typeString, out ComponentType type))
            {
                _settingsService.SelectedComponentType = type;
            }
        }
    }

    private void ExecuteUndo(object parameter)
    {
        _historyManager.Undo();
    }

    private void ExecuteRedo(object parameter)
    {
        _historyManager.Redo();
    }

    private double GetComponentValue()
    {
        return _settingsService.SelectedComponentType switch
        {
            ComponentType.RESISTOR => _settingsService.ResistanceValue,
            ComponentType.CAPACITOR => _settingsService.CapacitanceValue,
            ComponentType.INDUCTOR => _settingsService.InductanceValue,
            ComponentType.VOLTAGE_SOURCE => _settingsService.VoltageValue,
            _ => 0
        };
    }

    private void UpdateGhostComponent(Point mousePos)
    {
        if (_settingsService.SelectedComponentType == ComponentType.NONE)
        {
            GhostComponent = null;
            return;
        }

        var grid = _schematicManager.Grid;
        int gridX = (int)(mousePos.X / grid.PixelOffset);
        int gridY = (int)(mousePos.Y / grid.PixelOffset);

        // Проверяем валидность размещения
        bool isValid = _schematicManager.CanPlaceComponent(
            gridX, gridY, _settingsService.PlacementDirection);

        if (GhostComponent == null)
        {
            GhostComponent = CreateGhostComponent();
        }

        GhostComponent.UpdatePinsPosition(gridX, gridY, _settingsService.PlacementDirection);
        GhostComponent.IsValidPlacement = isValid;
        GhostComponent.UpdateBoundingBox();

        OnPropertyChanged(nameof(GhostComponent));
    }

    private SchematicComponent CreateGhostComponent()
    {
        var component = new SchematicComponent
        {
            Type = _settingsService.SelectedComponentType,
            IsGhost = true,
            Opacity = 0.6,
            DisplayName = "Ghost"
        };

        component.Pins.Add(new SchematicPin(0, 0));
        component.Pins.Add(new SchematicPin(0, 0));

        return component;
    }

    // Обработчики событий
    private void OnSettingsChanged(object sender, EventArgs e)
    {
        OnPropertyChanged(nameof(CurrentComponentDescription));
        GhostComponent = null; // Сбрасываем призрака при смене типа компонента
    }

    private void OnHistoryChanged(object sender, EventArgs e)
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnPropertyChanged(nameof(UndoDescription));
        OnPropertyChanged(nameof(RedoDescription));
    }

    private void OnSchemeChanged(object sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SchematicManager));
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}