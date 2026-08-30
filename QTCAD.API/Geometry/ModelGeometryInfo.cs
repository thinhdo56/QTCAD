using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Geometry
{
    public sealed class ModelGeometryInfo
    {
        public double XSize { get; init; }

        public double YSize { get; init; }

        public double ZSize { get; init; }

        public double Volume { get; init; }

        public double SurfaceArea { get; init; }

        public int BodyCount { get; init; }

        public int FaceCount { get; init; }

        public int EdgeCount { get; init; }

        public int VertexCount { get; init; }

        public string Unit { get; init; } = string.Empty;
        public int OccurrenceCount { get; init; }
        public IReadOnlyList<FaceGeometryInfo> Faces { get; init; } = [];
    }

}
