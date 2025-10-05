using Laboratorio.Cods.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Circut
{
    public class NetManager
    {
        private readonly PinGrid _grid;
        private readonly NetAnalyzer _netAnalyzer = new NetAnalyzer();
        private readonly Dictionary<string, SchematicNet> _nets = new Dictionary<string, SchematicNet>();

        public NetManager(PinGrid grid)
        {
            _grid = grid;
        }

        public void ConnectOrCreateNetForPin(SchematicPin pin, int gridX, int gridY)
        {
            SchematicPin targetPin = _grid.GetPin(gridX, gridY);

            if (targetPin == null)
            {
                pin.ConnectedNet = CreateNewNet($"net_{Guid.NewGuid()}");
            }
            else
            {
                pin.ConnectedNet = targetPin.ConnectedNet;
                targetPin.ConnectedNet.ConnectedPins.Add(pin);
            }
        }

        public void MergePins(SchematicPin pin1, SchematicPin pin2)
        {
            if (pin1.ConnectedNet == pin2.ConnectedNet) return;

            SchematicNet targetNet = pin1.ConnectedNet;
            SchematicNet netToRemove = pin2.ConnectedNet;

            foreach (var pin in netToRemove.ConnectedPins.ToList())
            {
                pin.ConnectedNet = targetNet;
                targetNet.ConnectedPins.Add(pin);
            }

            RemoveNet(netToRemove);
        }

        public void SplitNets(SchematicPin pin1, SchematicPin pin2)
        {
            if (pin1.ConnectedNet != pin2.ConnectedNet) return;

            var separatedGroups = _netAnalyzer.FindConnectedComponentsAfterSplit(
                pin1.ConnectedNet, pin1, pin2);

            if (separatedGroups.Count <= 1) return;

            HandleSplitGroups(separatedGroups, pin1.ConnectedNet);
        }

        private void HandleSplitGroups(List<HashSet<SchematicPin>> groups, SchematicNet originalNet)
        {
            originalNet.ConnectedPins = groups[0].ToList();

            for (int i = 1; i < groups.Count; i++)
            {
                var newNet = CreateNewNet($"net_{Guid.NewGuid()}");
                foreach (var pin in groups[i])
                {
                    pin.ConnectedNet = newNet;
                    newNet.ConnectedPins.Add(pin);
                }
            }
        }

        private SchematicNet CreateNewNet(string name)
        {
            var net = new SchematicNet(name);
            _nets[name] = net;
            return net;
        }

        private void RemoveNet(SchematicNet net)
        {
            _nets.Remove(net.Name);
        }
    }
}
