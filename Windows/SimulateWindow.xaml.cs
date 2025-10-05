using Laboratorio.Cods.Circut;
using Laboratorio.Cods.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Laboratorio.Windows
{
    public partial class SimulateWindow : Window
    {
        private double _currentZoom = 1.0;
        private readonly double _zoomFactor = 1.1;
        private Point _lastMousePoint;
        private bool _isPanning = false;

        private readonly MainViewModel _viewModel;


        public SimulateWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel = new MainViewModel();
            // Подписываемся на события
            _viewModel.SchematicManager.SchemeChanged += OnSchemeChanged;

            // Первоначальная отрисовка
            InitializePins();
            RedrawScheme();
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true;

                // Получаем позицию мыши относительно ScrollViewer
                var mousePoint = e.GetPosition(TableScrollViewer);

                // Вычисляем масштаб
                if (e.Delta > 0)
                    _currentZoom *= _zoomFactor;
                else
                    _currentZoom /= _zoomFactor;

                // Ограничиваем масштаб (например, от 0.25 до 4.0)
                _currentZoom = Math.Clamp(_currentZoom, 0.5, 3.0);

                // Применяем масштаб к Transform
                ZoomTransform.ScaleX = _currentZoom;
                ZoomTransform.ScaleY = _currentZoom;

                // Центрируем масштаб относительно курсора
                // Для этого корректируем прокрутку ScrollViewer
                var viewportSize = new Point(TableScrollViewer.ViewportWidth, TableScrollViewer.ViewportHeight);
                var newOffsetX = mousePoint.X * _currentZoom - viewportSize.X * 0.5;
                var newOffsetY = mousePoint.Y * _currentZoom - viewportSize.Y * 0.5;

                TableScrollViewer.ScrollToHorizontalOffset(newOffsetX);
                TableScrollViewer.ScrollToVerticalOffset(newOffsetY);
            }
            else
            {

            }
        }

        private void ResetZoom()
        {
            _currentZoom = 1.0;
            ZoomTransform.ScaleX = 1.0;
            ZoomTransform.ScaleY = 1.0;
            TableScrollViewer.ScrollToHorizontalOffset(0);
            TableScrollViewer.ScrollToVerticalOffset(0);
        }


        private void DesignCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isPanning = true;
                _lastMousePoint = e.GetPosition(TableScrollViewer);
                Mouse.Capture(Table);
                e.Handled = true;
            }
        }

        private void DesignCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                var currentPoint = e.GetPosition(TableScrollViewer);
                var delta = currentPoint - _lastMousePoint;

                TableScrollViewer.ScrollToHorizontalOffset(TableScrollViewer.HorizontalOffset - delta.X);
                TableScrollViewer.ScrollToVerticalOffset(TableScrollViewer.VerticalOffset - delta.Y);

                _lastMousePoint = currentPoint;
            }
        }

        private void DesignCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _isPanning = false;
                Mouse.Capture(null);
            }
        }

        private void InitializePins()
        {
            Table.Children.Clear();

            var grid = _viewModel.SchematicManager.Grid;

            // Рисуем пины сетки
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    DrawPin(x, y);
                }
            }
        }

        private void DrawPin(int gridX, int gridY)
        {
            var pixelOffset = _viewModel.SchematicManager.Grid.PixelOffset;

            // Создаем визуальное представление пина
            var ellipse = new Ellipse
            {
                Width = 16,
                Height = 16,
                Fill = Brushes.Gray,
                Stroke = Brushes.DarkGray,
                StrokeThickness = 1,
                Opacity = 0.3
            };

            // Позиционируем на Canvas
            Canvas.SetLeft(ellipse, gridX * pixelOffset - 8); // -3 для центрирования
            Canvas.SetTop(ellipse, gridY * pixelOffset - 8);

            // Сохраняем координаты в Tag для идентификации
            ellipse.Tag = new Point(gridX, gridY);

            // Добавляем обработчики событий
            ellipse.MouseEnter += Pin_MouseEnter;
            ellipse.MouseLeave += Pin_MouseLeave;
            ellipse.MouseDown += Pin_MouseDown;

            Table.Children.Add(ellipse);
        }

        // Перерисовка всей схемы (компоненты + провода)
        private void RedrawScheme()
        {
            // Удаляем старые компоненты (но оставляем пины)
            var componentsToRemove = Table.Children
                .OfType<FrameworkElement>()
                .Where(x => x.Tag?.ToString()?.StartsWith("component_") == true ||
                           x.Tag?.ToString()?.StartsWith("wire_") == true)
                .ToList();

            foreach (var element in componentsToRemove)
            {
                Table.Children.Remove(element);
            }

            // Рисуем компоненты
            foreach (var component in _viewModel.SchematicManager.Components)
            {
                DrawComponent(component);
            }

            // Рисуем провода
            DrawWires();
        }

        // Отрисовка компонента
        private void DrawComponent(SchematicComponent component)
        {
            if (component.Pins.Count < 2) return;

            var startPin = component.Pins[0];
            var endPin = component.Pins[1];

            // Линия компонента (резистор, конденсатор и т.д.)
            var line = new Line
            {
                X1 = startPin.Position.X,
                Y1 = startPin.Position.Y,
                X2 = endPin.Position.X,
                Y2 = endPin.Position.Y,
                Stroke = component.IsGhost ? Brushes.Blue : Brushes.Black,
                StrokeThickness = component.IsGhost ? 1 : 2,
                StrokeDashArray = component.IsGhost ? new DoubleCollection { 2, 2 } : null,
                Opacity = component.IsGhost ? 0.5 : 1.0,
                Tag = $"component_{component.Id}"
            };

            // Надпись с именем компонента
            var textBlock = new TextBlock
            {
                Text = component.DisplayName,
                FontSize = 10,
                Foreground = Brushes.DarkBlue,
                Background = Brushes.White,
                Padding = new Thickness(2),
                Tag = $"component_label_{component.Id}"
            };

            // Позиционируем надпись посередине компонента
            Canvas.SetLeft(textBlock, (startPin.Position.X + endPin.Position.X) / 2 - 10);
            Canvas.SetTop(textBlock, (startPin.Position.Y + endPin.Position.Y) / 2 - 8);

            Table.Children.Add(line);
            Table.Children.Add(textBlock);
        }

        // Отрисовка проводов
        private void DrawWires()
        {
            // Группируем пины по узлам и рисуем провода между ними
            var pinsByNet = _viewModel.SchematicManager.Grid.GetAllPins()
                .Where(p => p.ConnectedNet != null)
                .GroupBy(p => p.ConnectedNet)
                .Where(g => g.Count() > 1); // Только узлы с несколькими пинами

            foreach (var netGroup in pinsByNet)
            {
                var pins = netGroup.ToList();

                // Рисуем провода между всеми пинами в узле
                for (int i = 0; i < pins.Count; i++)
                {
                    for (int j = i + 1; j < pins.Count; j++)
                    {
                        DrawWire(pins[i], pins[j]);
                    }
                }
            }
        }

        private void DrawWire(SchematicPin pin1, SchematicPin pin2)
        {
            var wire = new Line
            {
                X1 = pin1.Position.X,
                Y1 = pin1.Position.Y,
                X2 = pin2.Position.X,
                Y2 = pin2.Position.Y,
                Stroke = Brushes.Green,
                StrokeThickness = 1,
                Opacity = 0.7,
                Tag = $"wire_{pin1.Id}_{pin2.Id}"
            };

            Table.Children.Add(wire);
        }

        // Обработчики событий пинов
        private void Pin_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                ellipse.Fill = Brushes.Aqua; // Подсветка при наведении
            }
        }

        private void Pin_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                ellipse.Fill = Brushes.Gray; // Возвращаем обычный цвет
            }
        }

        private void Pin_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse && ellipse.Tag is Point gridPos)
            {
                // Передаем координаты в ViewModel
                var mousePos = new System.Windows.Point(
                    gridPos.X * _viewModel.SchematicManager.Grid.PixelOffset,
                    gridPos.Y * _viewModel.SchematicManager.Grid.PixelOffset);

                _viewModel.MouseMoveCommand?.Execute(mousePos);

                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    _viewModel.PlaceComponentCommand?.Execute(mousePos);
                }
            }
        }

        // Обработчик изменений схемы
        private void OnSchemeChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(RedrawScheme);
        }

        protected override void OnClosed(EventArgs e)
        {
            // Отписываемся от событий
            _viewModel.SchematicManager.SchemeChanged -= OnSchemeChanged;
            base.OnClosed(e);
        }
    }
}
