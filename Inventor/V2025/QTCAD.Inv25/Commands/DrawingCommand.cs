using System;
using System.Windows.Forms;
using QTCAD.Inv25.Drawing;
using ICommand = QTCAD.API.Commands.ICommand;

namespace QTCAD.Inv25.Commands
{
    internal sealed class DrawingCommand : ICommand
    {
        private readonly DrawingService _drawingService;

        public string Id => "QTCAD.Drawing.Create2D";
        public string DisplayName => "Create 2D";
        public string Description => "Creates a drawing from the active model.";

        public DrawingCommand(DrawingService drawingService)
        {
            _drawingService = drawingService;
        }

        public void Execute()
        {
            try
            {
                _drawingService.CreateDrawing();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create drawing.\n\n{ex.Message}", "QTCAD");
            }
        }
    }
}
