using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Geometry
{
    public sealed class EdgeInfo
    {
        public int Index { get; init; }
        public string CurveType { get; init; } = string.Empty;
        public double Length { get; init; }
        public bool IsCircular { get; init; }
        public bool IsLinear { get; init; }
        public IReadOnlyList<int> AdjacentFaceIndices { get; set; } = [];
    }
}
