using Inventor;
using QTCAD.API.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Geometry
{
    internal sealed class GeometryExtractionResult
    {
        public IReadOnlyList<FaceInfo> Faces { get; init; } = [];
        public IReadOnlyList<EdgeInfo> Edges { get; init; } = [];
    }
    internal sealed class GeometryExtractor
    {
        private readonly FaceAnalyzer _faceAnalyzer;
        public GeometryExtractor(FaceAnalyzer faceAnalyzer)
        {
            _faceAnalyzer = faceAnalyzer;
        }
        internal GeometryExtractionResult Extract(PartComponentDefinition definition, UnitsOfMeasure unitsOfMeasure)
        {
            List<FaceInfo> faces = [];
            List<EdgeInfo> edges = [];
            Dictionary<int, int> edgeMap = [];
            int faceIndex = 0;

            foreach (SurfaceBody body in definition.SurfaceBodies)
            {
                foreach (Face face in body.Faces)
                {
                    FaceInfo faceInfo = _faceAnalyzer.Analyze(face, faceIndex++, unitsOfMeasure);
                    List<int> edgeIndices = [];

                    foreach (Edge edge in face.Edges)
                    {
                        int hash = edge.GetHashCode();

                        if (!edgeMap.TryGetValue(hash, out int edgeIndex))
                        {
                            edgeIndex = edges.Count;
                            edgeMap.Add(hash, edgeIndex);

                            edges.Add(new EdgeInfo
                            {
                                Index = edgeIndex,
                                CurveType = edge.CurveType.ToString(),
                                Length = 0,
                                IsCircular = edge.CurveType == CurveTypeEnum.kCircleCurve,
                                IsLinear = edge.CurveType == CurveTypeEnum.kLineSegmentCurve
                            });
                        }

                        edgeIndices.Add(edgeIndex);
                    }

                    faces.Add(new FaceInfo
                    {
                        Index = faceInfo.Index,
                        EdgeIndices = edgeIndices,
                        Area = faceInfo.Area,
                        CenterX = faceInfo.CenterX,
                        CenterY = faceInfo.CenterY,
                        CenterZ = faceInfo.CenterZ,
                        NormalX = faceInfo.NormalX,
                        NormalY = faceInfo.NormalY,
                        NormalZ = faceInfo.NormalZ,
                        Type = faceInfo.Type,
                        IsPlanar = faceInfo.IsPlanar,
                        Radius = faceInfo.Radius,
                        AxisX = faceInfo.AxisX,
                        AxisY = faceInfo.AxisY,
                        AxisZ = faceInfo.AxisZ
                    });
                }
            }

            return new GeometryExtractionResult
            {
                Faces = faces,
                Edges = edges
            };
        }
    }
}
