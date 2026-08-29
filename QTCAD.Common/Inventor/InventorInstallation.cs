using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Common.Inventor
{
    public sealed class InventorInstallation
    {
        public int Version { get; init; }
        public string RegistryVersion { get; init; } = string.Empty;
        public string InstallationPath { get; init; } = string.Empty;
        public string ExecutablePath { get; init; } = string.Empty;
        public string BinPath { get; init; } = string.Empty;
    }
}
