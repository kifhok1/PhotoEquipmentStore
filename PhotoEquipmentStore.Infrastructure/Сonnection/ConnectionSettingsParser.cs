using System.Text.Json;

namespace PhotoEquipmentStore.Infrastructure.Сonnection;

public static class ConnectionSettingsParser
{
    private static readonly string DefaultPath = Directory.GetCurrentDirectory() + "connectionSettings.json";

    public static ConnectionSettings Load()
    {
        string json = File.ReadAllText(DefaultPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        var settings = JsonSerializer.Deserialize<ConnectionSettings>(json, options)
                       ?? throw new InvalidOperationException("Не удалось десериализовать файл настроек.");

        Validate(settings);
        return settings;
    }

    private static void Validate(ConnectionSettings s)
    {
        if (string.IsNullOrWhiteSpace(s.Host))
            throw new InvalidOperationException("Поле 'host' не заполнено.");
        if (string.IsNullOrWhiteSpace(s.User))
            throw new InvalidOperationException("Поле 'user' не заполнено.");
        if (string.IsNullOrWhiteSpace(s.Database))
            throw new InvalidOperationException("Поле 'database' не заполнено.");
    }
}