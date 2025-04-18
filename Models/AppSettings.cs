using System;
using System.IO;
using System.Text.Json;

namespace BlogAutoWriter.Models
{
    public class AppSettings
    {
        public string OpenAiApiKey { get; set; } = "";
        public string KakaoEmail { get; set; } = "";
        public string KakaoPassword { get; set; } = "";
        public string TistoryEmail { get; set; } = "";
        public string TistoryPassword { get; set; } = "";
        public string[] Categories { get; set; } = Array.Empty<string>();

        private static readonly string _filePath = Path.Combine(Environment.CurrentDirectory, "settings.json");

        public static AppSettings Current { get; set; } = new();

        public static void Load()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                    Current = loaded;
            }
        }

        public static void Save()
        {
            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
