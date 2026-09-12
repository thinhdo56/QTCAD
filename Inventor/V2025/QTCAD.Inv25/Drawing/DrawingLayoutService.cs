using Inventor;
using QTCAD.Inv25.Adapter;
using System;

namespace QTCAD.Inv25.Drawing
{
    internal sealed class DrawingLayoutService
    {
        private readonly Sheet _sheet;
        private readonly TransientGeometry _geometry;

        public DrawingLayoutService(InventorContext context, DrawingDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            _sheet = document.ActiveSheet;
            _geometry = context.Application.TransientGeometry;
        }

        public void Layout(DrawingView frontView, DrawingView? topView = null, DrawingView? rightView = null, DrawingView? isoView = null)
        {
            if (frontView == null) throw new ArgumentNullException(nameof(frontView));

            double gap = Math.Min(_sheet.Width, _sheet.Height) * 0.05;
            double frontX = _sheet.Width * 0.35;
            double frontY = _sheet.Height * 0.55;

            frontView.Position = _geometry.CreatePoint2d(frontX, frontY);

            if (topView != null) LayoutTopView(frontView, topView, gap);
            if (rightView != null) LayoutRightView(frontView, rightView, gap);
            if (isoView != null) LayoutIsoView(isoView);
        }

        private void LayoutTopView(DrawingView frontView, DrawingView topView, double gap)
        {
            double x = frontView.Position.X;
            double y = frontView.Position.Y - frontView.Height / 2 - gap - topView.Height / 2;
            topView.Position = _geometry.CreatePoint2d(x, y);
        }

        private void LayoutRightView(DrawingView frontView, DrawingView rightView, double gap)
        {
            double x = frontView.Position.X + frontView.Width / 2 + gap + rightView.Width / 2;
            double y = frontView.Position.Y;
            rightView.Position = _geometry.CreatePoint2d(x, y);
        }

        private void LayoutIsoView(DrawingView isoView)
        {
            double x = _sheet.Width * 0.75;
            double y = _sheet.Height * 0.30;
            isoView.Position = _geometry.CreatePoint2d(x, y);
        }
    }
}