using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Сonnection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ClientCommands
{
    private static readonly string connString = ConnectionSettingsParser.Load().ToString();
    
    public static ObservableCollection<Client> GetClients()
    {
        var clients = new ObservableCollection<Client>();

        string query = @"SELECT 
                                c.id, 
                                c.full_name AS name, 
                                c.phone AS phoneNumber, 
                                CAST(c.total_purchases AS UNSIGNED) AS totalPurchases, 
                                COUNT(o.article) AS countOrders
                          FROM clients c
                          LEFT JOIN orders o ON c.id = o.client_id
                          WHERE c.is_deleted = 0
                          GROUP BY c.id, c.full_name, c.phone, c.total_purchases;";
        
        using var connection = new MySqlConnection(connString);
        connection.Open();

        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            string phone = reader.GetString("phoneNumber");
            int totalPurchases = reader.GetInt32("totalPurchases");
            int countOrders = reader.GetInt32("countOrders");

            clients.Add(new Client(id, name, phone, totalPurchases, countOrders));
        }

        return clients;
    }
}