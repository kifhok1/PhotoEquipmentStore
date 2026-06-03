using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Helpers;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ProductCommands
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    // ── Read ──────────────────────────────────────────────────────────────────

    public static ObservableCollection<Product> GetProducts()
    {
        try
        {
            var products = new ObservableCollection<Product>();

            const string query = @"
                SELECT
                    p.id,
                    p.name,
                    p.description,
                    CAST(p.price            AS UNSIGNED) AS price,
                    CAST(p.discount_percent AS UNSIGNED) AS discount,
                    p.stock_quantity                     AS quantity,
                    p.category_id                        AS categoryId,
                    cat.name                             AS categoryName,
                    p.manufacturer_id                    AS manufacturerId,
                    m.name                               AS manufacturerName,
                    p.supplier_id                        AS supplierId,
                    s.name                               AS supplierName,
                    p.image
                FROM products p
                JOIN categories    cat ON p.category_id    = cat.id
                JOIN manufacturers   m ON p.manufacturer_id = m.id
                JOIN suppliers       s ON p.supplier_id     = s.id
                WHERE p.is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetInt32("price"),
                    reader.GetInt32("discount"),
                    reader.GetInt32("quantity"),
                    reader.GetInt32("categoryId"),
                    reader.GetString("categoryName"),
                    reader.GetInt32("manufacturerId"),
                    reader.GetString("manufacturerName"),
                    reader.GetInt32("supplierId"),
                    reader.GetString("supplierName"),
                    reader.GetString("description"),
                    BlobReader.ToBytes(reader, "image")
                ));
            }

            return products;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при получении списка товаров.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при получении списка товаров.", ex);
        }
    }

    // ── Create ────────────────────────────────────────────────────────────────

    public bool CreateProduct(Product product)
    {
        try
        {
            const string query = @"
                INSERT INTO products
                    (name, description, price, discount_percent, stock_quantity,
                     category_id, manufacturer_id, supplier_id, image)
                VALUES
                    (@name, @description, @price, @discount, @quantity,
                     @categoryId, @manufacturerId, @supplierId, @image);";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name",           product.Name);
            command.Parameters.AddWithValue("@description",    product.Description);
            command.Parameters.AddWithValue("@price",          product.Price);
            command.Parameters.AddWithValue("@discount",       product.Discount);
            command.Parameters.AddWithValue("@quantity",       product.Quantity);
            command.Parameters.AddWithValue("@categoryId",     product.CategoryId);
            command.Parameters.AddWithValue("@manufacturerId", product.ManufacturerId);
            command.Parameters.AddWithValue("@supplierId",     product.SupplierId);
            command.Parameters.AddWithValue("@image",          product.Image ?? (object)DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при создании товара.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при создании товара.", ex);
        }
    }

    // ── Update ────────────────────────────────────────────────────────────────

    public bool UpdateProduct(Product product)
    {
        try
        {
            const string query = @"
                UPDATE products
                SET name             = @name,
                    description      = @description,
                    price            = @price,
                    discount_percent = @discount,
                    stock_quantity   = @quantity,
                    category_id      = @categoryId,
                    manufacturer_id  = @manufacturerId,
                    supplier_id      = @supplierId,
                    image            = @image
                WHERE id = @id
                  AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name",           product.Name);
            command.Parameters.AddWithValue("@description",    product.Description);
            command.Parameters.AddWithValue("@price",          product.Price);
            command.Parameters.AddWithValue("@discount",       product.Discount);
            command.Parameters.AddWithValue("@quantity",       product.Quantity);
            command.Parameters.AddWithValue("@categoryId",     product.CategoryId);
            command.Parameters.AddWithValue("@manufacturerId", product.ManufacturerId);
            command.Parameters.AddWithValue("@supplierId",     product.SupplierId);
            command.Parameters.AddWithValue("@image",          product.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@id",             product.Id);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при обновлении товара.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при обновлении товара.", ex);
        }
    }

    // ── Soft Delete ───────────────────────────────────────────────────────────

    public bool DeleteProduct(int productId)
    {
        try
        {
            const string query = @"
                UPDATE products
                SET is_deleted = 1
                WHERE id = @id
                  AND is_deleted = 0;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", productId);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Ошибка при удалении товара.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Непредвиденная ошибка при удалении товара.", ex);
        }
    }
}