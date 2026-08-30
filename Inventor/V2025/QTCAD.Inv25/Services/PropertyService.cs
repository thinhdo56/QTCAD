using Inventor;
using QTCAD.API.Properties;
using QTCAD.Inv25.Adapter;
using System.Collections.Generic;

namespace QTCAD.Inv25.Services
{
    internal sealed class PropertyService : IPropertyService
    {
        private readonly InventorContext _context;

        public PropertyService(InventorContext context)
        {
            _context = context;
        }

        public IReadOnlyList<PropertyInfo> GetDocumentProperties()
        {
            List<PropertyInfo> result = new();

            Document? document = _context.ActiveDocument;

            if (document == null)
            {
                return result;
            }

            PropertySets propertySets = document.PropertySets;

            for (int i = 1; i <= propertySets.Count; i++)
            {
                PropertySet propertySet = propertySets[i];

                for (int j = 1; j <= propertySet.Count; j++)
                {
                    Inventor.Property property = propertySet[j];

                    result.Add(new PropertyInfo
                    {
                        Name = property.Name,
                        Value = property.Value?.ToString() ?? string.Empty,
                        Category = propertySet.Name
                    });
                }
            }

            return result;
        }
    }
}