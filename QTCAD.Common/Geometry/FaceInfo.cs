using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Common.Geometry
{
    public sealed class FaceInfo
    {
        public int Index { get; init; }
        public string SurfaceType { get; init; } = string.Empty;
        public double Area { get; init; }
        public double NormalX { get; init; }
        public double NormalY { get; init; }
        public double NormalZ { get; init; }
        public bool IsPlanar { get; init; }
        public bool IsCylindrical { get; init; }
    }
}
