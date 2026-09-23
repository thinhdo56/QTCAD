using Inventor;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Utilities;
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
                            double minParam;
                            double maxParam;
                            double length;
                            if (edge == null) continue;

                            CurveEvaluator evaluator = edge.Evaluator;
                            if (evaluator == null) continue;
                            evaluator.GetParamExtents(out minParam, out maxParam);
                            evaluator.GetLengthAtParam(minParam, maxParam, out length);
                            length = ConversionTool.ToModelLength(length, unitsOfMeasure);
                            edges.Add(new EdgeInfo
                            {
                                Index = edgeIndex,
                                CurveType = edge.CurveType.ToString(),
                                Length = length,
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
                        AxisZ = faceInfo.AxisZ,
                        IsInterior = faceInfo.IsInterior,
                        ConeHalfAngle = faceInfo.ConeHalfAngle,
                        ConeIsExpanding = faceInfo.ConeIsExpanding
                    });
                }
            }
            Dictionary<int, List<int>> edgeFaces = [];

            foreach (FaceInfo face in faces)
            {
                foreach (int edgeIndex in face.EdgeIndices)
                {
                    if (!edgeFaces.TryGetValue(edgeIndex, out List<int>? faceIndices))
                    {
                        faceIndices = [];
                        edgeFaces.Add(edgeIndex, faceIndices);
                    }

                    faceIndices.Add(face.Index);
                }
            }
            foreach (EdgeInfo edge in edges)
            {
                if (edgeFaces.TryGetValue(edge.Index, out List<int>? faceIndices))
                {
                    edge.AdjacentFaceIndices = faceIndices;
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
