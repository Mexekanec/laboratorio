using Laboratorio.Cods.Circut;
using Laboratorio.Cods.Interface;
using Laboratorio.Cods.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Components
{
    public class DeleteComponentCommand : ICommand
    {
        private readonly SchematicManager _schematicManager;
        private readonly SchematicComponent _component;

        public string Description => $"Delete {_component.DisplayName}";

        public DeleteComponentCommand(SchematicManager schematicManager, SchematicComponent component)
        {
            _schematicManager = schematicManager;
            _component = component;
        }

        public void Execute()
        {
            _schematicManager.DeleteComponent(_component);
        }

        public void Undo()
        {
            _schematicManager.RestoreComponent(_component);
        }
    }
}
