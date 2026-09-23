using Inventor;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Geometry;
using QTCAD.Inv25.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QTCAD.Inv25.Geometry.FeatureRecognition
{
    internal sealed class HoleDetector
    {
        public IReadOnlyList<HoleFeature> Detect(GeometryExtractionResult geometry)
        {
            
            List<HoleFeature> holes = [];

            foreach (FaceInfo face in geometry.Faces)
            {
                if (face.Type != "kCylinderSurface") continue;

                List<EdgeInfo> boundaryEdges = face.EdgeIndices.Select(edgeIndex => geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex)).Where(x => x != null).Cast<EdgeInfo>().ToList();

                if (!face.IsInterior) continue;
                if (boundaryEdges.Count != 2) continue;
                if (!boundaryEdges.All(x => x.IsCircular)) continue;

                FaceAdjacencyAnalyzer adjacencyAnalyzer = new FaceAdjacencyAnalyzer();
                IReadOnlyList<int> adjacentFaces = adjacencyAnalyzer.GetAdjacentFaceIndices(face, geometry.Edges);
                FaceInfo? coneFace = adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).FirstOrDefault(x => x != null && x.Type == "kConeSurface");
                if (adjacentFaces.Count != 2) continue;
                bool isBlind = IsBlindHole(face, geometry, adjacentFaces);

                double depth = isBlind ? GetBlindHoleDepth(face, geometry, adjacentFaces) : 0.0;

                holes.Add(new HoleFeature
                {
                    CylindricalFaceIndex = face.Index,
                    Radius = face.Radius,
                    BoundaryEdgeIndices = face.EdgeIndices,
                    AdjacentFaceIndices = adjacentFaces,
                    IsBlind = isBlind,
                    Depth = depth
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
            FaceInfo? bottomFace = adjacentFaces.Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).FirstOrDefault(x => x != null && (x.Type == "kPlaneSurface" || x.Type == "kConeSurface"));
            if (bottomFace == null) return 0.0;

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