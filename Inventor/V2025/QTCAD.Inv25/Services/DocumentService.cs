using Inventor;
using QTCAD.API.Documents;
using QTCAD.Inv25.Adapter;
using System.IO;

namespace QTCAD.Inv25.Services
{
    public sealed class DocumentService: IDocumentService
    {
        private readonly InventorContext _context;

        public DocumentService(InventorContext context)
        {
            _context = context;
        }
        private static DocumentKind GetDocumentKind(Document document)
        {
            return document.DocumentType switch
            {
                DocumentTypeEnum.kPartDocumentObject => DocumentKind.Part,
                DocumentTypeEnum.kAssemblyDocumentObject => DocumentKind.Assembly,
                DocumentTypeEnum.kDrawingDocumentObject => DocumentKind.Drawing,
                DocumentTypeEnum.kPresentationDocumentObject => DocumentKind.Presentation,
                _ => DocumentKind.Unknown
            };
        }
        public ValidationResult ValidateActiveDocument()
        {
            Document? document = _context.ActiveDocument;

            if (document == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Không có document nào đang mở."
                };
            }

            if (string.IsNullOrWhiteSpace(document.FullFileName))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Document chưa có đường dẫn file hợp lệ."
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                Message = "Document hợp lệ."
            };
        }
        public DocumentInfo? GetActiveDocumentInfo()
        {
            Document? document = _context.ActiveDocument;

            if (document == null)
            {
                return null;
            }

            return new DocumentInfo
            {
                DisplayName = document.DisplayName,
                FileName = System.IO.Path.GetFileName(document.FullFileName),
                FullFileName = document.FullFileName,
                DocumentType = GetDocumentKind(document),
                IsSaved = !string.IsNullOrEmpty(document.FullFileName)
            };
        }
    }
}