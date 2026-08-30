
namespace QTCAD.API.Documents
{
    public interface IDocumentService
    {
        DocumentInfo? GetActiveDocumentInfo();
        ValidationResult ValidateActiveDocument();
    }
}
