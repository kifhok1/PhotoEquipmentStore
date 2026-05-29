using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class Login
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    public List<string> GetAllLogins()
    {
        var logins = new List<string>();

        string query = "SELECT login FROM users;";  // или с WHERE is_deleted = 0

        using var connection = new MySqlConnection(ConnString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            logins.Add(reader.GetString("login"));
        }

        return logins;
    }
}