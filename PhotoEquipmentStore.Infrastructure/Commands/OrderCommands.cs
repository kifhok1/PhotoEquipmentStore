using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Helpers;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class OrderCommands
{
    private static readonly string connString = ConnectionSettingsParser.Load().ToString();
    
    public static ObservableCollection<Order> GetOrders()
    {
        var orders = new ObservableCollection<Order>();

        string query = @"
            SELECT 
                o.article AS orderId,
                CAST(o.client_id AS CHAR) AS clientId,
                c.full_name AS clientName,
                c.phone AS clientPhoneNumber,
                CAST(o.discount_percent AS UNSIGNED) AS discountClient,
                o.employee_id AS userId,
                u.full_name AS userName,
                CAST(o.status_id AS CHAR) AS statusId,
                os.name AS statusName,
                o.created_at AS orderDate,
                COALESCE(SUM(oi.quantity * oi.price * (1 - oi.discount_percent / 100)), 0) 
                             AS totalSum
            FROM orders o
            JOIN clients c ON o.client_id = c.id
            JOIN users u ON o.employee_id = u.id
            JOIN order_statuses os ON o.status_id = os.id
            LEFT JOIN order_items oi ON o.article = oi.order_article
            GROUP BY o.article, o.client_id, c.full_name, c.phone, 
                     o.discount_percent, o.employee_id, u.full_name, 
                     o.status_id, os.name, o.created_at;";

        using (var connection = new MySqlConnection(connString))
        {
            connection.Open();

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string orderId = reader.GetString("orderId");
                    string clientId = reader.GetString("clientId");
                    string clientName = reader.GetString("clientName");
                    string clientPhone = reader.GetString("clientPhoneNumber");
                    int discountClient = reader.GetInt32("discountClient");
                    int userId = reader.GetInt32("userId");
                    string userName = reader.GetString("userName");
                    string statusId = reader.GetString("statusId");
                    string statusName = reader.GetString("statusName");
                    DateTime orderDate = reader.GetDateTime("orderDate");
                    decimal totalSum = decimal.Round(reader.GetDecimal("totalSum"), 2, MidpointRounding.AwayFromZero);

                    orders.Add(new Order(orderId, clientId, clientName, 
                        clientPhone, discountClient, userId, userName, 
                        statusId, statusName, orderDate, totalSum));
                }
            }
        }

        return orders;
    }

    public static ObservableCollection<OrderItem> GetOrderItems(string orderArticle)
    {
        var items = new ObservableCollection<OrderItem>();

        string query = @"
            SELECT 
                oi.product_id AS productId,
                p.name AS productName,
                oi.quantity,
                oi.price,
                p.image AS productImage,
                CAST(oi.discount_percent AS UNSIGNED) AS discount
            FROM order_items oi
            JOIN products p ON oi.product_id = p.id
            WHERE oi.order_article = @orderArticle;";

        using (var connection = new MySqlConnection(connString))
        {
            connection.Open();

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@orderArticle", orderArticle);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int productId = reader.GetInt32("productId");
                        string productName = reader.GetString("productName");
                        int quantity = reader.GetInt32("quantity");
                        decimal price = reader.GetDecimal("price");
                        int discount = reader.GetInt32("discount");
                        byte[] productImage = BlobReader.ToBytes(reader, "productImage");

                        items.Add(new OrderItem(productId, productName, quantity, price, discount, productImage));
                    }
                }
            }
        }

        return items;
    }
}