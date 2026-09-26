using QTCAD.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTCAD.Inv25.Geometry.FeatureRecognition
{
    internal sealed class CounterboreDetector
    {
        public bool TryDetect(FaceInfo cylinderFace, GeometryExtractionResult geometry, out FaceInfo? secondaryCylinder)
        {
            secondaryCylinder = null;

            foreach (int edgeIndex in cylinderFace.EdgeIndices)
            {
                EdgeInfo? cylinderEdge = geometry.Edges.FirstOrDefault(x => x.Index == edgeIndex);
                if (cylinderEdge == null || !cylinderEdge.IsCircular) continue;

                List<FaceInfo> connectedFaces = cylinderEdge.AdjacentFaceIndices.Where(x => x != cylinderFace.Index).Select(x => geometry.Faces.FirstOrDefault(f => f.Index == x)).OfType<FaceInfo>().ToList();

                foreach (FaceInfo stepFace in connectedFaces.Where(x => x.Type == "kPlaneSurface"))
                {
                    foreach (int stepEdgeIndex in stepFace.EdgeIndices)
                    {
                        if (stepEdgeIndex == cylinderEdge.Index) continue;

                        EdgeInfo? stepEdge = geometry.Edges.FirstOrDefault(x => x.Index == stepEdgeIndex);
                        if (stepEdge == null || !stepEdge.IsCircular) continue;

                        foreach (int faceIndex in stepEdge.AdjacentFaceIndices)
                        {
                            if (faceIndex == cylinderFace.Index) continue;

                            FaceInfo? candidateCylinder = geometry.Faces.FirstOrDefault(x => x.Index == faceIndex);
                            if (candidateCylinder == null || candidateCylinder.Type != "kCylinderSurface") continue;
                            if (!IsCoaxial(cylinderFace, candidateCylinder)) continue;
                            if (Math.Abs(candidateCylinder.Radius - cylinderFace.Radius) < 0.000001) continue;

                            secondaryCylinder = candidateCylinder;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsCoaxial(FaceInfo firstCylinder, FaceInfo secondCylinder)
        {
            double dot = Math.Abs(firstCylinder.AxisX * secondCylinder.AxisX + firstCylinder.AxisY * secondCylinder.AxisY + firstCylinder.AxisZ * secondCylinder.AxisZ);
            return dot >= 0.999999;
        }
    }
}
