using Inventor;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Geometry;
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

                List<EdgeInfo> boundaryEdges = face.EdgeIndices
                    .Select(edgeIndex => geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex))
                    .Where(x => x != null)
                    .Cast<EdgeInfo>()
                    .ToList();

                if (!face.IsInterior) continue;
                if (boundaryEdges.Count != 2) continue;
                if (!boundaryEdges.All(x => x.IsCircular)) continue;

                FaceAdjacencyAnalyzer adjacencyAnalyzer = new FaceAdjacencyAnalyzer();
                IReadOnlyList<int> adjacentFaces = adjacencyAnalyzer.GetAdjacentFaceIndices(face, geometry.Edges);

                if (adjacentFaces.Count != 2) continue;

                bool isBlind = IsBlindHole(face, geometry, adjacentFaces);

                holes.Add(new HoleFeature
                {
                    CylindricalFaceIndex = face.Index,
                    Radius = face.Radius,
                    BoundaryEdgeIndices = face.EdgeIndices,
                    AdjacentFaceIndices = adjacentFaces,
                    IsBlind = isBlind
                });
            }

            return holes;
        }
        private bool IsBlindHole(FaceInfo cylinderFace, GeometryExtractionResult geometry, IReadOnlyList<int> adjacentFaces)
        {
            foreach (int faceIndex in adjacentFaces)
            {
                FaceInfo? adjacentFace = geometry.Faces.FirstOrDefault(x => x.Index == faceIndex);
                if (adjacentFace == null || !adjacentFace.IsPlanar) continue;
                if (adjacentFace.EdgeIndices.Count != 1) continue;

                EdgeInfo? edge = geometry.Edges.FirstOrDefault(x => x.Index == adjacentFace.EdgeIndices[0]);
                if (edge == null || !edge.IsCircular) continue;

                return true;
            }
            return false;
        }
    }
}