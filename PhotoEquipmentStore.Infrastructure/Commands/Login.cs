using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Connection;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class Login
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    public List<string> GetAllLogins()
    {
        try
        {
            var logins = new List<string>();

            const string query = "SELECT login FROM users WHERE is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            while (reader.Read())
                logins.Add(reader.GetString("login"));

            return logins;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении списка логинов.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении списка логинов.", ex);
        }
    }
}