using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Connection;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Helpers;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class Authorization
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    private static MySqlConnection GetConnection() => new MySqlConnection(ConnString);

    public static UserAuth? GetUser(string login)
    {
        try
        {
            using var conn = GetConnection();
            conn.Open();

            const string query = @"
                SELECT u.id,
                       u.full_name,
                       u.login,
                       u.password_hash,
                       u.image,
                       r.id   AS roleId,
                       r.name AS roleName
                FROM users AS u
                INNER JOIN roles AS r ON r.id = u.role_id
                WHERE u.login = @login
                  AND u.is_deleted != 1;";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new UserAuth(
                id:           reader.GetInt32("id"),
                name:         reader.GetString("full_name"),
                login:        reader.GetString("login"),
                heshPassword: reader.GetString("password_hash"),
                userImage:    BlobReader.ToBytes(reader, "image"),
                roleId:       reader.GetInt32("roleId"),   // ← исправлен баг
                roleName:     reader.GetString("roleName"),
                timeOfLogout: 3
            );
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении пользователя для авторизации.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при авторизации.", ex);
        }
    }
}