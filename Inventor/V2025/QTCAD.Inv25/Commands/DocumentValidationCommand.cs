using QTCAD.Inv25.Services;
using System.Windows.Forms;
using QTCAD.API.Commands;

namespace QTCAD.Inv25.Commands
{
    internal sealed class DocumentValidationCommand : ICommand
    {
        private readonly DocumentService _documentService;
        public string Id => "QTCAD.DocumentValidation";
        public string DisplayName => "Document Validation";
        public string Description => "Validate active document";
        public DocumentValidationCommand(DocumentService documentService)
        {
            _documentService = documentService;
        }

        public void Execute()
        {
            var result = _documentService.ValidateActiveDocument();

            MessageBox.Show(
                result.Message,
                result.IsValid ? "QTCAD - Valid" : "QTCAD - Invalid");
        }
    }
}
