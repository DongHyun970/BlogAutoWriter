using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BlogAutoWriter.Services
{
    public class UserSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string KakaoId { get; set; } = string.Empty;
        public string KakaoPassword { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public bool UseImage { get; set; } = false;
    }

    public static class SettingsManager
    {
        private static readonly string SettingsFile = "settings.json";

        public static void Save(UserSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
        }

        public static UserSettings Load()
        {
            if (!File.Exists(SettingsFile)) return new UserSettings();
            var json = File.ReadAllText(SettingsFile);
            return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
        }
    }
}