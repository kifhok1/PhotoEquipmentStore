using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Domain.Enums;
using PhotoEquipmentStore.Infrastructure.Connection;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ReportCommands
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    public DashboardStats GetDashboardStats()
    {
        try
        {
            const string query = @"
                SELECT
                    COUNT(DISTINCT CASE WHEN DATE(o.created_at) = CURDATE() THEN o.article END)
                        AS ordersToday,
                    COALESCE(SUM(CASE WHEN DATE(o.created_at) = CURDATE() THEN oi.quantity END), 0)
                        AS itemsToday,
                    COALESCE(SUM(CASE WHEN DATE(o.created_at) = CURDATE()
                        THEN oi.quantity * oi.price * (1 - oi.discount_percent / 100) END), 0)
                        AS revenueToday,
                    COUNT(DISTINCT CASE
                        WHEN MONTH(o.created_at) = MONTH(CURDATE())
                         AND YEAR(o.created_at)  = YEAR(CURDATE())
                        THEN o.article END)
                        AS ordersMonth,
                    COALESCE(SUM(CASE
                        WHEN MONTH(o.created_at) = MONTH(CURDATE())
                         AND YEAR(o.created_at)  = YEAR(CURDATE())
                        THEN oi.quantity END), 0)
                        AS itemsMonth,
                    COALESCE(SUM(CASE
                        WHEN MONTH(o.created_at) = MONTH(CURDATE())
                         AND YEAR(o.created_at)  = YEAR(CURDATE())
                        THEN oi.quantity * oi.price * (1 - oi.discount_percent / 100) END), 0)
                        AS revenueMonth
                FROM orders o
                LEFT JOIN order_items oi ON o.article = oi.order_article;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            if (!reader.Read())
                return new DashboardStats(0, 0, 0m, 0, 0, 0m);

            return new DashboardStats(
                reader.GetInt32("ordersToday"),
                reader.GetInt32("itemsToday"),
                decimal.Round(reader.GetDecimal("revenueToday"), 2),
                reader.GetInt32("ordersMonth"),
                reader.GetInt32("itemsMonth"),
                decimal.Round(reader.GetDecimal("revenueMonth"), 2)
            );
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении статистики.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении статистики.", ex);
        }
    }

    public (DateTime Min, DateTime Max) GetOrderDateRange()
    {
        try
        {
            const string query = @"
                SELECT
                    DATE(MIN(created_at)) AS minDate,
                    DATE(MAX(created_at)) AS maxDate
                FROM orders;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            if (!reader.Read() || reader.IsDBNull(reader.GetOrdinal("minDate")))
                return (DateTime.Today, DateTime.Today);

            return (
                reader.GetDateTime("minDate"),
                reader.GetDateTime("maxDate")
            );
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении диапазона дат.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении диапазона дат.", ex);
        }
    }

    public List<SalesReportData> GetSalesReport(DateTime from, DateTime to)
    {
        try
        {
            const string query = @"
                SELECT
                    o.article                                                                   AS orderId,
                    o.created_at                                                                AS orderDate,
                    c.full_name                                                                 AS clientName,
                    c.phone                                                                     AS clientPhone,
                    u.full_name                                                                 AS employeeName,
                    os.name                                                                     AS statusName,
                    o.status_id                                                                 AS statusId,
                    CAST(o.discount_percent AS UNSIGNED)                                        AS discountPercent,
                    COUNT(oi.product_id)                                                        AS itemsCount,
                    COALESCE(SUM(oi.quantity), 0)                                               AS totalQuantity,
                    COALESCE(SUM(oi.quantity * oi.price * (1 - oi.discount_percent / 100)), 0) AS totalSum
                FROM orders o
                JOIN clients        c  ON o.client_id   = c.id
                JOIN users          u  ON o.employee_id = u.id
                JOIN order_statuses os ON o.status_id   = os.id
                LEFT JOIN order_items oi ON o.article   = oi.order_article
                WHERE DATE(o.created_at) BETWEEN @from AND @to
                GROUP BY o.article, o.created_at, c.full_name, c.phone,
                         u.full_name, os.name, o.status_id, o.discount_percent
                ORDER BY o.created_at;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@from", from.Date);
            command.Parameters.AddWithValue("@to",   to.Date);

            using var reader = command.ExecuteReader();
            var result = new List<SalesReportData>();

            while (reader.Read())
            {
                result.Add(new SalesReportData(
                    reader.GetString("orderId"),
                    reader.GetDateTime("orderDate"),
                    reader.GetString("clientName"),
                    reader.GetString("clientPhone"),
                    reader.GetString("employeeName"),
                    reader.GetString("statusName"),
                    reader.GetInt32("discountPercent"),
                    reader.GetInt32("itemsCount"),
                    reader.GetInt32("totalQuantity"),
                    decimal.Round(reader.GetDecimal("totalSum"), 2),
                    reader.GetInt32("statusId") == 2
                ));
            }

            return result;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении отчёта по продажам.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении отчёта по продажам.", ex);
        }
    }

    public List<StockReportData> GetStockReport(int? categoryId)
    {
        try
        {
            string query = @"
                SELECT
                    p.id                                         AS productId,
                    p.name                                       AS productName,
                    cat.name                                     AS categoryName,
                    m.name                                       AS manufacturerName,
                    s.name                                       AS supplierName,
                    p.stock_quantity                             AS quantity
                FROM products p
                JOIN categories    cat ON p.category_id     = cat.id
                JOIN manufacturers   m ON p.manufacturer_id = m.id
                JOIN suppliers       s ON p.supplier_id     = s.id
                WHERE p.is_deleted = 0"
                + (categoryId.HasValue ? " AND p.category_id = @categoryId" : "")
                + " ORDER BY cat.name, p.name;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            if (categoryId.HasValue)
                command.Parameters.AddWithValue("@categoryId", categoryId.Value);

            using var reader = command.ExecuteReader();
            var result = new List<StockReportData>();

            while (reader.Read())
            {
                result.Add(new StockReportData(
                    reader.GetInt32("productId"),
                    reader.GetString("productName"),
                    reader.GetString("categoryName"),
                    reader.GetString("manufacturerName"),
                    reader.GetString("supplierName"),
                    reader.GetInt32("quantity")
                ));
            }

            return result;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении отчёта по остаткам.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении отчёта по остаткам.", ex);
        }
    }

    public List<PopularityReportData> GetPopularityReport(int? categoryId, PopularityMode mode)
    {
        try
        {
            bool descending = mode is PopularityMode.AllDesc or PopularityMode.Top30;
            bool hasLimit   = mode is PopularityMode.Top30   or PopularityMode.Bottom30;

            string query = @"
                SELECT
                    p.id                             AS productId,
                    p.name                           AS productName,
                    cat.name                         AS categoryName,
                    m.name                           AS manufacturerName,
                    CAST(p.price AS UNSIGNED)        AS price,
                    COALESCE(SUM(oi.quantity), 0)    AS totalSold,
                    COUNT(DISTINCT oi.order_article) AS ordersCount
                FROM products p
                JOIN categories    cat ON p.category_id     = cat.id
                JOIN manufacturers   m ON p.manufacturer_id = m.id
                LEFT JOIN order_items oi ON p.id = oi.product_id
                WHERE p.is_deleted = 0"
                + (categoryId.HasValue ? " AND p.category_id = @categoryId" : "")
                + " GROUP BY p.id, p.name, cat.name, m.name, p.price"
                + " ORDER BY totalSold " + (descending ? "DESC" : "ASC")
                + (hasLimit ? " LIMIT 30" : "") + ";";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            if (categoryId.HasValue)
                command.Parameters.AddWithValue("@categoryId", categoryId.Value);

            using var reader = command.ExecuteReader();
            var result = new List<PopularityReportData>();
            int rank = 1;

            while (reader.Read())
            {
                result.Add(new PopularityReportData(
                    rank++,
                    reader.GetInt32("productId"),
                    reader.GetString("productName"),
                    reader.GetString("categoryName"),
                    reader.GetString("manufacturerName"),
                    reader.GetInt32("price"),
                    reader.GetInt32("totalSold"),
                    reader.GetInt32("ordersCount")
                ));
            }

            return result;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении отчёта по популярности.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении отчёта по популярности.", ex);
        }
    }
}
