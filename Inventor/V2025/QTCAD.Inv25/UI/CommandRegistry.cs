using QTCAD.API.Commands;
using QTCAD.API.Properties;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Commands;
using QTCAD.Inv25.Drawing;
using QTCAD.Inv25.Geometry;
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
            ModelDocumentService modelDocumentService = new ModelDocumentService(context);
            PropertyService propertyService = new PropertyService(context);
            FaceAnalyzer faceAnalyzer = new FaceAnalyzer(context.Application.TransientGeometry);
            ModelGeometryService geometryService = new ModelGeometryService(context, faceAnalyzer);
            DrawingService drawingService = new DrawingService(context, modelDocumentService);
            DrawingCommand drawingCommand = new DrawingCommand(drawingService);



            Register(new TestCommand(documentService));
            Register(new DocumentValidationCommand(documentService)); 
            Register(new PropertyCommand(propertyService));
            Register(new GeometryAnalysisCommand(geometryService));
            Register(drawingCommand);  
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