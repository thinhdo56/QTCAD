using Inventor;
using QTCAD.API.Drawing;
using QTCAD.Inv25.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Drawing
{
    internal sealed class DrawingService
    {
        private readonly InventorContext _context;

        public DrawingService(InventorContext context)
        {
            _context = context;
        }
        public DrawingDocument CreateDrawing(Document model, string templatePath, double scale)
        {
            DrawingDocument drawing = (DrawingDocument)_context.Application.Documents.Add(DocumentTypeEnum.kDrawingDocumentObject, templatePath, true);
            DrawingViewService viewService = new DrawingViewService(drawing);
            TransientGeometry geometry = _context.Application.TransientGeometry;
            Point2d frontPosition = geometry.CreatePoint2d(20, 15);
            DrawingView frontView = viewService.CreateBaseView(model, frontPosition, scale, BaseViewType.Front);
            return drawing;
        }
    }
}
