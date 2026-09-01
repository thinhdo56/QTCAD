using Inventor;
using QTCAD.API.Geometry;

namespace QTCAD.Inv25.Geometry
{
    internal sealed class FaceAnalyzer
    {
        private readonly TransientGeometry _transientGeometry;

        public FaceAnalyzer(TransientGeometry transientGeometry)
        {
            _transientGeometry = transientGeometry;
        }

        public FaceGeometryInfo Analyze(Face face, int index)
        {

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

            return new FaceGeometryInfo
            {
                Index = index,
                Area = face.Evaluator.Area,
                CenterX = center.X,
                CenterY = center.Y,
                CenterZ = center.Z,
                NormalX = normals[0],
                NormalY = normals[1],
                NormalZ = normals[2],
                Type = face.SurfaceType.ToString()
            };
        }
    }
}
