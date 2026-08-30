using System;
using Inventor;

namespace QTCAD.Inv25.UI
{
    internal static class RibbonEnvironmentExtensions
    {
        public static string GetRibbonName(this RibbonEnvironment environment)
        {
            return environment switch
            {
                RibbonEnvironment.Part => "Part",
                RibbonEnvironment.Assembly => "Assembly",
                RibbonEnvironment.Drawing => "Drawing",
                _ => throw new ArgumentOutOfRangeException(nameof(environment))
            };
        }
    }
}
