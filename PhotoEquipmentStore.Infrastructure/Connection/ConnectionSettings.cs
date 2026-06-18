using System.Text.Json.Serialization;

namespace PhotoEquipmentStore.Infrastructure.Connection;

/// <summary>
/// Модель настроек подключения к базе данных, десериализуемая из JSON-файла.
/// </summary>
public class ConnectionSettings
{
    /// <summary>
    /// Адрес хоста сервера базы данных.
    /// </summary>
    [JsonPropertyName("host")]
    public string Host { get; set; }

    /// <summary>
    /// Имя пользователя для подключения.
    /// </summary>
    [JsonPropertyName("user")]
    public string User { get; set; }

    /// <summary>
    /// Пароль для подключения.
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; }

    /// <summary>
    /// Имя базы данных.
    /// </summary>
    [JsonPropertyName("database")]
    public string Database { get; set; }

    /// <summary>
    /// Формирует строку подключения в формате MySQL Connector.
    /// </summary>
    public override string ToString() =>
        $"host={Host};uid={User};pwd={Password};database={Database}";
}
