using System;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Helpers;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class UserCommands
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    // ── Read ──────────────────────────────────────────────────────────────────

    public static ObservableCollection<User> GetUsers()
    {
        try
        {
            var users = new ObservableCollection<User>();

            const string query = @"
                SELECT
                    u.id,
                    u.full_name AS name,
                    u.login,
                    u.phone     AS phoneNumber,
                    r.name      AS role,
                    u.role_id   AS roleID,
                    u.image
                FROM users u
                JOIN roles r ON u.role_id = r.id
                WHERE u.is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetString("login"),
                    reader.GetString("phoneNumber"),
                    reader.GetString("role"),
                    reader.GetInt32("roleID"),
                    BlobReader.ToBytes(reader, "image")
                ));
            }

            return users;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении списка пользователей.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении списка пользователей.", ex);
        }
    }

    // ── Create ────────────────────────────────────────────────────────────────

    public bool CreateUser(User user, string passwordHash)
    {
        try
        {
            const string query = @"
                INSERT INTO users (full_name, login, password_hash, phone, role_id, image)
                VALUES (@name, @login, @passwordHash, @phone, @roleId, @image);";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name",         user.Name);
            command.Parameters.AddWithValue("@login",        user.Login);
            command.Parameters.AddWithValue("@passwordHash", passwordHash);
            command.Parameters.AddWithValue("@phone",        user.PhoneNumber);
            command.Parameters.AddWithValue("@roleId",       user.RoleID);
            command.Parameters.AddWithValue("@image",        user.Image ?? (object)DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при создании пользователя.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при создании пользователя.", ex);
        }
    }

    // ── Update ────────────────────────────────────────────────────────────────

    public bool UpdateUser(User user)
    {
        try
        {
            const string query = @"
                UPDATE users
                SET full_name = @name,
                    login     = @login,
                    phone     = @phone,
                    role_id   = @roleId,
                    image     = @image
                WHERE id = @id
                  AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name",   user.Name);
            command.Parameters.AddWithValue("@login",  user.Login);
            command.Parameters.AddWithValue("@phone",  user.PhoneNumber);
            command.Parameters.AddWithValue("@roleId", user.RoleID);
            command.Parameters.AddWithValue("@image",  user.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@id",     user.Id);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при обновлении пользователя.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при обновлении пользователя.", ex);
        }
    }

    public bool UpdateUserWithPassword(User user, string passwordHash)
    {
        try
        {
            const string query = @"
                UPDATE users
                SET full_name     = @name,
                    login         = @login,
                    phone         = @phone,
                    role_id       = @roleId,
                    image         = @image,
                    password_hash = @passwordHash
                WHERE id = @id
                  AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name",         user.Name);
            command.Parameters.AddWithValue("@login",        user.Login);
            command.Parameters.AddWithValue("@phone",        user.PhoneNumber);
            command.Parameters.AddWithValue("@roleId",       user.RoleID);
            command.Parameters.AddWithValue("@image",        user.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@passwordHash", passwordHash);
            command.Parameters.AddWithValue("@id",           user.Id);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при обновлении пользователя.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при обновлении пользователя.", ex);
        }
    }

    // ── Soft Delete ───────────────────────────────────────────────────────────

    public bool DeleteUser(int userId)
    {
        try
        {
            const string query = @"
                UPDATE users
                SET is_deleted = 1
                WHERE id = @id
                  AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", userId);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при удалении пользователя.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при удалении пользователя.", ex);
        }
    }

    // ── Проверка уникальности логина ──────────────────────────────────────────

    public bool LoginExists(string login, int? excludeUserId = null)
    {
        try
        {
            string query = excludeUserId.HasValue
                ? "SELECT COUNT(*) FROM users WHERE login = @login AND id != @excludeId AND is_deleted = 0;"
                : "SELECT COUNT(*) FROM users WHERE login = @login AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@login", login);
            if (excludeUserId.HasValue)
                command.Parameters.AddWithValue("@excludeId", excludeUserId.Value);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при проверке уникальности логина.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при проверке уникальности логина.", ex);
        }
    }
}