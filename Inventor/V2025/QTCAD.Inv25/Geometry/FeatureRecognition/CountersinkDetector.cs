using QTCAD.Core.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace QTCAD.Inv25.Geometry.FeatureRecognition
{
    internal sealed class CountersinkDetector
    {
        private readonly HoleTopologyAnalyzer _topologyAnalyzer = new();

        public bool TryDetect(FaceInfo cylinderFace, GeometryExtractionResult geometry, out FaceInfo? coneFace, out EdgeInfo? boundaryEdge)
        {
            coneFace = null;
            boundaryEdge = null;

            foreach (EdgeInfo edge in _topologyAnalyzer.GetCircularEdges(cylinderFace, geometry))
            {
                List<FaceInfo> connectedFaces = edge.AdjacentFaceIndices.Where(index => index != cylinderFace.Index).Select(index => geometry.Faces.FirstOrDefault(x => x.Index == index)).OfType<FaceInfo>().ToList();

                FaceInfo? candidate = connectedFaces.FirstOrDefault(x => x.Type == "kConeSurface");
                if (candidate == null) continue;

                coneFace = candidate;
                boundaryEdge = edge;
                return true;
            }

            return false;
        }
    }
}
