using QTCAD.Core.Geometry;

namespace QTCAD.Core.Features
{
    public interface IFeatureDetector<out TFeature>
        where TFeature : QTCADFeature
    {
        IReadOnlyList<TFeature> Detect(GeometryExtractionResult geometry);
    }
}