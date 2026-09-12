using Inventor;
using System;

namespace QTCAD.Inv25.Drawing
{
    internal sealed class DrawingScaleService
    {
        public double FitView(DrawingView view, double maxWidth, double maxHeight)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (maxWidth <= 0) throw new ArgumentOutOfRangeException(nameof(maxWidth));
            if (maxHeight <= 0) throw new ArgumentOutOfRangeException(nameof(maxHeight));
            if (view.Width <= 0 || view.Height <= 0) throw new InvalidOperationException("Drawing view has an invalid size.");

            double targetScale = Math.Min(maxWidth / view.Width, maxHeight / view.Height);
            double scale = GetRoundRatio(targetScale, 1000);
            view.Scale = scale;
            return scale;
        }

        private static double GetRoundRatio(double value, int maxNumber, double tolerance = 0.05)
        {
            double bestScale = 0;
            int bestComplexity = int.MaxValue;

            for (int a = 1; a <= maxNumber; a++)
            {
                int b = Math.Max(1, (int)Math.Round(a / value));
                if (b > maxNumber) continue;

                double scale = (double)a / b;
                double error = Math.Abs(scale - value) / value;

                if (error <= tolerance && a + b < bestComplexity)
                {
                    bestScale = scale;
                    bestComplexity = a + b;
                }
            }

            if (bestScale > 0) return bestScale;

            return value;
        }
    }
}