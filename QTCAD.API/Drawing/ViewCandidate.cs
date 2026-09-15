using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTCAD.API.Drawing;    

namespace QTCAD.API.Geometry
{
    public sealed class ViewCandidate
    {
        public BaseViewType ViewType { get; init; }

        public double Score { get; init; }

        public int VisibleFaces { get; init; }

        public int HiddenFaces { get; init; }

        public int CircularFeatures { get; init; }

        public int FeatureCount { get; init; }

        public double VisibleArea { get; init; }
    }
}
