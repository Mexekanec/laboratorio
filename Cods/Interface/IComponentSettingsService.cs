using Laboratorio.Cods.Circut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Interface
{
    public interface IComponentSettingsService
    {
        ComponentType SelectedComponentType { get; set; }
        double ResistanceValue { get; set; }
        double CapacitanceValue { get; set; }
        double VoltageValue { get; set; }
        double InductanceValue { get; set; }
        PlacementDirection PlacementDirection { get; set; }

        event EventHandler SettingsChanged;
    }
}
