using Inventor;
using QTCAD.API.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Drawing
{
    internal sealed class DrawingViewService
    {
        private readonly Sheet _sheet;

        public DrawingViewService(DrawingDocument document)
        {
            _sheet = document.ActiveSheet;
        }

        public DrawingView CreateBaseView(_Document model, Point2d position, double scale, BaseViewType viewType)
        {
            ViewOrientationTypeEnum orientation = StandardView.GetBaseView(viewType);
            return _sheet.DrawingViews.AddBaseView(model, position, scale, orientation, DrawingViewStyleEnum.kHiddenLineRemovedDrawingViewStyle);
        }

        public DrawingView CreateIsoView(_Document model, Point2d position, double scale, IsoViewType viewType)
        {
            ViewOrientationTypeEnum orientation = StandardView.GetIsoView(viewType);
            return _sheet.DrawingViews.AddBaseView(model, position, scale, orientation, DrawingViewStyleEnum.kShadedDrawingViewStyle);
        }

    }
}
