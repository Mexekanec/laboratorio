using Laboratorio.Cods.Circut;
using Laboratorio.Cods.Components;
using Laboratorio.Cods.Interface;
using SpiceSharp.Components;
using System;
using System.Collections.Generic;


namespace Laboratorio.Cods.UI
{
    public class SchematicManager
    {
        private readonly CommandHistoryManager _historyManager;
        private readonly IComponentNamingService _namingService;
        private readonly PinGrid _grid;
        private readonly NetManager _netManager;

        private readonly List<SchematicComponent> _components = new List<SchematicComponent>();
        private readonly List<SchematicComponent> _deletedComponents = new List<SchematicComponent>();

        public event EventHandler SchemeChanged;

        public IReadOnlyList<SchematicComponent> Components => _components;
        public IReadOnlyList<SchematicComponent> DeletedComponents => _deletedComponents;
        public PinGrid Grid => _grid;

        public SchematicManager(CommandHistoryManager historyManager,
            IComponentNamingService namingService, int gridWidth, int gridHeight, double pixelOffset)
        {
            _historyManager = historyManager;
            _namingService = namingService;
            _grid = new PinGrid(gridWidth, gridHeight, pixelOffset);
            _netManager = new NetManager(_grid);
        }

        public SchematicComponent PlaceComponent(ComponentType type, int gridX, int gridY,
            PlacementDirection direction, double value)
        {
            if (!_grid.AreCoordinatesValid(gridX, gridY)) return null;

            var (endX, endY) = CalculateEndPoint(gridX, gridY, direction);
            if (!_grid.AreCoordinatesValid(endX, endY)) return null;

            if (!_grid.CanPlaceComponentAt(gridX, gridY) || !_grid.CanPlaceComponentAt(endX, endY))
                return null;

            var newComponent = CreateNewComponent(type, gridX, gridY, direction, value);

            _netManager.ConnectOrCreateNetForPin(newComponent.Pins[0], gridX, gridY);
            _netManager.ConnectOrCreateNetForPin(newComponent.Pins[1], endX, endY);

            _grid.PlacePin(gridX, gridY, newComponent.Pins[0]);
            _grid.PlacePin(endX, endY, newComponent.Pins[1]);

            _components.Add(newComponent);

            SchemeChanged?.Invoke(this, EventArgs.Empty);
            return newComponent;
        }

        public void DeleteComponent(SchematicComponent component)
        {
            _components.Remove(component);
            _deletedComponents.Add(component);
            _namingService.ComponentDeleted(component.Type, component.DisplayName);
            SchemeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RestoreComponent(SchematicComponent component)
        {
            _deletedComponents.Remove(component);
            _components.Add(component);
            SchemeChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanPlaceComponent(int x, int y, PlacementDirection direction)
        {
            if (!_grid.AreCoordinatesValid(x, y)) return false;

            var (endX, endY) = CalculateEndPoint(x, y, direction);
            if (!_grid.AreCoordinatesValid(endX, endY)) return false;

            return _grid.CanPlaceComponentAt(x, y) && _grid.CanPlaceComponentAt(endX, endY);
        }

        private (int, int) CalculateEndPoint(int startX, int startY, PlacementDirection dir)
        {
            return dir switch
            {
                PlacementDirection.RIGHT => (startX + 1, startY),
                PlacementDirection.LEFT => (startX - 1, startY),
                PlacementDirection.UP => (startX, startY - 1),
                PlacementDirection.DOWN => (startX, startY + 1),
                _ => (startX, startY)
            };
        }

        private SchematicComponent CreateNewComponent(ComponentType type, int x, int y,
            PlacementDirection dir, double value)
        {
            var component = new SchematicComponent
            {
                Type = type,
                GridX = x,
                GridY = y,
                Direction = dir,
                Value = value,
                DisplayName = _namingService.GetNextName(type)
            };

            component.Pins.Add(new SchematicPin(x, y));
            component.Pins.Add(new SchematicPin(0, 0));
            component.UpdatePinsPosition(x, y, dir);

            return component;
        }
    }
}
