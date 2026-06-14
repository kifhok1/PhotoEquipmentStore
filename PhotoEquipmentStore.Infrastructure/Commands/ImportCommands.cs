using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Connection;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ImportCommands
{
    private static string ConnString => ConnectionSettingsParser.Load().ToString();

    public async Task<bool> ExistsAsync(string table, string column, object value)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(
                    $"SELECT COUNT(1) FROM `{table}` WHERE `{column}` = @v;", conn);
                cmd.Parameters.AddWithValue("@v", value);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Ошибка проверки существования в {table}.{column}.", ex);
        }
    }

    public async Task<bool> IsUniqueAsync(string table, string column, object value)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(
                    $"SELECT COUNT(1) FROM `{table}` WHERE `{column}` = @v;", conn);
                cmd.Parameters.AddWithValue("@v", value);
                return Convert.ToInt32(cmd.ExecuteScalar()) == 0;
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Ошибка проверки уникальности в {table}.{column}.", ex);
        }
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                const string sql = @"
                    SELECT COUNT(1) FROM (
                        SELECT phone FROM clients WHERE phone = @p
                        UNION ALL
                        SELECT phone FROM users   WHERE phone = @p
                    ) t;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p", phone);
                return Convert.ToInt32(cmd.ExecuteScalar()) == 0;
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка проверки уникальности телефона.", ex);
        }
    }

    public async Task InsertSimpleAsync(string table, string name)
    {
        try
        {
            await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(
                    $"INSERT INTO `{table}` (name) VALUES (@name);", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Ошибка вставки в {table}.", ex);
        }
    }

    public async Task InsertClientAsync(string fullName, string phone)
    {
        try
        {
            await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO clients (full_name, phone) VALUES (@n, @p);", conn);
                cmd.Parameters.AddWithValue("@n", fullName);
                cmd.Parameters.AddWithValue("@p", phone);
                cmd.ExecuteNonQuery();
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка вставки клиента.", ex);
        }
    }

    public async Task InsertUserAsync(
        string fullName, string login, string passwordHash,
        string phone, int roleId)
    {
        try
        {
            await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(@"
                    INSERT INTO users (full_name, login, password_hash, phone, role_id)
                    VALUES (@fn, @l, @ph, @p, @r);", conn);
                cmd.Parameters.AddWithValue("@fn", fullName);
                cmd.Parameters.AddWithValue("@l",  login);
                cmd.Parameters.AddWithValue("@ph", passwordHash);
                cmd.Parameters.AddWithValue("@p",  phone);
                cmd.Parameters.AddWithValue("@r",  roleId);
                cmd.ExecuteNonQuery();
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка вставки пользователя.", ex);
        }
    }

    public async Task InsertProductAsync(
        string name, int categoryId, decimal price, decimal discountPercent,
        string description, int stockQuantity, int supplierId, int manufacturerId)
    {
        try
        {
            await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(@"
                    INSERT INTO products
                        (name, category_id, price, discount_percent,
                         description, stock_quantity, supplier_id, manufacturer_id)
                    VALUES
                        (@name, @cat, @price, @disc,
                         @desc, @stock, @sup, @man);", conn);
                cmd.Parameters.AddWithValue("@name",  name);
                cmd.Parameters.AddWithValue("@cat",   categoryId);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@disc",  discountPercent);
                cmd.Parameters.AddWithValue("@desc",  description);
                cmd.Parameters.AddWithValue("@stock", stockQuantity);
                cmd.Parameters.AddWithValue("@sup",   supplierId);
                cmd.Parameters.AddWithValue("@man",   manufacturerId);
                cmd.ExecuteNonQuery();
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка вставки товара.", ex);
        }
    }

    public async Task InsertOrderAsync(
        string article, int statusId, int clientId,
        int discountPercent, int employeeId, DateTime createdAt)
    {
        try
        {
            await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(@"
                    INSERT INTO orders
                        (article, status_id, client_id, discount_percent, employee_id, created_at)
                    VALUES
                        (@art, @st, @cl, @disc, @emp, @dt);", conn);
                cmd.Parameters.AddWithValue("@art",  article);
                cmd.Parameters.AddWithValue("@st",   statusId);
                cmd.Parameters.AddWithValue("@cl",   clientId);
                cmd.Parameters.AddWithValue("@disc", discountPercent);
                cmd.Parameters.AddWithValue("@emp",  employeeId);
                cmd.Parameters.AddWithValue("@dt",   createdAt.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка вставки заказа.", ex);
        }
    }

    public async Task InsertOrderItemAsync(
        string orderArticle, int productId,
        int quantity, decimal price, decimal discountPercent)
    {
        try
        {
            await Task.Run(() =>
            {
                using var conn = new MySqlConnection(ConnString);
                conn.Open();
                using var cmd = new MySqlCommand(@"
                    INSERT INTO order_items
                        (order_article, product_id, quantity, price, discount_percent)
                    VALUES
                        (@art, @prod, @qty, @price, @disc);", conn);
                cmd.Parameters.AddWithValue("@art",   orderArticle);
                cmd.Parameters.AddWithValue("@prod",  productId);
                cmd.Parameters.AddWithValue("@qty",   quantity);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@disc",  discountPercent);
                cmd.ExecuteNonQuery();
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка вставки позиции заказа.", ex);
        }
    }
}
