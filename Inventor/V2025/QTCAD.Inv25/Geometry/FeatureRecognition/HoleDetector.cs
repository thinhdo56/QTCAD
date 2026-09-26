using Inventor;
using QTCAD.Core.Features;
using QTCAD.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Forms;
using static QTCAD.Core.Features.HoleFeature;
using QTCADHoleFeature = QTCAD.Core.Features.HoleFeature;

namespace QTCAD.Inv25.Geometry.FeatureRecognition
{
    internal sealed class HoleDetector: IFeatureDetector<QTCADHoleFeature>
    {
        private readonly CounterboreDetector _counterboreDetector = new();
        private readonly CountersinkDetector _countersinkDetector = new();
        public IReadOnlyList<QTCADHoleFeature> Detect(GeometryExtractionResult geometry)
        {
            HashSet<int> consumedFaceIndices = [];
            List<QTCADHoleFeature> holes = [];

            foreach (FaceInfo face in geometry.Faces)
            {
                if (face.Type != "kCylinderSurface") continue;
                if (consumedFaceIndices.Contains(face.Index)) continue;

                List<EdgeInfo> boundaryEdges = face.EdgeIndices.Select(edgeIndex => geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex)).Where(x => x != null).Cast<EdgeInfo>().ToList();

                if (!face.IsInterior) continue;
                if (boundaryEdges.Count != 2) continue;
                if (!boundaryEdges.All(x => x.IsCircular)) continue;

                FaceAdjacencyAnalyzer adjacencyAnalyzer = new FaceAdjacencyAnalyzer();
                IReadOnlyList<int> adjacentFaces = adjacencyAnalyzer.GetAdjacentFaceIndices(face, geometry.Edges);

                bool hasCounterbore = _counterboreDetector.TryDetect(face, geometry, out FaceInfo? secondaryCylinder);

                HoleType holeType = HoleType.Simple;
                FaceInfo mainCylinder = face;
                FaceInfo? counterboreCylinder = null;
                FaceInfo? countersinkCone = null;
                EdgeInfo? countersinkEdge = null;

                if (hasCounterbore && secondaryCylinder != null)
                {
                    holeType = HoleType.Counterbore;

                    if (face.Radius <= secondaryCylinder.Radius)
                    {
                        mainCylinder = face;
                        counterboreCylinder = secondaryCylinder;
                    }
                    else
                    {
                        mainCylinder = secondaryCylinder;
                        counterboreCylinder = face;
                    }

                    consumedFaceIndices.Add(face.Index);
                    consumedFaceIndices.Add(secondaryCylinder.Index);
                }
                else
                {
                    if (!IsBlindHole(face, geometry, adjacentFaces) && _countersinkDetector.TryDetect(face, geometry, out countersinkCone, out countersinkEdge))
                    {
                        holeType = HoleType.Countersink;
                    }
                }

                IReadOnlyList<int> mainAdjacentFaces = adjacencyAnalyzer.GetAdjacentFaceIndices(mainCylinder, geometry.Edges);

                bool isBlind = IsBlindHole(mainCylinder, geometry, mainAdjacentFaces);

                HoleBottomType bottomType = HoleBottomType.Unknown;
                double coneHalfAngle = 0.0;
                bool coneIsExpanding = false;

                if (!isBlind)
                {
                    bottomType = HoleBottomType.Through;
                }
                else
                {
                    FaceInfo? coneFace = mainAdjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).OfType<FaceInfo>().FirstOrDefault(x => x.Type == "kConeSurface");
                    FaceInfo? planeFace = mainAdjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).OfType<FaceInfo>().FirstOrDefault(x => x.Type == "kPlaneSurface");

                    if (coneFace != null)
                    {
                        bottomType = HoleBottomType.Conical;
                        coneHalfAngle = coneFace.ConeHalfAngle;
                        coneIsExpanding = coneFace.ConeIsExpanding;
                    }
                    else if (planeFace != null)
                    {
                        bottomType = HoleBottomType.Flat;
                    }
                }

                double depth = isBlind ? GetBlindHoleDepth(mainCylinder, geometry) : GetCylinderDepth(mainCylinder, geometry.Edges);
                double secondaryDepth = counterboreCylinder != null ? GetCylinderDepth(counterboreCylinder, geometry.Edges) : 0.0;

