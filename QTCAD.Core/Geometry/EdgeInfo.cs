using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Core.Geometry
{
        public sealed class EdgeInfo
        {
            public int Index { get; init; }
            public string CurveType { get; init; } = string.Empty;
            public double Length { get; init; }
            public bool IsCircular { get; init; }
            public bool IsLinear { get; init; }
            public double CenterX { get; init; }
            public double CenterY { get; init; }
            public double CenterZ { get; init; }
            public IReadOnlyList<int> AdjacentFaceIndices { get; set; } = [];
        }
}
