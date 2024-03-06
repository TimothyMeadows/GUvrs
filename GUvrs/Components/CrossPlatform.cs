namespace GUvrs.Components;

public static class CrossPlatform
{
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