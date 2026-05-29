using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Helpers;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ProductCommands
{
    private static readonly string connString = ConnectionSettingsParser.Load().ToString();
    
    public static ObservableCollection<Product> GetProducts()
    {
        
        var products = new ObservableCollection<Product>();

        string query = @"
            SELECT 
                p.id,
                p.name,
                p.description,
                CAST(p.price AS UNSIGNED) AS price,
                CAST(p.discount_percent AS UNSIGNED) AS discount,
                p.stock_quantity AS quantity,
                p.category_id AS categoryId,
                cat.name AS categoryName,
                p.manufacturer_id AS manufacturerId,
                m.name AS manufacturerName,
                p.supplier_id AS supplierId,
                s.name AS supplierName,
                p.image
            FROM products p
            JOIN categories cat ON p.category_id = cat.id
            JOIN manufacturers m ON p.manufacturer_id = m.id
            JOIN suppliers s ON p.supplier_id = s.id;";

        using var connection = new MySqlConnection(connString); 
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            string description = reader.GetString("description");
            int price = reader.GetInt32("price");
            int discount = reader.GetInt32("discount");
            int quantity = reader.GetInt32("quantity");
            int categoryId = reader.GetInt32("categoryId");
            string categoryName = reader.GetString("categoryName");
            int manufacturerId = reader.GetInt32("manufacturerId");
            string manufacturerName = reader.GetString("manufacturerName");
            int supplierId = reader.GetInt32("supplierId");
            string supplierName = reader.GetString("supplierName");
            byte[]? image = BlobReader.ToBytes(reader, "image");

            products.Add(new Product(id, name, price, discount,
                quantity, categoryId, categoryName, manufacturerId, manufacturerName,
                supplierId, supplierName,description, image));
        }

        return products;
    }
}