
namespace QTCAD.Core.Features
{
    public abstract class QTCADFeature
    {
        // Identity
        public int Id { get; init; }

        // Feature classification
        public string Type { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        // Topology
        public IReadOnlyList<int> FaceIndices { get; init; } = [];

        public IReadOnlyList<int> EdgeIndices { get; init; } = [];

        public IReadOnlyList<int> VertexIndices { get; init; } = [];

        // Geometric location
        public double CenterX { get; init; }

        public double CenterY { get; init; }

        public double CenterZ { get; init; }

        // Direction / orientation
        public double AxisX { get; init; }

        public double AxisY { get; init; }

        public double AxisZ { get; init; }

        // Bounding dimensions
        public double XSize { get; init; }

        public double YSize { get; init; }

        public double ZSize { get; init; }

        // Recognition information
        public double Confidence { get; init; }

        public string Source { get; init; } = string.Empty;
    }
}
