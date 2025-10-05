using Laboratorio.Cods.UI;
using SpiceSharp.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laboratorio.Cods.Circut
{
        public class SchematicComponent
        {
            public string Id { get; } = Guid.NewGuid().ToString();
            public List<SchematicPin> Pins { get; set; } = new List<SchematicPin>();
            public Component SpiceComponent { get; set; }
            public int GridX { get; set; }
            public int GridY { get; set; }
            public PlacementDirection Direction { get; set; }
            public ComponentType Type { get; set; }
            public double Value { get; set; }
            public string DisplayName { get; set; }

            // Визуальные свойства
            public string ImageSource { get; set; }
            public string GhostImageSource { get; set; }
            public string RedImageSource { get; set; }
            public bool IsGhost { get; set; }
            public bool IsValidPlacement { get; set; } = true;
            public double Opacity { get; set; } = 1.0;
            public System.Windows.Rect BoundingBox { get; set; }
            public System.Windows.Point LabelPosition { get; set; }

            public void UpdatePinsPosition(int gridX, int gridY, PlacementDirection direction)
            {
                GridX = gridX;
                GridY = gridY;
                Direction = direction;

                Pins[0].GridX = gridX;
                Pins[0].GridY = gridY;

                switch (direction)
                {
                    case PlacementDirection.RIGHT:
                        Pins[1].GridX = gridX + 1;
                        Pins[1].GridY = gridY;
                        break;
                    case PlacementDirection.LEFT:
                        Pins[1].GridX = gridX - 1;
                        Pins[1].GridY = gridY;
                        break;
                    case PlacementDirection.UP:
                        Pins[1].GridX = gridX;
                        Pins[1].GridY = gridY - 1;
                        break;
                    case PlacementDirection.DOWN:
                        Pins[1].GridX = gridX;
                        Pins[1].GridY = gridY + 1;
                        break;
                }
            }

            public bool ContainsPoint(System.Windows.Point point)
            {
                return BoundingBox.Contains(point);
            }

            public void UpdateBoundingBox()
            {
                if (Pins.Count < 2) return;

                double minX = Pins.Min(p => p.Position.X);
                double minY = Pins.Min(p => p.Position.Y);
                double maxX = Pins.Max(p => p.Position.X);
                double maxY = Pins.Max(p => p.Position.Y);

                BoundingBox = new System.Windows.Rect(
                    minX - 10, minY - 10,
                    (maxX - minX) + 20, (maxY - minY) + 20);
            }
        }

        // Вспомогательные перечисления
    public enum PlacementDirection { RIGHT, UP, LEFT, DOWN }
    public enum ComponentType { RESISTOR,CAPACITOR,VOLTAGE_SOURCE,INDUCTOR,AC_GENERATOR,DIODE,BJT,MOSFET,TRIANGLE_GENERATOR, STAR_GENERATOR, NONE }
}
