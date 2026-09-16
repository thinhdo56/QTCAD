using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Geometry
{
    public sealed class FeatureInfo
    {
        public int Index { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public double Size { get; init; }
        public double Radius { get; init; }
        public double Length { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }
        public double CenterX { get; init; }
        public double CenterY { get; init; }
        public double CenterZ { get; init; }
        public double AxisX { get; init; }
        public double AxisY { get; init; }
        public double AxisZ { get; init; }
    }
}
