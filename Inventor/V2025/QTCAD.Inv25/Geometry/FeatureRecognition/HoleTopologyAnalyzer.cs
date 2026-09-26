using QTCAD.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTCAD.Inv25.Geometry.FeatureRecognition
{
    internal sealed class HoleTopologyAnalyzer
    {
        public IReadOnlyList<FaceInfo> GetConnectedFaces(FaceInfo face, GeometryExtractionResult geometry)
        {
            HashSet<int> indices = [];

            foreach (int edgeIndex in face.EdgeIndices)
            {
                EdgeInfo? edge = geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex);
                if (edge == null) continue;

                foreach (int faceIndex in edge.AdjacentFaceIndices)
                {
                    if (faceIndex != face.Index)
                        indices.Add(faceIndex);
                }
            }

            return geometry.Faces.Where(x => indices.Contains(x.Index)).ToList();
        }

        public IReadOnlyList<EdgeInfo> GetCircularEdges(FaceInfo face, GeometryExtractionResult geometry)
        {
            return face.EdgeIndices.Select(index => geometry.Edges.FirstOrDefault(x => x.Index == index)).OfType<EdgeInfo>().Where(x => x.IsCircular).ToList();
        }
    }
}