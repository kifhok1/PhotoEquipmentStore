using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис чтения и сохранения параметров подключения к базе данных.
/// </summary>
public class ConnectionServise
{
    /// <summary>
    /// Сохраняет параметры подключения в конфигурационный файл.
    /// </summary>
    /// <param name="connectionSettings">Параметры подключения.</param>
    public static void SaveConnection(ConnectionToDBSettings connectionSettings)
    {
        ConnectionSettingsEditor.Update(
            connectionSettings.Host,
            connectionSettings.User,
            connectionSettings.Password,
            connectionSettings.Database);
    }

    /// <summary>
    /// Загружает сохранённые параметры подключения из конфигурации.
    /// </summary>
    /// <returns>Текущие параметры подключения к БД.</returns>
    public static ConnectionToDBSettings GetConnectionSettings()
    {
        ConnectionSettings conn = ConnectionSettingsParser.Load();

        return new ConnectionToDBSettings(
            conn.Host,
            conn.User,
            conn.Password,
            conn.Database);
    }
}
