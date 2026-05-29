using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Connection;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Helpers;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class Authorization
{
    private static readonly string connString = ConnectionSettingsParser.Load().ToString();

    private static MySqlConnection GetConnection()
    {
        return new MySqlConnection(connString);
    }

    public static UserAuth? GetUser(string login)
    {
        using var conn = GetConnection();
        conn.Open();
        const string query = @"SELECT u.id, 
                                      u.full_name, 
                                      u.login, 
                                      u.password_hash, 
                                      u.image, 
                                      r.id, 
                                      r.name 
                               FROM users AS u
                               INNER JOIN roles AS r ON r.id = u.role_id
                               WHERE u.login = @login and u.is_deleted != 1;";

        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@login", login);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return new UserAuth(
            id:            reader.GetInt32("id"),
            name:          reader.GetString("full_name"),
            login:         reader.GetString("login"),
            heshPassword:  reader.GetString("password_hash"),
            userImage:     BlobReader.ToBytes(reader, "image"),
            roleId:        reader.GetInt32("id"),
            roleName:      reader.GetString("name"),
            timeOfLogout:  3
        );
    }
}