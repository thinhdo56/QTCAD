using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Utilities
{
    public static class ConversionTool
    {
        public static double ToModelLength(double value, UnitsOfMeasure unitsOfMeasure)
        {
            return unitsOfMeasure.ConvertUnits(value,UnitsTypeEnum.kDatabaseLengthUnits,unitsOfMeasure.LengthUnits);
        }
        public static double FromModelLength(double value, UnitsOfMeasure unitsOfMeasure, UnitsTypeEnum sourceUnits)
        {
            return unitsOfMeasure.ConvertUnits(value,sourceUnits,UnitsTypeEnum.kDatabaseLengthUnits);
        }
        public static string GetUnitSymbol(UnitsOfMeasure unitsOfMeasure)
        {
            string unitName = unitsOfMeasure.GetStringFromType(unitsOfMeasure.LengthUnits);
            return unitName.ToLowerInvariant() switch { "millimeter" => "mm", "centimeter" => "cm", "meter" => "m", "inch" => "in", "foot" => "ft", _ => unitName };
        }
    }
}
