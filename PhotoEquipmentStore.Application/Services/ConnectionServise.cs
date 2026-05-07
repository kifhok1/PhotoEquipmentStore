using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Сonnection;

namespace PhotoEquipmentStore.Application.Services;

public class ConnectionServise
{
    public static void SaveConnection(ConnectionToDBSettings connectionSettings)
    {
        ConnectionSettingsEditor.Update(
            connectionSettings.Host,
            connectionSettings.User,
            connectionSettings.Password,
            connectionSettings.Database);
    }

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