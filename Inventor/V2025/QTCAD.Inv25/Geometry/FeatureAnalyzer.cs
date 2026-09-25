using QTCAD.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Geometry
{
    internal sealed class FeatureAnalyzer
    {
        public IReadOnlyList<FeatureInfo> Analyze(IReadOnlyList<FaceInfo> faces)
        {
            List<FeatureInfo> features = new List<FeatureInfo>();
            int index = 0;

            foreach (FaceInfo face in faces)
            {
                if (!face.Type.Contains("Cylinder", StringComparison.OrdinalIgnoreCase)) continue;

                features.Add(new FeatureInfo
                {
                    Index = index++,
                    Type = "Cylinder",
                    Radius = face.Radius,
                    CenterX = face.CenterX,
                    CenterY = face.CenterY,
                    CenterZ = face.CenterZ,
                    AxisX = face.AxisX,
                    AxisY = face.AxisY,
                    AxisZ = face.AxisZ
                });
            }

            return features;
        }
    }
}
