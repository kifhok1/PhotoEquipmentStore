using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Сonnection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class Phone
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();
    
    public List<string> GetAllPhoneNumbers()
    {
        var phones = new List<string>();

        string query = @"
        SELECT phone FROM users WHERE phone IS NOT NULL AND phone != ''
        UNION ALL
        SELECT phone FROM clients WHERE phone IS NOT NULL AND phone != '';";

        using var connection = new MySqlConnection(ConnString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            phones.Add(reader.GetString("phone"));
        }

        return phones;
    }
}