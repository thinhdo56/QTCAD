using Inventor;
using QTCAD.Inv25.Adapter;

namespace QTCAD.Inv25.Services
{
    internal sealed class ModelDocumentService
    {

        private readonly InventorContext _context;

        public ModelDocumentService(InventorContext context)
        {
            _context = context;
            
        }
        public Document? GetActiveModel ()
        {
            Document? document = _context.ActiveDocument;

            if (document is PartDocument || document is AssemblyDocument)
                return document;

            return null;
        }

    }
}
