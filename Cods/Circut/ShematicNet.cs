using Laboratorio.Cods.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Circut
{
    public class SchematicNet
    {
        // Уникальное имя узла для SpiceSharp (например "net_1", "vcc", "gnd")
        public string Name { get; set; }

        public List<SchematicPin> ConnectedPins { get; set; } = new List<SchematicPin>();


        public System.Windows.Media.Color Color { get; set; }

        public SchematicNet(string name)
        {
            Name = name;
        }
    }
}
