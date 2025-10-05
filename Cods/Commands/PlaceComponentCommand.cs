using Laboratorio.Cods.Circut;
using Laboratorio.Cods.Interface;
using Laboratorio.Cods.UI;


namespace Laboratorio.Cods.Commands
{
    public class PlaceComponentCommand : ICommand
    {
        private readonly SchematicManager _schematicManager;
        private readonly ComponentType _type;
        private readonly int _x;
        private readonly int _y;
        private readonly PlacementDirection _direction;
        private readonly double _value;

        private SchematicComponent _createdComponent;

        public string Description => $"Place {_type} at ({_x},{_y})";

        public PlaceComponentCommand(SchematicManager schematicManager,
            ComponentType type, int x, int y, PlacementDirection direction, double value)
        {
            _schematicManager = schematicManager;
            _type = type;
            _x = x;
            _y = y;
            _direction = direction;
            _value = value;
        }

        public void Execute()
        {
            _createdComponent = _schematicManager.PlaceComponent(_type, _x, _y, _direction, _value);
        }

        public void Undo()
        {
            if (_createdComponent != null)
            {
                _schematicManager.DeleteComponent(_createdComponent);
            }
        }
    }
}
