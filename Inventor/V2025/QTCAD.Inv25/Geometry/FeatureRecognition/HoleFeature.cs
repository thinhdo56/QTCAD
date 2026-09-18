using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Geometry.FeatureRecognition
{
    internal sealed class HoleFeature
    {
        public int CylindricalFaceIndex { get; init; }
        public double Radius { get; init; }
        public double Diameter => Radius * 2.0;
        public IReadOnlyList<int> BoundaryEdgeIndices { get; init; } = [];
        public IReadOnlyList<int> AdjacentFaceIndices { get; init; } = [];
    }
}
