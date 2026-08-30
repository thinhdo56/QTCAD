using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Documents
{

        public sealed class ValidationResult
        {
            public bool IsValid { get; init; }

            public string Message { get; init; } = string.Empty;
        }
}
