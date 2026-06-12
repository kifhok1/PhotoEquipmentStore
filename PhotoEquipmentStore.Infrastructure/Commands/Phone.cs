using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class Phone
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    public List<string> GetAllPhoneNumbers()
    {
        try
        {
            var phones = new List<string>();

            const string query = @"
                SELECT phone FROM users   WHERE phone IS NOT NULL AND phone != '' AND is_deleted = 0
                UNION ALL
                SELECT phone FROM clients WHERE phone IS NOT NULL AND phone != '' AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            while (reader.Read())
                phones.Add(reader.GetString("phone"));

            return phones;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении списка номеров телефонов.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении списка номеров телефонов.", ex);
        }
    }
}