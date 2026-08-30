using QTCAD.API.Commands;
using QTCAD.Inv25.Services;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;

namespace QTCAD.Inv25.Commands
{
    internal sealed class PropertyCommand : ICommand
    {
        private readonly PropertyService _propertyService;

        public string Id => "QTCAD.Properties";

        public string DisplayName => "Properties";

        public string Description => "Show active document properties";

        public PropertyCommand(PropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [SupportedOSPlatform("windows6.1")]
        public void Execute()
        {
            var properties = _propertyService.GetDocumentProperties();

            if (properties.Count == 0)
            {
                MessageBox.Show(
                    "Không tìm thấy Property nào.",
                    "QTCAD Properties");

                return;
            }

            StringBuilder message = new();

            foreach (var property in properties)
            {
                message.AppendLine(
                    $"[{property.Category}] {property.Name} = {property.Value}");
            }

            MessageBox.Show(
                message.ToString(),
                "QTCAD Properties");
        }
    }
}