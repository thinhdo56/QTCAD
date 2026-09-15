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
        private readonly FaceAnalyzer _faceAnalyzer;
        private readonly ViewCandidateAnalyzer _viewCandidateAnalyzer;



        public ModelGeometryService(InventorContext context, FaceAnalyzer faceAnalyzer, ViewCandidateAnalyzer viewCandidateAnalyzer)
        {
            _context = context;
            _faceAnalyzer = faceAnalyzer;
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
            List<FaceInfo> faces = new List<FaceInfo>();
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
            ModelGeometryInfo geometry = GetBasicGeometry(document.UnitsOfMeasure, rangeBox, definition.SurfaceBodies.Count, 0, faces.Count, faces);
            IReadOnlyList<ViewCandidate> candidates = _viewCandidateAnalyzer.Analyze(geometry);
            string result = string.Join("\n", candidates.Select(x => $"{x.ViewType}: Score={x.Score:F3}, VisibleFaces={x.VisibleFaces}, HiddenFaces={x.HiddenFaces}, VisibleArea={x.VisibleArea:F2}"));
            MessageBox.Show(result, "View Candidate Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private ModelGeometryInfo GetBasicGeometry(UnitsOfMeasure DocUnits, Box rangeBox,int bodyCount = 0, int occurrenceCount = 0, int faceCount=0, IReadOnlyList<FaceInfo>? faces = null)
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