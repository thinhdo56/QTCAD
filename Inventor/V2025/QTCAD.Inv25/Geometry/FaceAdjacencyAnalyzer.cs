using QTCAD.API.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace QTCAD.Inv25.Geometry
{
    internal sealed class FaceAdjacencyAnalyzer
    {
        public IReadOnlyList<int> GetAdjacentFaceIndices(FaceInfo face, IReadOnlyList<EdgeInfo> edges)
        {
            HashSet<int> adjacentFaces = [];

            foreach (int edgeIndex in face.EdgeIndices)
            {
                EdgeInfo? edge = edges.FirstOrDefault(x => x.Index == edgeIndex);
                if (edge == null)
                {
                    continue;
                }

                foreach (int faceIndex in edge.AdjacentFaceIndices)
                {
                    if (faceIndex != face.Index)
                    {
                        adjacentFaces.Add(faceIndex);
                    }
                }
            }
            return adjacentFaces.ToList();
        }
    }
}
