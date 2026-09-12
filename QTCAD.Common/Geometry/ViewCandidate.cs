using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTCAD.API.Drawing;    

namespace QTCAD.Common.Geometry
{
    public sealed class ViewCandidate
    {
        public BaseViewType ViewType { get; init; }
        public double Score { get; set; }
        public int VisibleFaces { get; set; }
        public int HiddenFaces { get; set; }
        public int CircularFeatures { get; set; }
        public int FeatureCount { get; set; }
        public double VisibleArea { get; set; }
    }
}
