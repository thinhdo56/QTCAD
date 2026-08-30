using Inventor;
using System.Collections.Generic;

namespace QTCAD.Inv25.Geometry
{
    internal interface IFaceFilter
    {
        IEnumerable<Face> Filter(IEnumerable<Face> faces);
    }
}