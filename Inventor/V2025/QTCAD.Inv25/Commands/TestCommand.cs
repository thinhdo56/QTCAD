using Inventor;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QTCAD.API.Commands;

namespace QTCAD.Inv25.Commands
{
    internal sealed class TestCommand : ICommand
    {
        private readonly DocumentService _documentService;
        public string Id => "QTCAD.Test";
        public string DisplayName => "QTCAD Test";
        public string Description => "Test QTCAD Inventor connection";
        public TestCommand(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [SupportedOSPlatform("windows6.1")]
        public void Execute()
        {
            var document = _documentService.GetActiveDocumentInfo();

            if (document == null)
            {
                MessageBox.Show("Không có document nào đang mở.", "QTCAD Test");
                return;
            }

            MessageBox.Show(
            $"File Name: {document.FileName}\n" +
            $"Display Name: {document.DisplayName}\n" +
            $"Full Path: {document.FullFileName}\n" +
            $"Document Type: {document.DocumentType}\n" +
            $"Saved: {document.IsSaved}",
            "QTCAD Document Information");
        }
    }
}
