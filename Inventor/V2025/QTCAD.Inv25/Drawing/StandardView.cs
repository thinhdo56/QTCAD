using Inventor;
using QTCAD.API.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Drawing
{
    internal static class StandardView
    {
        public static ViewOrientationTypeEnum GetBaseView(BaseViewType view)
        {
            return view switch
            {
                BaseViewType.Front => ViewOrientationTypeEnum.kFrontViewOrientation,
                BaseViewType.Back => ViewOrientationTypeEnum.kBackViewOrientation,
                BaseViewType.Left => ViewOrientationTypeEnum.kLeftViewOrientation,
                BaseViewType.Right => ViewOrientationTypeEnum.kRightViewOrientation,
                BaseViewType.Top => ViewOrientationTypeEnum.kTopViewOrientation,
                BaseViewType.Bottom => ViewOrientationTypeEnum.kBottomViewOrientation,
                _ => throw new ArgumentOutOfRangeException(nameof(view))
            };
        }

        public static ViewOrientationTypeEnum GetIsoView(IsoViewType view)
        {
            return view switch
            {
                IsoViewType.IsoTopRight => ViewOrientationTypeEnum.kIsoTopRightViewOrientation,
                IsoViewType.IsoTopLeft => ViewOrientationTypeEnum.kIsoTopLeftViewOrientation,
                IsoViewType.IsoBottomRight => ViewOrientationTypeEnum.kIsoBottomRightViewOrientation,
                IsoViewType.IsoBottomLeft => ViewOrientationTypeEnum.kIsoBottomLeftViewOrientation,
                _ => throw new ArgumentOutOfRangeException(nameof(view))
            };
        }
    }
}
