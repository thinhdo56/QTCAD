using Inventor;
using QTCAD.API.Commands;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Services;
using System.Windows.Forms;

namespace QTCAD.Inv25.Commands
{
    internal sealed class GeometryAnalysisCommand : ICommand
    {
        private readonly IGeometryService _geometryService;

        public string Id => "QTCAD.GeometryAnalysis";

        public string DisplayName => "Geometry Analysis";

        public string Description => "Analyze active model geometry";

        public GeometryAnalysisCommand(IGeometryService geometryService)
        {
            _geometryService = geometryService;
        }
        public void Execute()
        {
            var geometry = _geometryService.Analyze();

            if (geometry == null)
            {
                MessageBox.Show("Không có Model hợp lệ.", "QTCAD Geometry Analysis");
                return;
            }
            string specificInfo = geometry.GetType().Name == "Part" ? $"Bodies: {geometry.BodyCount}" : $"Occurrences: {geometry.OccurrenceCount}";
            MessageBox.Show($"Type: {geometry.GetType().Name}\nX Size: {geometry.XSize:F2} {geometry.Unit}\nY Size: {geometry.YSize:F2} {geometry.Unit}\nZ Size: {geometry.ZSize:F2} {geometry.Unit}\n\n{specificInfo}", "QTCAD Geometry Analysis");
        }
    }
}