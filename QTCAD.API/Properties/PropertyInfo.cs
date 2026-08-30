using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Properties
{
    public sealed class PropertyInfo
    {
        public string Name { get; init; } = string.Empty;

        public string Value { get; init; } = string.Empty;

        public string Category { get; init; } = string.Empty;
    }
}
