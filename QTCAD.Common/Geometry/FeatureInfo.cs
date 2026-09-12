using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Common.Geometry
{
    public sealed class FeatureInfo
    {
        public int Index { get; init; }
        public string Type { get; init; } = string.Empty;
        public double Size { get; init; }
        public double Depth { get; init; }
        public int FaceCount { get; init; }
    }
}
