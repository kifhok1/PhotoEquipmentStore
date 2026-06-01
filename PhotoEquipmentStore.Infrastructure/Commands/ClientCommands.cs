using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ClientCommands
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    public static ObservableCollection<Client> GetClients()
    {
        try
        {
            var clients = new ObservableCollection<Client>();

            const string query = @"
                SELECT
                    c.id,
                    c.full_name          AS name,
                    c.phone              AS phoneNumber,
                    CAST(c.total_purchases AS UNSIGNED) AS totalPurchases,
                    COUNT(o.article)     AS countOrders
                FROM clients c
                LEFT JOIN orders o ON c.id = o.client_id
                WHERE c.is_deleted = 0
                GROUP BY c.id, c.full_name, c.phone, c.total_purchases;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            while (reader.Read())
            {
                clients.Add(new Client(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetString("phoneNumber"),
                    reader.GetInt32("totalPurchases"),
                    reader.GetInt32("countOrders")
                ));
            }

            return clients;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении списка клиентов.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении списка клиентов.", ex);
        }
    }

    public bool CreateClient(Client client)
    {
        try
        {
            const string query = @"
                INSERT INTO clients (full_name, phone, total_purchases)
                VALUES (@name, @phone, @totalPurchases);";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name",           client.FullName);
            command.Parameters.AddWithValue("@phone",          client.Phone);
            command.Parameters.AddWithValue("@totalPurchases", client.TotalPurchases);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при создании клиента.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при создании клиента.", ex);
        }
    }

    public bool UpdateClient(Client client)
    {
        try
        {
            const string query = @"
                UPDATE clients
                SET full_name       = @name,
                    phone           = @phone
                WHERE id = @id
                  AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name",           client.FullName);
            command.Parameters.AddWithValue("@phone",          client.Phone);
            command.Parameters.AddWithValue("@id",             client.Id);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при обновлении клиента.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при обновлении клиента.", ex);
        }
    }

    public bool DeleteClient(int clientId)
    {
        try
        {
            const string query = @"
                UPDATE clients
                SET is_deleted = 1
                WHERE id = @id
                  AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", clientId);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при удалении клиента.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при удалении клиента.", ex);
        }
    }
}