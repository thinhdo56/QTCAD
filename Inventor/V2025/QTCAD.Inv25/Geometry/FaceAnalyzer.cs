using Inventor;
using QTCAD.API.Geometry;
using System.Collections.Generic;

namespace QTCAD.Inv25.Geometry
{
    internal sealed class FaceAnalyzer
    {
        private readonly TransientGeometry _transientGeometry;
        List<int> edgeIndices = [];

        public FaceAnalyzer(TransientGeometry transientGeometry)
        {
            _transientGeometry = transientGeometry;
        }
        public bool IsInteriorCylindricalFace(Face face)
        {
            if (face.SurfaceType != SurfaceTypeEnum.kCylinderSurface)
            { 
                return false; 
            }

            Cylinder cylinder = (Cylinder)face.Geometry;
            double[] parameters = { 0.5, 0.5 };
            double[] points = new double[3];
            face.Evaluator.GetPointAtParam(ref parameters, ref points);

            Point point = _transientGeometry.CreatePoint(points[0], points[1], points[2]);
            double[] normals = new double[3];
            face.Evaluator.GetNormal(ref parameters, ref normals);

            Vector normal = _transientGeometry.CreateVector(normals[0], normals[1], normals[2]);
            normal.ScaleBy(cylinder.Radius);
            point.TranslateBy(normal);

            Line axisLine = _transientGeometry.CreateLine(cylinder.BasePoint, cylinder.AxisVector.AsVector());
            Line sampleLine = _transientGeometry.CreateLine(point, cylinder.AxisVector.AsVector());

            return sampleLine.IsColinearTo[axisLine, 0.001];
        }
        public FaceInfo Analyze(Face face, int index, UnitsOfMeasure unitsOfMeasure)
        {

            double radius = 0.0;
            double axisX = 0.0;
            double axisY = 0.0;
            double axisZ = 0.0;

            bool isInterior = false;

            if (face.SurfaceType == SurfaceTypeEnum.kCylinderSurface)
            {
                Cylinder cylinder = (Cylinder)face.Geometry;
                radius = unitsOfMeasure.ConvertUnits(cylinder.Radius, UnitsTypeEnum.kDatabaseLengthUnits, unitsOfMeasure.LengthUnits);
                axisX = cylinder.AxisVector.X;
                axisY = cylinder.AxisVector.Y;
                axisZ = cylinder.AxisVector.Z;
                isInterior = IsInteriorCylindricalFace(face);
            }
            Box2d paramRange = face.Evaluator.ParamRangeRect;

            double u = (paramRange.MinPoint.X + paramRange.MaxPoint.X) / 2.0;
            double v = (paramRange.MinPoint.Y + paramRange.MaxPoint.Y) / 2.0;

            double[] parameters = { u, v };
            double[] points = new double[3];

            face.Evaluator.GetPointAtParam(ref parameters, ref points);

            Point center = _transientGeometry.CreatePoint(points[0], points[1], points[2]);

            double[] facePoints = { center.X, center.Y, center.Z };
            double[] guessParams = { u, v };
            double[] maxDeviations = { 0.001, 0.001 };
            double[] normalParams = new double[2];
            SolutionNatureEnum[] solutionNatures = new SolutionNatureEnum[2];


            face.Evaluator.GetParamAtPoint(
                ref facePoints,
                ref guessParams,
                ref maxDeviations,
                ref normalParams,
                ref solutionNatures);

            double[] normals = new double[3];

            face.Evaluator.GetNormal(ref normalParams, ref normals);

            return new FaceInfo
            {
                Index = index,
                Area = face.Evaluator.Area,
                CenterX = center.X,
                CenterY = center.Y,
                CenterZ = center.Z,
                NormalX = normals[0],
                NormalY = normals[1],
                NormalZ = normals[2],
                Type = face.SurfaceType.ToString(),
                IsPlanar = face.SurfaceType == SurfaceTypeEnum.kPlaneSurface,
                Radius = radius,
                AxisX = axisX,
                AxisY = axisY,
                AxisZ = axisZ,
                IsInterior = isInterior
            };
        }
    }
}
