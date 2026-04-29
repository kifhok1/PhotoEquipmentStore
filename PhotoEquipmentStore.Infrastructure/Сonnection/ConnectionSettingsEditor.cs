using System.Text.Json;

namespace PhotoEquipmentStore.Infrastructure.Сonnection;

public static class ConnectionSettingsEditor
{
    private static readonly string DefaultPath = Directory.GetCurrentDirectory() + "connectionSettings.json";

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public static void Update(
        string host,
        string user,
        string password,
        string database)
    {
        if (!File.Exists(DefaultPath))
            throw new FileNotFoundException($"Файл не найден: {DefaultPath}");

        string json = File.ReadAllText(DefaultPath);
        var settings = JsonSerializer.Deserialize<ConnectionSettings>(json, ReadOptions)
                       ?? throw new InvalidOperationException("Не удалось прочитать настройки.");

        settings.Host     = host;
        settings.User     = user;
        settings.Password = password;
        settings.Database = database;
        
        File.WriteAllText(DefaultPath, JsonSerializer.Serialize(settings, WriteOptions));
    }
    
}