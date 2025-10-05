using Laboratorio.Cods.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Circut
{
    public class NetAnalyzer
    {
        public List<HashSet<SchematicPin>> FindConnectedComponentsAfterSplit(
            SchematicNet net, SchematicPin excludedPin1, SchematicPin excludedPin2)
        {
            var visited = new HashSet<SchematicPin>();
            var components = new List<HashSet<SchematicPin>>();

            foreach (var startPin in net.ConnectedPins)
            {
                if (visited.Contains(startPin) || startPin == excludedPin1 || startPin == excludedPin2)
                    continue;

                var component = new HashSet<SchematicPin>();
                TraverseGraph(startPin, visited, component, excludedPin1, excludedPin2);
                components.Add(component);
            }

            return components;
        }

        private void TraverseGraph(
            SchematicPin currentPin,
            HashSet<SchematicPin> visited,
            HashSet<SchematicPin> component,
            SchematicPin excludedPin1,
            SchematicPin excludedPin2)
        {
            visited.Add(currentPin);
            component.Add(currentPin);

            foreach (var neighbor in currentPin.ConnectedNet.ConnectedPins)
            {
                if (visited.Contains(neighbor) || neighbor == excludedPin1 || neighbor == excludedPin2)
                    continue;

                TraverseGraph(neighbor, visited, component, excludedPin1, excludedPin2);
            }
        }
    }
}

