using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Geometry
{
    public sealed class FaceGeometryInfo
    {
        public int Index { get; init; }
        public double Area { get; init; }
        public double CenterX { get; init; }
        public double CenterY { get; init; }
        public double CenterZ { get; init; }
        public double NormalX { get; init; }
        public double NormalY { get; init; }
        public double NormalZ { get; init; }
        public string Type { get; init; } = string.Empty;
    }
}