                holes.Add(new QTCADHoleFeature
                {
                    Id = mainCylinder.Index,
                    Type = "Hole",
                    Name = $"Hole_{mainCylinder.Index}",

                    FaceIndices = counterboreCylinder != null ? new[] { mainCylinder.Index, counterboreCylinder.Index } : new[] { mainCylinder.Index },
                    EdgeIndices = counterboreCylinder != null ? mainCylinder.EdgeIndices.Concat(counterboreCylinder.EdgeIndices).Distinct().ToArray() : mainCylinder.EdgeIndices,

                    CenterX = mainCylinder.CenterX,
                    CenterY = mainCylinder.CenterY,
                    CenterZ = mainCylinder.CenterZ,

                    AxisX = mainCylinder.AxisX,
                    AxisY = mainCylinder.AxisY,
                    AxisZ = mainCylinder.AxisZ,

                    Radius = mainCylinder.Radius,

                    CylindricalFaceIndex = mainCylinder.Index,
                    BoundaryEdgeIndices = mainCylinder.EdgeIndices,
                    AdjacentFaceIndices = mainAdjacentFaces,

                    IsBlind = isBlind,
                    Depth = depth,
                    BottomType = bottomType,

                    ConeHalfAngle = coneHalfAngle,
                    ConeIsExpanding = coneIsExpanding,

                    Source = "RuleBased",
                    Confidence = 1.0,

                    TypeOfHole = holeType,

                    SecondaryCylindricalFaceIndex = counterboreCylinder?.Index ?? -1,
                    SecondaryRadius = counterboreCylinder?.Radius ?? 0.0,
                    SecondaryDepth = secondaryDepth,

                    ProfileFaceIndices = counterboreCylinder != null ? new[] { mainCylinder.Index, counterboreCylinder.Index } : new[] { mainCylinder.Index },
                    ProfileEdgeIndices = counterboreCylinder != null ? mainCylinder.EdgeIndices.Concat(counterboreCylinder.EdgeIndices).Distinct().ToArray() : mainCylinder.EdgeIndices,

                    CountersinkAngle = countersinkCone?.ConeHalfAngle ?? 0.0
                });
            }
            return holes;
        }
        private bool IsBlindHole(FaceInfo cylinderFace, GeometryExtractionResult geometry, IReadOnlyList<int> adjacentFaces)
        {
            foreach (int faceIndex in adjacentFaces)
            {
                FaceInfo? adjacentFace = geometry.Faces.FirstOrDefault(x => x.Index == faceIndex);
                if (adjacentFace == null) continue;
                if (adjacentFace.Type != "kPlaneSurface" && adjacentFace.Type != "kConeSurface") continue;

                foreach (int edgeIndex in adjacentFace.EdgeIndices)
                {
                    EdgeInfo? edge = geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex);
                    if (edge == null || !edge.IsCircular) continue;
                    if (!cylinderFace.EdgeIndices.Contains(edge.Index)) continue;

                    return true;
                }
            }

            return false;
        }
        private static double GetBlindHoleDepth(FaceInfo cylinderFace, GeometryExtractionResult geometry)
        {
            double cylinderDepth = GetCylinderDepth(cylinderFace, geometry.Edges);

            foreach (int edgeIndex in cylinderFace.EdgeIndices)
            {
                EdgeInfo? edge = geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex);
                if (edge == null || !edge.IsCircular) continue;

                List<FaceInfo> connectedFaces = edge.AdjacentFaceIndices.Where(index => index != cylinderFace.Index).Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).OfType<FaceInfo>().ToList();

                FaceInfo? coneFace = connectedFaces.FirstOrDefault(x => x.Type == "kConeSurface");

                if (coneFace != null)
                {
                    double angle = coneFace.ConeHalfAngle;
                    if (angle <= 0.0) return cylinderDepth;

                    double coneDepth = cylinderFace.Radius / Math.Tan(angle);
                    return cylinderDepth + coneDepth;
                }
            }

            return cylinderDepth;
        }
        private static double GetCylinderDepth(FaceInfo cylinderFace, IReadOnlyList<EdgeInfo> edges)
        {
            List<EdgeInfo> circularEdges = cylinderFace.EdgeIndices.Select(index => edges.FirstOrDefault(x => x.Index == index)).OfType<EdgeInfo>().Where(x => x.IsCircular).ToList();
            if (circularEdges.Count != 2) return 0.0;

            EdgeInfo edge1 = circularEdges[0];
            EdgeInfo edge2 = circularEdges[1];

            double dx = edge2.CenterX - edge1.CenterX;
            double dy = edge2.CenterY - edge1.CenterY;
            double dz = edge2.CenterZ - edge1.CenterZ;

            return Math.Abs(dx * cylinderFace.AxisX + dy * cylinderFace.AxisY + dz * cylinderFace.AxisZ);
        }
    }
}