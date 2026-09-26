
namespace QTCAD.Core.Features
{
    public sealed class HoleFeature : QTCADFeature
    {
        public int CylindricalFaceIndex { get; init; }
        public double Radius { get; init; }
        public double Diameter => Radius * 2.0;

        public IReadOnlyList<int> BoundaryEdgeIndices { get; init; } = [];
        public IReadOnlyList<int> AdjacentFaceIndices { get; init; } = [];

        public bool IsBlind { get; set; }
        public double Depth { get; set; }
        public double SecondaryDepth { get; init; }
        public double TotalDepth => Depth + SecondaryDepth;

        public HoleBottomType BottomType { get; init; }
        public HoleType TypeOfHole { get; init; }

        public IReadOnlyList<int> ProfileFaceIndices { get; init; } = [];
        public IReadOnlyList<int> ProfileEdgeIndices { get; init; } = [];

        public double SecondaryRadius { get; init; }
        public double SecondaryDiameter => SecondaryRadius * 2.0;

        public double CountersinkAngle { get; init; }
        public int SecondaryCylindricalFaceIndex { get; init; }

        public double ConeHalfAngle { get; init; }
        public bool ConeIsExpanding { get; init; }

        public enum HoleType
        {
            Simple, Counterbore, Countersink
        }

        public enum HoleBottomType
        {
            Through, Flat, Conical, Unknown
        }
    }
}
    