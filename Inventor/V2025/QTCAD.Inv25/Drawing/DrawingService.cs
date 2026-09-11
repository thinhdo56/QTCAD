using Inventor;
using QTCAD.API.Drawing;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Services;
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
        private readonly ModelDocumentService _modelDocumentService;

        public DrawingService(InventorContext context, ModelDocumentService modelDocumentService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _modelDocumentService = modelDocumentService ?? throw new ArgumentNullException(nameof(modelDocumentService));
        }
        public DrawingDocument CreateDrawing()
        {
            Document? model = _modelDocumentService.GetActiveModel();

            if (model == null)
                throw new InvalidOperationException("Please open a Part or Assembly document.");

            DrawingDocument drawing = (DrawingDocument)_context.Application.Documents.Add(DocumentTypeEnum.kDrawingDocumentObject, string.Empty, true);

            DrawingViewService viewService = new DrawingViewService(drawing);
            TransientGeometry geometry = _context.Application.TransientGeometry;
            Point2d frontPosition = geometry.CreatePoint2d(20, 15);

            viewService.CreateBaseView(model, frontPosition, 1.0, BaseViewType.Front);

            return drawing;
        }
    }
}
