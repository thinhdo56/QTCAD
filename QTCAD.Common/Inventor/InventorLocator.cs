using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace QTCAD.Common.Inventor
{
    public static class InventorLocator
    {
        private const string BaseRegistryPath = @"SOFTWARE\Autodesk\Inventor";
        private static readonly Dictionary<int, string> RegistryVersions = new()
        {
            [2023] = "RegistryVersion27.0",
            [2024] = "RegistryVersion28.0",
            [2025] = "RegistryVersion29.0",
            [2026] = "RegistryVersion30.0"
        };
        public static InventorInstallation? Find(int version)
        {
            if (!RegistryVersions.TryGetValue(version, out string? registryVersion))
                return null;

            string registryPath = $@"{BaseRegistryPath}\{registryVersion}\TaskScheduler\Installation Path";

            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(registryPath);

            if (key == null)
                return null;

            string? binPath = key.GetValue("Path")?.ToString();

            if (string.IsNullOrWhiteSpace(binPath))
                return null;

            binPath = binPath.TrimEnd('\\');

            string executablePath = Path.Combine(binPath, "Inventor.exe");

            if (!File.Exists(executablePath))
                return null;

            string installationPath = Directory.GetParent(binPath)?.FullName ?? binPath;

            return new InventorInstallation
            {
                Version = version,
                RegistryVersion = registryVersion,
                InstallationPath = installationPath,
                ExecutablePath = executablePath,
                BinPath = binPath
            };
        }

        public static bool IsInstalled(int version)
        {
            return Find(version) != null;
        }
    }
}
