using Inventor;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace QTCAD.Inv25.Services
{
    internal sealed class ModelGeometryService : IGeometryService
    {
        private readonly InventorContext _context;
        private readonly FaceAnalyzer _faceAnalyzer;
        public ModelGeometryService(InventorContext context, FaceAnalyzer faceAnalyzer)
        {
            _context = context;
            _faceAnalyzer = faceAnalyzer;
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
            List<FaceGeometryInfo> faces = new List<FaceGeometryInfo>();
            int index = 0;
            foreach (SurfaceBody body in definition.SurfaceBodies)
            {
                foreach (Face face in body.Faces)
                {
                    faces.Add(_faceAnalyzer.Analyze(face, index++));
                }
            }
            Box? rangeBox = definition.RangeBox;

            if (rangeBox == null)
            {
                return null;
            }
            string faceInfo = string.Join("\n", faces.Select(f => $"Face {f.Index}: Area={f.Area:F2}, Center=({f.CenterX:F2}, {f.CenterY:F2}, {f.CenterZ:F2}), Normal=({f.NormalX:F4}, {f.NormalY:F4}, {f.NormalZ:F4}), Type={f.Type}"));
            MessageBox.Show(faceInfo, "Face Analyzer Test", MessageBoxButtons.OK, MessageBoxIcon.Information );
            return GetBasicGeometry(document.UnitsOfMeasure, rangeBox, definition.SurfaceBodies.Count, 0, faces.Count, faces);
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

            return GetBasicGeometry(document.UnitsOfMeasure, rangeBox);
        }
        private ModelGeometryInfo? AnalyzeAssembly(AssemblyDocument document)
        {
            AssemblyComponentDefinition definition = document.ComponentDefinition;
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null) return null;

            return GetBasicGeometry(document.UnitsOfMeasure, rangeBox);
        }
        private ModelGeometryInfo? AnalyzeWeldment(AssemblyDocument document)
        {
            AssemblyComponentDefinition definition = document.ComponentDefinition;
            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null) return null;


            return GetBasicGeometry(document.UnitsOfMeasure, rangeBox);
        }

        private ModelGeometryInfo GetBasicGeometry(UnitsOfMeasure DocUnits, Box rangeBox,int bodyCount = 0, int occurrenceCount = 0, int faceCount=0, IReadOnlyList<FaceGeometryInfo>? faces = null)
        {

            string unitSymbol = GetUnitSymbol(DocUnits.GetStringFromType(DocUnits.LengthUnits));
            double xSize = rangeBox.MaxPoint.X - rangeBox.MinPoint.X;
            double ySize = rangeBox.MaxPoint.Y - rangeBox.MinPoint.Y;
            double zSize = rangeBox.MaxPoint.Z - rangeBox.MinPoint.Z;
            return new ModelGeometryInfo
            {
                XSize = DocUnits.ConvertUnits(xSize, UnitsTypeEnum.kDatabaseLengthUnits, DocUnits.LengthUnits),
                YSize = DocUnits.ConvertUnits(ySize, UnitsTypeEnum.kDatabaseLengthUnits, DocUnits.LengthUnits),
                ZSize = DocUnits.ConvertUnits(zSize, UnitsTypeEnum.kDatabaseLengthUnits, DocUnits.LengthUnits),
                Unit = unitSymbol,
                BodyCount = bodyCount,
                OccurrenceCount = occurrenceCount,
                FaceCount = faceCount,
                Faces = faces ?? []
            };
        }
        private string GetUnitSymbol(string unitName)
        {
            return unitName.ToLowerInvariant() switch { "millimeter" => "mm", "centimeter" => "cm", "meter" => "m", "inch" => "in", "foot" => "ft", _ => unitName };
        }
    }
}