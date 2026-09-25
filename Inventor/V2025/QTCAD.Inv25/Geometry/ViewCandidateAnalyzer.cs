using QTCAD.API.Drawing;
using QTCAD.API.Geometry;
using QTCAD.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTCAD.Inv25.Geometry
{
    internal sealed class ViewCandidateAnalyzer
    {
        private const double NormalAngleTolerance = Math.PI / 4;
        private static readonly double NormalThreshold = Math.Cos(NormalAngleTolerance);
        private const double AxisAngleTolerance = Math.PI / 6;
        private static readonly double AxisThreshold = Math.Cos(AxisAngleTolerance);

        public IReadOnlyList<ViewCandidate> Analyze(ModelGeometryInfo geometry)
        {
            List<ViewCandidate> candidates = new List<ViewCandidate>();
            foreach (BaseViewType viewType in Enum.GetValues<BaseViewType>()) candidates.Add(AnalyzeView(geometry, viewType));
            return candidates.OrderByDescending(x => x.Score).ToList();
        }

        private ViewCandidate AnalyzeView(ModelGeometryInfo geometry, BaseViewType viewType)
        {
            GetViewDirection(viewType, out double dx, out double dy, out double dz);
            int visibleFaces = 0;
            int circularFeatures = 0;
            double visibleArea = 0.0;

            foreach (FaceInfo face in geometry.Faces)
            {
                double dot = face.NormalX * dx + face.NormalY * dy + face.NormalZ * dz;

                if (dot >= NormalThreshold)
                {
                    visibleFaces++;
                    visibleArea += face.Area * dot;
                }

                if (IsCircularFeature(face, dx, dy, dz)) circularFeatures++;
            }

            int totalFaces = geometry.FaceCount > 0 ? geometry.FaceCount : geometry.Faces.Count;
            int hiddenFaces = Math.Max(0, totalFaces - visibleFaces);
            double faceRatio = totalFaces > 0 ? (double)visibleFaces / totalFaces : 0.0;
            double areaRatio = geometry.SurfaceArea > 0 ? visibleArea / geometry.SurfaceArea : 0.0;
            double circularRatio = geometry.Faces.Count > 0 ? (double)circularFeatures / geometry.Faces.Count : 0.0;
            double score = faceRatio * 0.3 + areaRatio * 0.4 + circularRatio * 0.3;

            return new ViewCandidate
            {
                ViewType = viewType,
                Score = score,
                VisibleFaces = visibleFaces,
                HiddenFaces = hiddenFaces,
                CircularFeatures = circularFeatures,
                FeatureCount = geometry.Features?.Count ?? 0,
                VisibleArea = visibleArea
            };
        }

        private bool IsCircularFeature(FaceInfo face, double dx, double dy, double dz)
        {
            if (!face.Type.Contains("Cylinder", StringComparison.OrdinalIgnoreCase)) return false;
            if (face.Radius <= 0.0) return false;

            double dot = Math.Abs(face.AxisX * dx + face.AxisY * dy + face.AxisZ * dz);
            return dot >= AxisThreshold;
        }

        private void GetViewDirection(BaseViewType viewType, out double x, out double y, out double z)
        {
            switch (viewType)
            {
                case BaseViewType.Front:
                    x = 0; y = 0; z = 1;
                    break;
                case BaseViewType.Back:
                    x = 0; y = 0; z = -1;
                    break;
                case BaseViewType.Left:
                    x = -1; y = 0; z = 0;
                    break;
                case BaseViewType.Right:
                    x = 1; y = 0; z = 0;
                    break;
                case BaseViewType.Top:
                    x = 0; y = 1; z = 0;
                    break;
                case BaseViewType.Bottom:
                    x = 0; y = -1; z = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewType));
            }
        }
    }
}