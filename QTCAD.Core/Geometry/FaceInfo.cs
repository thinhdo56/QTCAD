using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Core.Geometry
{
    public sealed class FaceInfo
    {
        public int Index { get; init; }
        public IReadOnlyList<int> EdgeIndices { get; init; } = [];
        public double Area { get; init; }
        public double CenterX { get; init; }
        public double CenterY { get; init; }
        public double CenterZ { get; init; }
        public double NormalX { get; init; }
        public double NormalY { get; init; }
        public double NormalZ { get; init; }
        public string Type { get; init; } = string.Empty;
        public bool IsPlanar { get; init; }
        public double Radius { get; init; }
        public double AxisX { get; init; }
        public double AxisY { get; init; }
        public double AxisZ { get; init; }
        public bool IsInterior { get; set; }
        public double ConeHalfAngle { get; init; }
        public bool ConeIsExpanding { get; init; }
    }
}
