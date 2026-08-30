using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Documents
{
    public sealed class DocumentInfo
    {
        public string DisplayName { get; init; } = string.Empty;

        public string FullFileName { get; init; } = string.Empty;

        public DocumentKind DocumentType { get; init; }
        public string FileName { get; init; } = string.Empty;

        public bool IsSaved { get; init; }
    }
}
