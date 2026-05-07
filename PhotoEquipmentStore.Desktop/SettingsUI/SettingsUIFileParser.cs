using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PhotoEquipmentStore.SettingsUI;

public class SettingsUIFileParser
{
    private static readonly string _filePath = Directory.GetCurrentDirectory() + "/SettingsUI/settings.json";

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    
    public static string? GetTheme()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"File not found: {_filePath}");

        string json = File.ReadAllText(_filePath);
        JsonNode? root = JsonNode.Parse(json);

        return root?["theme"]?.GetValue<string>();
    }
    
    public static void SetTheme(string theme)
    {
        if (theme == null) throw new ArgumentNullException(nameof(theme));

        JsonObject root;

        if (File.Exists(_filePath))
        {
            string existingJson = File.ReadAllText(_filePath);
            root = JsonNode.Parse(existingJson)?.AsObject() ?? new JsonObject();
        }
        else
        {
            root = new JsonObject();
        }

        root["theme"] = theme;

        string updatedJson = root.ToJsonString(_jsonOptions);
        File.WriteAllText(_filePath, updatedJson);
    }
}