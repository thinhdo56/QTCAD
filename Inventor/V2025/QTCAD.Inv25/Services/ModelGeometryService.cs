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
        private readonly GeometryExtractor _geometryExtractor;
        private readonly ViewCandidateAnalyzer _viewCandidateAnalyzer;



        public ModelGeometryService(InventorContext context, FaceAnalyzer faceAnalyzer, GeometryExtractor geometryExtractor, ViewCandidateAnalyzer viewCandidateAnalyzer)
        {
            _context = context;
            _faceAnalyzer = faceAnalyzer;
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
            GeometryExtractionResult geometry = _geometryExtractor.Extract(definition, document.UnitsOfMeasure);
            Box? rangeBox = definition.RangeBox;

            if (rangeBox == null)
            {
                return null;
            }
            ModelGeometryInfo geometryInfo = GetBasicGeometry(document.UnitsOfMeasure, rangeBox, definition.SurfaceBodies.Count, 0, geometry.Edges.Count, geometry.Faces.Count, geometry.Faces, geometry.Edges);

            string edgeResult = string.Join(System.Environment.NewLine, geometryInfo.Edges.Select(x => $"Edge {x.Index} | Type={x.CurveType} | Length={x.Length:F3} | Circular={x.IsCircular} | Linear={x.IsLinear}"));
            MessageBox.Show(edgeResult.Length > 0 ? edgeResult : "No Edge detected", "Edge Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string cylinders = string.Join("\n", geometry.Faces.Where(x => x.Radius > 0.0).Select(x => $"Face {x.Index}: Type={x.Type}, Radius={x.Radius:F3}, Axis=({x.AxisX:F3}, {x.AxisY:F3}, {x.AxisZ:F3})"));
            MessageBox.Show(cylinders.Length > 0 ? cylinders : "No Cylinder detected", "Cylinder Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string faceEdges = string.Join(System.Environment.NewLine, geometryInfo.Faces.Select(x => $"Face {x.Index} | Type={x.Type} | Edges=[{string.Join(", ", x.EdgeIndices)}]"));
            MessageBox.Show(faceEdges, "Face Edge Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string edgeFacesResult = string.Join(System.Environment.NewLine, geometry.Edges.Select(x => $"Edge {x.Index} | Faces=[{string.Join(", ", x.AdjacentFaceIndices)}]"));
            MessageBox.Show(edgeFacesResult, "Edge Adjacent Faces Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

            IReadOnlyList<ViewCandidate> candidates = _viewCandidateAnalyzer.Analyze(geometryInfo);
            string result = string.Join(System.Environment.NewLine, candidates.Select((x, i) => $"{i + 1}. {x.ViewType} | Score={x.Score:F3} | VisibleFaces={x.VisibleFaces} | HiddenFaces={x.HiddenFaces} | CircularFeatures={x.CircularFeatures} | FeatureCount={x.FeatureCount} | VisibleArea={x.VisibleArea:F2}"));
            MessageBox.Show(result, "View Candidate Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return geometryInfo;
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

        private ModelGeometryInfo GetBasicGeometry(UnitsOfMeasure DocUnits, Box rangeBox, int bodyCount, int occurrenceCount, int edgeCount, int faceCount, IReadOnlyList<FaceInfo> faces, IReadOnlyList<EdgeInfo> edges)
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