using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Geometry
{
    public interface IGeometryService
    {
        ModelGeometryInfo? Analyze();
    }
}
