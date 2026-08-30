using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Properties
{
    public interface IPropertyService
    {
        IReadOnlyList<PropertyInfo> GetDocumentProperties();
    }
}
