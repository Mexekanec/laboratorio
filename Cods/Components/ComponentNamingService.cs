using Laboratorio.Cods.Circut;
using Laboratorio.Cods.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Components
{
    public class ComponentNamingService : IComponentNamingService
    {
        private readonly Dictionary<ComponentType, int> _counters = new();
        private readonly Dictionary<ComponentType, List<string>> _deletedNames = new();

        public ComponentNamingService()
        {
            foreach (ComponentType type in Enum.GetValues(typeof(ComponentType)))
            {
                _counters[type] = 1;
                _deletedNames[type] = new List<string>();
            }
        }

        public string GetNextName(ComponentType type)
        {
            string prefix = GetPrefix(type);

            if (_deletedNames[type].Count > 0)
            {
                var name = _deletedNames[type].First();
                _deletedNames[type].RemoveAt(0);
                return name;
            }

            return $"{prefix}{_counters[type]++}";
        }

        public void ComponentDeleted(ComponentType type, string name)
        {
            _deletedNames[type].Add(name);
            _deletedNames[type].Sort((a, b) =>
                int.Parse(a[1..]).CompareTo(int.Parse(b[1..])));
        }

        public void ResetCounter(ComponentType type)
        {
            _counters[type] = 1;
            _deletedNames[type].Clear();
        }

        private string GetPrefix(ComponentType type) => type switch
        {
            ComponentType.RESISTOR => "R",
            ComponentType.CAPACITOR => "C",
            ComponentType.INDUCTOR => "L",
            ComponentType.DIODE => "D",
            ComponentType.VOLTAGE_SOURCE => "V",
            _ => "X"
        };
    }
}
