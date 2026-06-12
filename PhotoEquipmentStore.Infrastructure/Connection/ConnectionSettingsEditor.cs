using System;
using System.IO;
using System.Text.Json;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Infrastructure.Connection;

public static class ConnectionSettingsEditor
{
    private static readonly string DefaultPath =
        Directory.GetCurrentDirectory() + "/Connection/connectionSettings.json";

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented        = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas         = true,
        ReadCommentHandling         = JsonCommentHandling.Skip
    };

    public static void Update(string host, string user, string password, string database)
    {
        try
        {
            if (!File.Exists(DefaultPath))
                throw new FileNotFoundException($"Файл настроек не найден: {DefaultPath}");

            string json     = File.ReadAllText(DefaultPath);
            var    settings = JsonSerializer.Deserialize<ConnectionSettings>(json, ReadOptions)
                              ?? throw new InvalidOperationException("Не удалось прочитать настройки.");

            settings.Host     = host;
            settings.User     = user;
            settings.Password = password;
            settings.Database = database;

            File.WriteAllText(DefaultPath, JsonSerializer.Serialize(settings, WriteOptions));
        }
        catch (FileNotFoundException ex)
        {
            throw new DatabaseException("Файл настроек подключения не найден.", ex);
        }
        catch (JsonException ex)
        {
            throw new DatabaseException("Ошибка чтения файла настроек: неверный формат JSON.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new DatabaseException("Нет прав на запись файла настроек подключения.", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new DatabaseException("Ошибка обновления настроек подключения.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при обновлении настроек подключения.", ex);
        }
    }
}