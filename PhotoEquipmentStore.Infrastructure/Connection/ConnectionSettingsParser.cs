using System;
using System.IO;
using System.Text.Json;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Infrastructure.Connection;

/// <summary>
/// Загрузка и валидация настроек подключения к базе данных из JSON-файла.
/// </summary>
public static class ConnectionSettingsParser
{
    private static readonly string DefaultPath =
        Directory.GetCurrentDirectory() + "/Connection/connectionSettings.json";

    /// <summary>
    /// Читает, десериализует и проверяет настройки подключения из файла конфигурации.
    /// </summary>
    public static ConnectionSettings Load()
    {
        try
        {
            if (!File.Exists(DefaultPath))
                throw new FileNotFoundException($"Файл настроек подключения не найден: {DefaultPath}");

            string json = File.ReadAllText(DefaultPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas         = true,
                ReadCommentHandling         = JsonCommentHandling.Skip
            };

            var settings = JsonSerializer.Deserialize<ConnectionSettings>(json, options)
                           ?? throw new InvalidOperationException("Не удалось десериализовать файл настроек.");

            Validate(settings);
            return settings;
        }
        catch (FileNotFoundException ex)
        {
            throw new DatabaseException("Файл настроек подключения к базе данных не найден.", ex);
        }
        catch (JsonException ex)
        {
            throw new DatabaseException("Ошибка чтения файла настроек: неверный формат JSON.", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new DatabaseException("Ошибка инициализации настроек подключения.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при загрузке настроек подключения.", ex);
        }
    }

    /// <summary>
    /// Проверяет наличие обязательных полей в настройках подключения.
    /// </summary>
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
