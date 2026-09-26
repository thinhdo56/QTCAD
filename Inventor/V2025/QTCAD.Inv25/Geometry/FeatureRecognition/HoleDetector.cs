using Inventor;
using QTCAD.Core.Features;
using QTCAD.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
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
                if (adjacentFaces.Count != 2) continue;
                bool isBlind = IsBlindHole(face, geometry, adjacentFaces);

                if (!isBlind)
                {
                    bottomType = HoleBottomType.Through;
                }
                else
                {
                    FaceInfo? bottomFace = adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).FirstOrDefault(x =>x != null &&(x.Type == "kPlaneSurface" ||x.Type == "kConeSurface"));

                    if (bottomFace != null)
                    {
                        bottomType = bottomFace.Type switch
                        {
                            "kPlaneSurface" => HoleBottomType.Flat,
                            "kConeSurface" => HoleBottomType.Conical,
                            _ => HoleBottomType.Unknown
                        };
                    }
                }
                double coneHalfAngle = 0.0;
                bool coneIsExpanding = false;

                if (bottomType == HoleBottomType.Conical)
                {
                    FaceInfo? coneFace = adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).FirstOrDefault(x => x != null && x.Type == "kConeSurface");

                    if (coneFace != null)
                    {
                        coneHalfAngle = coneFace.ConeHalfAngle;
                        coneIsExpanding = coneFace.ConeIsExpanding;
                    }
                }
                double depth = isBlind ? GetBlindHoleDepth(face, geometry, adjacentFaces) : 0.0;

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

                if (adjacentFace.EdgeIndices.Count != 1) continue;

                EdgeInfo? edge = geometry.Edges.FirstOrDefault(x => x.Index == adjacentFace.EdgeIndices[0]);
                if (edge == null || !edge.IsCircular) continue;

                return true;
            }
            return false;
        }
        private double GetBlindHoleDepth(FaceInfo cylinderFace, GeometryExtractionResult geometry, IReadOnlyList<int> adjacentFaces)
        {
            FaceInfo? bottomFace;
            if (adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).FirstOrDefault(x => x != null && x.Type == "kConeSurface") is FaceInfo coneFace)
            {
                bottomFace = coneFace;
            }
            else
            {
                bottomFace = adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).FirstOrDefault(x => x != null && x.Type == "kPlaneSurface");
            }
            if (bottomFace == null)
            {
                return 0.0;
            }

            double dx = bottomFace.CenterX - cylinderFace.CenterX;
            double dy = bottomFace.CenterY - cylinderFace.CenterY;
            double dz = bottomFace.CenterZ - cylinderFace.CenterZ;

            double axialDistance = Math.Abs(dx * cylinderFace.AxisX + dy * cylinderFace.AxisY + dz * cylinderFace.AxisZ);

            if (bottomFace.Type == "kPlaneSurface")
            {
                return axialDistance * 2.0;
            }

            if (bottomFace.Type == "kConeSurface")
            {
                if (bottomFace.ConeHalfAngle <= 0.0) return 0.0;

                double coneDepth = cylinderFace.Radius / Math.Tan(bottomFace.ConeHalfAngle);

                return axialDistance * 2.0 + coneDepth;
            }

            return 0.0;
        }
    }
}