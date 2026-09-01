using Inventor;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Adapter;
using System;
using System.Collections.Generic;

namespace QTCAD.Inv25.Services
{
    internal sealed class ModelGeometryService : IGeometryService
    {
        private readonly InventorContext _context;

        public ModelGeometryService(InventorContext context)
        {
            _context = context;
        }

        public ModelGeometryInfo? Analyze()
        {
            Document? document = _context.ActiveDocument;

            if (document is PartDocument partDocument)
            {
                var result = AnalyzePartDocument(partDocument);
                return result;
            }

            if (document is AssemblyDocument assemblyDocument)
            {
                var result = AnalyzeAssemblyDocument(assemblyDocument);
                return result;
            }

            return null;
        }
        private ModelGeometryInfo? AnalyzePartDocument(PartDocument document)
        {
            if (document.ComponentDefinition is SheetMetalComponentDefinition)

            {
                return AnalyzeSheetMetal(document);
            }
            return AnalyzePart(document);
        }
        private ModelGeometryInfo? AnalyzeAssemblyDocument(AssemblyDocument document)
        {
            if (document.ComponentDefinition is WeldmentComponentDefinition)

            {
                return AnalyzeWeldment(document);
            }

            return AnalyzeAssembly(document);
        }
        private ModelGeometryInfo? AnalyzePart(PartDocument document)
        {
            PartComponentDefinition definition = document.ComponentDefinition;
            Box? rangeBox = definition.RangeBox;

            if (rangeBox == null)
            {
                return null;
            }

            UnitsTypeEnum docUnits = document.UnitsOfMeasure.LengthUnits;
            string unitSymbol = GetUnitSymbol(document.UnitsOfMeasure.GetStringFromType(docUnits));
            double xSize = rangeBox.MaxPoint.X - rangeBox.MinPoint.X;
            double ySize = rangeBox.MaxPoint.Y - rangeBox.MinPoint.Y;
            double zSize = rangeBox.MaxPoint.Z - rangeBox.MinPoint.Z;

            return new ModelGeometryInfo
            {
                XSize = document.UnitsOfMeasure.ConvertUnits(xSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                YSize = document.UnitsOfMeasure.ConvertUnits(ySize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                ZSize = document.UnitsOfMeasure.ConvertUnits(zSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                BodyCount = definition.SurfaceBodies.Count,
                Unit = unitSymbol
            };
        }
        private ModelGeometryInfo? AnalyzeSheetMetal(PartDocument document)
        {
            PartComponentDefinition definition = document.ComponentDefinition;
            Box? rangeBox = definition.RangeBox;

            if (rangeBox == null)
            {
                return null;
            }

            UnitsTypeEnum docUnits = document.UnitsOfMeasure.LengthUnits;
            string unitSymbol = GetUnitSymbol(document.UnitsOfMeasure.GetStringFromType(docUnits));
            double xSize = rangeBox.MaxPoint.X - rangeBox.MinPoint.X;
            double ySize = rangeBox.MaxPoint.Y - rangeBox.MinPoint.Y;
            double zSize = rangeBox.MaxPoint.Z - rangeBox.MinPoint.Z;

            return new ModelGeometryInfo
            {
                XSize = document.UnitsOfMeasure.ConvertUnits(xSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                YSize = document.UnitsOfMeasure.ConvertUnits(ySize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                ZSize = document.UnitsOfMeasure.ConvertUnits(zSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                BodyCount = definition.SurfaceBodies.Count,
                Unit = unitSymbol
            };
        }
        private ModelGeometryInfo? AnalyzeAssembly(AssemblyDocument document)
        {
            AssemblyComponentDefinition definition = document.ComponentDefinition;
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null) return null;

            UnitsTypeEnum docUnits = document.UnitsOfMeasure.LengthUnits;
            string unitSymbol = GetUnitSymbol(document.UnitsOfMeasure.GetStringFromType(docUnits));
            double xSize = rangeBox.MaxPoint.X - rangeBox.MinPoint.X;
            double ySize = rangeBox.MaxPoint.Y - rangeBox.MinPoint.Y;
            double zSize = rangeBox.MaxPoint.Z - rangeBox.MinPoint.Z;

            return new ModelGeometryInfo
            {
                XSize = document.UnitsOfMeasure.ConvertUnits(xSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                YSize = document.UnitsOfMeasure.ConvertUnits(ySize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                ZSize = document.UnitsOfMeasure.ConvertUnits(zSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                Unit = unitSymbol,
                OccurrenceCount = definition.Occurrences.Count
            };
        }
        private ModelGeometryInfo? AnalyzeWeldment(AssemblyDocument document)
        {
            AssemblyComponentDefinition definition = document.ComponentDefinition;
            ObjectTypeEnum type = definition.Type;
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null) return null;

            UnitsTypeEnum docUnits = document.UnitsOfMeasure.LengthUnits;
            string unitSymbol = GetUnitSymbol(document.UnitsOfMeasure.GetStringFromType(docUnits));
            double xSize = rangeBox.MaxPoint.X - rangeBox.MinPoint.X;
            double ySize = rangeBox.MaxPoint.Y - rangeBox.MinPoint.Y;
            double zSize = rangeBox.MaxPoint.Z - rangeBox.MinPoint.Z;

            return new ModelGeometryInfo
            {
                XSize = document.UnitsOfMeasure.ConvertUnits(xSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                YSize = document.UnitsOfMeasure.ConvertUnits(ySize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                ZSize = document.UnitsOfMeasure.ConvertUnits(zSize, UnitsTypeEnum.kDatabaseLengthUnits, docUnits),
                Unit = unitSymbol,
                OccurrenceCount = definition.Occurrences.Count
            };
        }
        private string GetUnitSymbol(string unitName)
        {
            return unitName.ToLowerInvariant() switch { "millimeter" => "mm", "centimeter" => "cm", "meter" => "m", "inch" => "in", "foot" => "ft", _ => unitName };
        }
    }
}