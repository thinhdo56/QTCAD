using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.API.Geometry
{
    public sealed class BoundingBoxInfo
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
    }
}
