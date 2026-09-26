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
        public IReadOnlyList<QTCADHoleFeature> Detect(GeometryExtractionResult geometry)
        {
            
            List<QTCADHoleFeature> holes = [];

            foreach (FaceInfo face in geometry.Faces)
            {

                if (face.Type != "kCylinderSurface") continue;

                List<EdgeInfo> boundaryEdges = face.EdgeIndices.Select(edgeIndex => geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex)).Where(x => x != null).Cast<EdgeInfo>().ToList();

                if (!face.IsInterior) continue;
                if (boundaryEdges.Count != 2) continue;
                if (!boundaryEdges.All(x => x.IsCircular)) continue;
                HoleBottomType bottomType = HoleBottomType.Unknown;

                FaceAdjacencyAnalyzer adjacencyAnalyzer = new FaceAdjacencyAnalyzer();
                IReadOnlyList<int> adjacentFaces = adjacencyAnalyzer.GetAdjacentFaceIndices(face, geometry.Edges);

                bool isBlind = IsBlindHole(face, geometry, adjacentFaces);
                double coneHalfAngle = 0.0;
                bool coneIsExpanding = false;

                if (!isBlind)
                {
                    bottomType = HoleBottomType.Through;
                }
                else
                {
                    FaceInfo? coneFace = adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).OfType<FaceInfo>().FirstOrDefault(x => x.Type == "kConeSurface");
                    FaceInfo? planeFace = adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).OfType<FaceInfo>().FirstOrDefault(x => x.Type == "kPlaneSurface");

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
                    else
                    {
                        bottomType = HoleBottomType.Unknown;
                    }
                }

                double depth = isBlind ? GetBlindHoleDepth(face, geometry) : 0.0;

                holes.Add(new QTCADHoleFeature
                {
                    // 1. Identity
                    Id = face.Index,
                    Type = "Hole",
                    Name = $"Hole_{face.Index}",

                    // 2. Generic topology
                    FaceIndices = new[] { face.Index },
                    EdgeIndices = face.EdgeIndices,

                    // 3. Generic geometry
                    CenterX = face.CenterX,
                    CenterY = face.CenterY,
                    CenterZ = face.CenterZ,

                    AxisX = face.AxisX,
                    AxisY = face.AxisY,
                    AxisZ = face.AxisZ,

                    // 4. Hole geometry
                    Radius = face.Radius,

                    // 5. Hole topology
                    CylindricalFaceIndex = face.Index,
                    BoundaryEdgeIndices = face.EdgeIndices,
                    AdjacentFaceIndices = adjacentFaces,

                    // 6. Hole characteristics
                    IsBlind = isBlind,
                    Depth = depth,
                    BottomType = bottomType,

                    // 7. Cone characteristics
                    ConeHalfAngle = coneHalfAngle,
                    ConeIsExpanding = coneIsExpanding,

                    // 8. Recognition metadata
                    Source = "RuleBased",
                    Confidence = 1.0
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