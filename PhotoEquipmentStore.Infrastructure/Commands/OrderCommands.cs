using System;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Helpers;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class OrderCommands
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    public static ObservableCollection<Order> GetOrders()
    {
        try
        {
            var orders = new ObservableCollection<Order>();

            const string query = @"
                SELECT
                    o.article                                                        AS orderId,
                    CAST(o.client_id AS CHAR)                                        AS clientId,
                    c.full_name                                                      AS clientName,
                    c.phone                                                          AS clientPhoneNumber,
                    CAST(o.discount_percent AS UNSIGNED)                             AS discountClient,
                    o.employee_id                                                    AS userId,
                    u.full_name                                                      AS userName,
                    CAST(o.status_id AS CHAR)                                        AS statusId,
                    os.name                                                          AS statusName,
                    o.created_at                                                     AS orderDate,
                    COALESCE(SUM(oi.quantity * oi.price * (1 - oi.discount_percent / 100)), 0) AS totalSum
                FROM orders o
                JOIN clients       c  ON o.client_id   = c.id
                JOIN users         u  ON o.employee_id = u.id
                JOIN order_statuses os ON o.status_id   = os.id
                LEFT JOIN order_items oi ON o.article  = oi.order_article
                GROUP BY o.article, o.client_id, c.full_name, c.phone,
                         o.discount_percent, o.employee_id, u.full_name,
                         o.status_id, os.name, o.created_at;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(new Order(
                    reader.GetString("orderId"),
                    reader.GetString("clientId"),
                    reader.GetString("clientName"),
                    reader.GetString("clientPhoneNumber"),
                    reader.GetInt32("discountClient"),
                    reader.GetInt32("userId"),
                    reader.GetString("userName"),
                    reader.GetString("statusId"),
                    reader.GetString("statusName"),
                    reader.GetDateTime("orderDate"),
                    decimal.Round(reader.GetDecimal("totalSum"), 2, MidpointRounding.AwayFromZero)
                ));
            }

            return orders;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении списка заказов.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении списка заказов.", ex);
        }
    }

    public static ObservableCollection<OrderItem> GetOrderItems(string orderArticle)
    {
        try
        {
            var items = new ObservableCollection<OrderItem>();

            const string query = @"
                SELECT
                    oi.product_id                        AS productId,
                    p.name                               AS productName,
                    oi.quantity,
                    oi.price,
                    p.image                              AS productImage,
                    CAST(oi.discount_percent AS UNSIGNED) AS discount
                FROM order_items oi
                JOIN products p ON oi.product_id = p.id
                WHERE oi.order_article = @orderArticle;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@orderArticle", orderArticle);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new OrderItem(
                    reader.GetInt32("productId"),
                    reader.GetString("productName"),
                    reader.GetInt32("quantity"),
                    reader.GetDecimal("price"),
                    reader.GetInt32("discount"),
                    BlobReader.ToBytes(reader, "productImage")
                ));
            }

            return items;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении позиций заказа.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении позиций заказа.", ex);
        }
    }
}