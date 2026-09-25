using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Core.Geometry
{
    public sealed class GeometryExtractionResult
    {
        public IReadOnlyList<FaceInfo> Faces { get; init; } = [];
        public IReadOnlyList<EdgeInfo> Edges { get; init; } = [];
    }
}
