using QTCAD.Core.Geometry;

namespace QTCAD.API.Geometry
{
    public interface IGeometryService
    {
        ModelGeometryInfo? Analyze();
    }
}
