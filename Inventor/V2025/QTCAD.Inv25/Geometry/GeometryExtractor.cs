using Inventor;
using QTCAD.Core.Geometry;
using QTCAD.Inv25.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Geometry
{
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
                        if (edge == null) continue;
                        int hash = edge.GetHashCode();

                        if (!edgeMap.TryGetValue(hash, out int edgeIndex))
                        {
                            edgeIndex = edges.Count;
                            edgeMap.Add(hash, edgeIndex);
                            double minParam;
                            double maxParam;
                            double length;

                            CurveEvaluator evaluator = edge.Evaluator;
                            if (evaluator == null) continue;

                            evaluator.GetParamExtents(out minParam, out maxParam);
                            evaluator.GetLengthAtParam(minParam, maxParam, out length);

                            length = ConversionTool.ToModelLength(length, unitsOfMeasure);

                            double centerX = 0.0;
                            double centerY = 0.0;
                            double centerZ = 0.0;

                            bool isCircular = edge.CurveType == CurveTypeEnum.kCircleCurve;
                            bool isLinear = edge.CurveType == CurveTypeEnum.kLineSegmentCurve;

                            if (isCircular)
                            {
                                Circle circle = (Circle)edge.Geometry;
                                Point center = circle.Center;

                                centerX = center.X;
                                centerY = center.Y;
                                centerZ = center.Z;
                            }

                            edges.Add(new EdgeInfo
                            {
                                Index = edgeIndex,
                                CurveType = edge.CurveType.ToString(),
                                Length = length,
                                IsCircular = isCircular,
                                IsLinear = isLinear,
                                CenterX = centerX,
                                CenterY = centerY,
                                CenterZ = centerZ
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
