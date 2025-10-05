using Laboratorio.Cods.Circut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.UI
{
    public class SchematicPin
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public int GridX { get; set; }
        public int GridY { get; set; }
        public SchematicComponent ParentComponent { get; set; }
        public SchematicNet ConnectedNet { get; set; }
        public System.Windows.Point Position { get; set; }
        public System.Windows.Rect HitBox { get; set; }

        public SchematicPin(int gridX, int gridY)
        {
            GridX = gridX;
            GridY = gridY;
        }

        public bool IsStandalone => ParentComponent == null;
    }
}
