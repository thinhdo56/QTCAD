using QTCAD.API.Commands;
using QTCAD.API.Properties;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Commands;
using QTCAD.Inv25.Services;
using System.Collections.Generic;

namespace QTCAD.Inv25.UI
{
    public sealed class CommandRegistry
    {
        private readonly Dictionary<string, ICommand> _commands = new();

        public CommandRegistry(InventorContext context)
        {
            DocumentService documentService = new DocumentService(context);
            PropertyService propertyService = new PropertyService(context);
            ModelGeometryService geometryService = new ModelGeometryService(context);
            Register(new TestCommand(documentService));
            Register(new DocumentValidationCommand(documentService)); 
            Register(new PropertyCommand(propertyService));
            Register(new GeometryAnalysisCommand(geometryService));
        }

        private void Register(ICommand command)
        {
            _commands.Add(command.Id, command);
        }

        public ICommand Get(string id)
        {
            return _commands[id];
        }
    }
}