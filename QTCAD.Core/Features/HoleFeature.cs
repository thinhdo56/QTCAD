
namespace QTCAD.Core.Features
{
    public sealed class HoleFeature: QTCADFeature
    {
        public int CylindricalFaceIndex { get; init; }
        public double Radius { get; init; }
        public double Diameter => Radius * 2.0;
        public IReadOnlyList<int> BoundaryEdgeIndices { get; init; } = [];
        public IReadOnlyList<int> AdjacentFaceIndices { get; init; } = [];
        public bool IsBlind { get; set; }
        public double Depth { get; set; }
        public HoleBottomType BottomType { get; init; }
        public object ConeHalfAngle { get; set; }
        public object ConeIsExpanding { get; set; }

        public enum HoleBottomType
        {
            Through, Flat, Conical, Unknown
        }
    }
}
    