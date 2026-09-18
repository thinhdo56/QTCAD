using Inventor;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QTCAD.API.Drawing;

namespace QTCAD.Inv25.Services
{
    internal sealed class ModelGeometryService : IGeometryService
    {
        private readonly InventorContext _context;
        private readonly GeometryExtractor _geometryExtractor;
        private readonly ViewCandidateAnalyzer _viewCandidateAnalyzer;



        public ModelGeometryService(InventorContext context, GeometryExtractor geometryExtractor, ViewCandidateAnalyzer viewCandidateAnalyzer)
        {
            _context = context;
            _geometryExtractor = geometryExtractor;
            _viewCandidateAnalyzer = viewCandidateAnalyzer;
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
            GeometryExtractionResult geometry = _geometryExtractor.Extract( definition, document.UnitsOfMeasure);
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null)
            {
                return null;
            }
            return GetBasicGeometry(
                document.UnitsOfMeasure,
                rangeBox,
                definition.SurfaceBodies.Count,
                0,
                geometry.Edges.Count,
                geometry.Faces.Count,
                geometry.Faces,
                geometry.Edges);
        }
        private ModelGeometryInfo? AnalyzeSheetMetal(PartDocument document)
        {
            SheetMetalComponentDefinition definition = (SheetMetalComponentDefinition)document.ComponentDefinition;
            bool hasFlatPattern = definition.HasFlatPattern;
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null)
            {
                return null;
            }
            return GetBasicGeometry(document.UnitsOfMeasure, rangeBox, 0, 0, 0, 0, [], []);
        }
        private ModelGeometryInfo? AnalyzeAssembly(AssemblyDocument document)
        {
            AssemblyComponentDefinition definition = document.ComponentDefinition;
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null) return null;

            return GetBasicGeometry(document.UnitsOfMeasure, rangeBox,0,0,0,0, [], []);
        }
        private ModelGeometryInfo? AnalyzeWeldment(AssemblyDocument document)
        {
            AssemblyComponentDefinition definition = document.ComponentDefinition;
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null) return null;


            return GetBasicGeometry(document.UnitsOfMeasure, rangeBox, 0, 0, 0, 0, [], []);
        }

        private ModelGeometryInfo GetBasicGeometry(UnitsOfMeasure unitsOfMeasure, Box rangeBox, int bodyCount, int occurrenceCount, int edgeCount, int faceCount, IReadOnlyList<FaceInfo> faces, IReadOnlyList<EdgeInfo> edges)
        {

            string unitSymbol = GetUnitSymbol(unitsOfMeasure.GetStringFromType(unitsOfMeasure.LengthUnits));
            double xSize = rangeBox.MaxPoint.X - rangeBox.MinPoint.X;
            double ySize = rangeBox.MaxPoint.Y - rangeBox.MinPoint.Y;
            double zSize = rangeBox.MaxPoint.Z - rangeBox.MinPoint.Z;
            return new ModelGeometryInfo
            {
                XSize = unitsOfMeasure.ConvertUnits(xSize, UnitsTypeEnum.kDatabaseLengthUnits, unitsOfMeasure.LengthUnits),
                YSize = unitsOfMeasure.ConvertUnits(ySize, UnitsTypeEnum.kDatabaseLengthUnits, unitsOfMeasure.LengthUnits),
                ZSize = unitsOfMeasure.ConvertUnits(zSize, UnitsTypeEnum.kDatabaseLengthUnits, unitsOfMeasure.LengthUnits),
                Unit = unitSymbol,
                BodyCount = bodyCount,
                EdgeCount = edgeCount,
                OccurrenceCount = occurrenceCount,
                FaceCount = faceCount,
                Faces = faces ?? [],
                Edges = edges
            };
        }
        private string GetUnitSymbol(string unitName)
        {
            return unitName.ToLowerInvariant() switch { "millimeter" => "mm", "centimeter" => "cm", "meter" => "m", "inch" => "in", "foot" => "ft", _ => unitName };
        }
    }
}