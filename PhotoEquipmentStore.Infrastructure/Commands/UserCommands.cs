using System.Collections.ObjectModel;
using System.Data;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Helpers;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class UserCommands
{
    private static readonly string connString = ConnectionSettingsParser.Load().ToString();
    
    public static ObservableCollection<User> GetUsers()
    {
        var users = new ObservableCollection<User>();
        string query = @"
            SELECT 
                u.id,
                u.full_name AS name,
                u.login,
                u.phone AS phoneNumber,
                r.name AS role,
                u.role_id AS roleID,
                u.image
            FROM users u
            JOIN roles r ON u.role_id = r.id
            WHERE u.is_deleted = 0;";

        using var connection = new MySqlConnection(connString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            string login = reader.GetString("login");
            string phone = reader.GetString("phoneNumber");
            string role = reader.GetString("role");
            int roleID = reader.GetInt32("roleID");
            byte[]? image = BlobReader.ToBytes(reader, "image");

            users.Add(new User(id, name, login, phone, role, roleID, image));
        }

        return users;
    }
}