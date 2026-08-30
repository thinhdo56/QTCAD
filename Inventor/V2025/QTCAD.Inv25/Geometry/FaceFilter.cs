using Inventor;
using System.Collections.Generic;

namespace QTCAD.Inv25.Geometry
{
    internal sealed class FaceFilter : IFaceFilter
    {
        public IEnumerable<Face> Filter(IEnumerable<Face> faces)
        {
            return faces;
        }
    }
}