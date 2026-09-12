using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Common.Geometry
{
    internal sealed class ModelGeometryInfo
    {
        public string DocumentName { get; init; } = string.Empty;
        public int BodyCount { get; init; }
        public int FaceCount { get; init; }
        public int EdgeCount { get; init; }
        public double Length { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }
        public double Volume { get; init; }
        public double SurfaceArea { get; init; }
        public IReadOnlyList<FaceInfo> Faces { get; init; } = new List<FaceInfo>();
        public IReadOnlyList<EdgeInfo> Edges { get; init; } = new List<EdgeInfo>();
        public IReadOnlyList<FeatureInfo> Features { get; init; } = new List<FeatureInfo>();
    }
}
