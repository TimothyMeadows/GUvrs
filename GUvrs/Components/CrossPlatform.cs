using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GUvrs.Components;

public static class CrossPlatform
{
    public static void Open(string path)
    {
        var fileManager = "explorer";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            fileManager = "open";

        var startInfo = new ProcessStartInfo
        {
            FileName = fileManager,
            Arguments = path,
            UseShellExecute = false
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
    }

    public static class FileSystem
    {
        public static string AppDataDirectory => Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
        public static string GuLogPath = string.Empty;

        public static void CreateIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static bool Exists(string path)
        {
            return File.Exists($"{AppDataDirectory}\\{path}");
        }

        public static void WriteText(string path, string content)
        {
            File.WriteAllText($"{AppDataDirectory}\\{path}", content);
        }

        public static string ReadText(string path)
        {
            return File.ReadAllText($"{AppDataDirectory}\\{path}");
        }
    }
}