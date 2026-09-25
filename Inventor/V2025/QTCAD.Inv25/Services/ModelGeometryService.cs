using Inventor;
using QTCAD.API.Drawing;
using QTCAD.API.Geometry;
using QTCAD.Inv25.Adapter;
using QTCAD.Inv25.Geometry;
using QTCAD.Inv25.Geometry.FeatureRecognition;
using QTCAD.Inv25.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QTCADHoleFeature = QTCAD.Inv25.Geometry.FeatureRecognition.HoleFeature;

namespace QTCAD.Inv25.Services
{
    internal sealed class ModelGeometryService : IGeometryService
    {
        private readonly InventorContext _context;
        private readonly GeometryExtractor _geometryExtractor;
        private readonly FaceAnalyzer _faceAnalyzer;
        private readonly ViewCandidateAnalyzer _viewCandidateAnalyzer;


        public ModelGeometryService(InventorContext context, GeometryExtractor geometryExtractor, FaceAnalyzer faceAnalyzer, ViewCandidateAnalyzer viewCandidateAnalyzer)
        {
            _context = context;
            _geometryExtractor = geometryExtractor;
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
            UnitsOfMeasure unitsOfMeasure = document.UnitsOfMeasure;
            GeometryExtractionResult geometry = _geometryExtractor.Extract(definition, unitsOfMeasure);

            List<string> interiorResults = new List<string>();

            foreach (SurfaceBody body in definition.SurfaceBodies)
            {
                foreach (Face face in body.Faces)
                {
                    if (face.SurfaceType != SurfaceTypeEnum.kCylinderSurface) continue;

                    Cylinder cylinder = (Cylinder)face.Geometry;
                    bool isInterior = _faceAnalyzer.IsInteriorCylindricalFace(face);
                    interiorResults.Add($"Radius={cylinder.Radius:F3} | Interior={isInterior}");
                }
            }
            HoleDetector holeDetector = new HoleDetector();
            
            IReadOnlyList<QTCADHoleFeature> holes = holeDetector.Detect(geometry);
            string holeResult = string.Join(
                System.Environment.NewLine,
                holes.Select(x =>
                {
                    double diameter = ConversionTool.ToModelLength(x.Radius * 2,unitsOfMeasure);

                    double depth = ConversionTool.ToModelLength(x.Depth,unitsOfMeasure);
                    string unit = ConversionTool.GetUnitSymbol(unitsOfMeasure);
                    return $"Hole | Face={x.CylindricalFaceIndex} | Diameter={diameter:F3} {unit} | Depth={depth:F3} {unit} | Edges=[{string.Join(", ", x.BoundaryEdgeIndices)}] | AdjacentFaces=[{string.Join(", ", x.AdjacentFaceIndices)}]";
                })); 
            MessageBox.Show(holeResult.Length > 0 ? holeResult : "No Hole detected", "Hole Detection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Box? rangeBox = definition.RangeBox;
            if (rangeBox == null) return null;

            return GetBasicGeometry(unitsOfMeasure, rangeBox, definition.SurfaceBodies.Count, 0, geometry.Edges.Count, geometry.Faces.Count, geometry.Faces, geometry.Edges);
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

            string unitSymbol = ConversionTool.GetUnitSymbol(unitsOfMeasure);
            double xSize = rangeBox.MaxPoint.X - rangeBox.MinPoint.X;
            double ySize = rangeBox.MaxPoint.Y - rangeBox.MinPoint.Y;
            double zSize = rangeBox.MaxPoint.Z - rangeBox.MinPoint.Z;
            return new ModelGeometryInfo
            {
                XSize = ConversionTool.ToModelLength(xSize, unitsOfMeasure),
                YSize = ConversionTool.ToModelLength(ySize, unitsOfMeasure),
                ZSize = ConversionTool.ToModelLength(zSize, unitsOfMeasure),
                Unit = unitSymbol,
                BodyCount = bodyCount,
                EdgeCount = edgeCount,
                OccurrenceCount = occurrenceCount,
                FaceCount = faceCount,
                Faces = faces ?? [],
                Edges = edges
            };
        }

    }
}