using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.UI
{
    public class PinGrid
    {
        public int Width { get; }
        public int Height { get; }
        public double PixelOffset { get; }

        private SchematicPin[,] _grid;

        public PinGrid(int width, int height, double pixelOffset)
        {
            Width = width;
            Height = height;
            PixelOffset = pixelOffset;
            _grid = new SchematicPin[width, height];
        }

        public SchematicPin GetPin(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;
            return _grid[x, y];
        }

        public void PlacePin(int x, int y, SchematicPin pin)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException();

            _grid[x, y] = pin;
            pin.GridX = x;
            pin.GridY = y;
            pin.Position = new System.Windows.Point(x * PixelOffset, y * PixelOffset);
            pin.HitBox = new System.Windows.Rect(pin.Position.X - 5, pin.Position.Y - 5, 10, 10);
        }

        public void ClearPin(int x, int y)
        {
            _grid[x, y] = null;
        }

        public bool AreCoordinatesValid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool CanPlaceComponentAt(int x, int y)
        {
            var pin = GetPin(x, y);
            return pin == null || pin.IsStandalone;
        }

        public List<SchematicPin> GetAllPins()
        {
            var pins = new List<SchematicPin>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_grid[x, y] != null)
                        pins.Add(_grid[x, y]);
                }
            }
            return pins;
        }
    }
}
