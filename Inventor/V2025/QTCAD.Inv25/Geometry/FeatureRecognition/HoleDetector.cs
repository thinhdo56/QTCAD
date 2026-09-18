using QTCAD.API.Geometry;
using QTCAD.Inv25.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace QTCAD.Inv25.Geometry.FeatureRecognition
{
    internal sealed class HoleDetector
    {
        public IReadOnlyList<HoleFeature> Detect(GeometryExtractionResult geometry)
        {
            List<HoleFeature> holes = [];

            foreach (FaceInfo face in geometry.Faces)
            {
                if (face.Type != "kCylinderSurface")
                {
                    continue;
                }

                List<EdgeInfo> boundaryEdges = face.EdgeIndices
                    .Select(edgeIndex => geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex))
                    .Where(x => x != null)
                    .Cast<EdgeInfo>()
                    .ToList();

                if (boundaryEdges.Count != 2)
                {
                    continue;
                }

                if (!boundaryEdges.All(x => x.IsCircular))
                {
                    continue;
                }

                FaceAdjacencyAnalyzer adjacencyAnalyzer = new FaceAdjacencyAnalyzer();

                IReadOnlyList<int> adjacentFaces =
                    adjacencyAnalyzer.GetAdjacentFaceIndices(face, geometry.Edges);

                if (adjacentFaces.Count != 2)
                {
                    continue;
                }

                holes.Add(new HoleFeature
                {
                    CylindricalFaceIndex = face.Index,
                    Radius = face.Radius,
                    BoundaryEdgeIndices = face.EdgeIndices,
                    AdjacentFaceIndices = adjacentFaces
                });
            }

            return holes;
        }
    }
}