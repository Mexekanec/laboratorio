using Laboratorio.Cods.Circut;
using Laboratorio.Cods.Interface;
using System;

using System.ComponentModel;

using System.Runtime.CompilerServices;


namespace Laboratorio.Cods.Components
{
    public class ComponentSettingsService : IComponentSettingsService, INotifyPropertyChanged
    {
        private ComponentType _selectedComponentType = ComponentType.NONE;
        private double _resistanceValue = 1000;
        private double _capacitanceValue = 1e-6;
        private double _voltageValue = 5;
        private double _inductanceValue = 1e-3;
        private PlacementDirection _placementDirection = PlacementDirection.RIGHT;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsChanged;

        public ComponentType SelectedComponentType
        {
            get => _selectedComponentType;
            set
            {
                if (_selectedComponentType != value)
                {
                    _selectedComponentType = value;
                    OnPropertyChanged();
                    OnSettingsChanged();
                }
            }
        }

        public double ResistanceValue
        {
            get => _resistanceValue;
            set
            {
                if (_resistanceValue != value)
                {
                    _resistanceValue = value;
                    OnPropertyChanged();
                    OnSettingsChanged();
                }
            }
        }

        public double CapacitanceValue
        {
            get => _capacitanceValue;
            set
            {
                if (_capacitanceValue != value)
                {
                    _capacitanceValue = value;
                    OnPropertyChanged();
                    OnSettingsChanged();
                }
            }
        }

        public double VoltageValue
        {
            get => _voltageValue;
            set
            {
                if (_voltageValue != value)
                {
                    _voltageValue = value;
                    OnPropertyChanged();
                    OnSettingsChanged();
                }
            }
        }

        public double InductanceValue
        {
            get => _inductanceValue;
            set
            {
                if (_inductanceValue != value)
                {
                    _inductanceValue = value;
                    OnPropertyChanged();
                    OnSettingsChanged();
                }
            }
        }

        public PlacementDirection PlacementDirection
        {
            get => _placementDirection;
            set
            {
                if (_placementDirection != value)
                {
                    _placementDirection = value;
                    OnPropertyChanged();
                    OnSettingsChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
