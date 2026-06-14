using System;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Connection;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Helpers;

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
                    o.article                                                              AS orderId,
                    CAST(o.client_id AS CHAR)                                             AS clientId,
                    c.full_name                                                           AS clientName,
                    c.phone                                                               AS clientPhoneNumber,
                    CAST(o.discount_percent AS UNSIGNED)                                  AS discountClient,
                    o.employee_id                                                         AS userId,
                    u.full_name                                                           AS userName,
                    CAST(o.status_id AS CHAR)                                             AS statusId,
                    os.name                                                               AS statusName,
                    o.created_at                                                          AS orderDate,
                    COALESCE(SUM(oi.quantity * oi.price * (1 - oi.discount_percent / 100)), 0) AS totalSum
                FROM orders o
                JOIN clients        c  ON o.client_id   = c.id
                JOIN users          u  ON o.employee_id = u.id
                JOIN order_statuses os ON o.status_id   = os.id
                LEFT JOIN order_items oi ON o.article   = oi.order_article
                GROUP BY o.article, o.client_id, c.full_name, c.phone,
                         o.discount_percent, o.employee_id, u.full_name,
                         o.status_id, os.name, o.created_at
                ORDER BY o.created_at DESC;";

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
                    oi.product_id                         AS productId,
                    p.name                                AS productName,
                    oi.quantity,
                    oi.price,
                    p.image                               AS productImage,
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

    public bool UpdateOrderStatus(string orderArticle)
    {
        try
        {
            const string query = @"
                UPDATE orders
                SET status_id = 2
                WHERE article = @orderArticle;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@orderArticle", orderArticle);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при обновлении статуса заказа.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при обновлении статуса заказа.", ex);
        }
    }

    public bool ArticleExists(string article)
    {
        try
        {
            const string query = "SELECT COUNT(*) FROM orders WHERE article = @article;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@article", article);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при проверке артикула заказа.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при проверке артикула заказа.", ex);
        }
    }

    public bool CreateOrder(
        string orderArticle,
        int clientId,
        int employeeId,
        int discountPercent,
        decimal totalAmount,
        List<(int productId, int quantity, decimal price, decimal discountPercent)> items)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {

                const string stockSql = @"
                    UPDATE products
                    SET stock_quantity = stock_quantity - @qty
                    WHERE id = @id AND stock_quantity >= @qty;";

                foreach (var item in items)
                {
                    using var stockCmd = new MySqlCommand(stockSql, connection, transaction);
                    stockCmd.Parameters.AddWithValue("@qty", item.quantity);
                    stockCmd.Parameters.AddWithValue("@id",  item.productId);

                    if (stockCmd.ExecuteNonQuery() == 0)
                        throw new DatabaseException(
                            $"Недостаточно товара на складе (product_id={item.productId}).");
                }

                const string orderSql = @"
                    INSERT INTO orders (article, status_id, client_id, discount_percent, employee_id, created_at)
                    VALUES (@article, 1, @clientId, @discount, @employeeId, NOW());";

                using var orderCmd = new MySqlCommand(orderSql, connection, transaction);
                orderCmd.Parameters.AddWithValue("@article",    orderArticle);
                orderCmd.Parameters.AddWithValue("@clientId",   clientId);
                orderCmd.Parameters.AddWithValue("@discount",   discountPercent);
                orderCmd.Parameters.AddWithValue("@employeeId", employeeId);
                orderCmd.ExecuteNonQuery();

                const string itemSql = @"
                    INSERT INTO order_items (order_article, product_id, quantity, price, discount_percent)
                    VALUES (@article, @productId, @qty, @price, @discount);";

                foreach (var item in items)
                {
                    using var itemCmd = new MySqlCommand(itemSql, connection, transaction);
                    itemCmd.Parameters.AddWithValue("@article",   orderArticle);
                    itemCmd.Parameters.AddWithValue("@productId", item.productId);
                    itemCmd.Parameters.AddWithValue("@qty",       item.quantity);
                    itemCmd.Parameters.AddWithValue("@price",     item.price);
                    itemCmd.Parameters.AddWithValue("@discount",  item.discountPercent);
                    itemCmd.ExecuteNonQuery();
                }

                const string clientSql = @"
                    UPDATE clients
                    SET total_purchases = total_purchases + @amount
                    WHERE id = @clientId;";

                using var clientCmd = new MySqlCommand(clientSql, connection, transaction);
                clientCmd.Parameters.AddWithValue("@amount",   totalAmount);
                clientCmd.Parameters.AddWithValue("@clientId", clientId);
                clientCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при создании заказа.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при создании заказа.", ex);
        }
    }
}
