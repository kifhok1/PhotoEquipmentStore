using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Сonnection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ReferenceCommands
{
    private static readonly string connString = ConnectionSettingsParser.Load().ToString();
    
    // -------------------------------------------------------------
    // Роли (зависимые объекты – активные пользователи)
    // -------------------------------------------------------------
    public static ObservableCollection<Reference> GetRoles()
    {
        var roles = new ObservableCollection<Reference>();

        string query = @"
            SELECT 
                r.id, 
                r.name, 
                COUNT(u.id) AS cnt,
                CASE WHEN COUNT(u.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM roles r
            LEFT JOIN users u ON u.role_id = r.id AND u.is_deleted = 0
            GROUP BY r.id, r.name;";

        using var connection = new MySqlConnection(connString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            int count = reader.GetInt32("cnt");
            bool isDeleted = reader.GetBoolean("isDeleted");

            roles.Add(new Reference(id, name, count, isDeleted));
        }
        return roles;
    }

    // -------------------------------------------------------------
    // Статусы заказов (зависимые объекты – заказы)
    // -------------------------------------------------------------
    public static ObservableCollection<Reference> GetOrderStatuses()
    {
        var statuses = new ObservableCollection<Reference>();

        string query = @"
            SELECT 
                os.id, 
                os.name, 
                COUNT(o.article) AS cnt,
                CASE WHEN COUNT(o.article) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM order_statuses os
            LEFT JOIN orders o ON o.status_id = os.id
            GROUP BY os.id, os.name;";

        using var connection = new MySqlConnection(connString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            int count = reader.GetInt32("cnt");
            bool isDeleted = reader.GetBoolean("isDeleted");

            statuses.Add(new Reference(id, name, count, isDeleted));
        }
        return statuses;
    }

    // -------------------------------------------------------------
    // Категории (зависимые объекты – товары)
    // -------------------------------------------------------------
    public static ObservableCollection<Reference> GetCategories()
    {
        var categories = new ObservableCollection<Reference>();

        string query = @"
            SELECT 
                cat.id, 
                cat.name, 
                COUNT(p.id) AS cnt,
                CASE WHEN COUNT(p.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM categories cat
            LEFT JOIN products p ON p.category_id = cat.id
            GROUP BY cat.id, cat.name;";

        using var connection = new MySqlConnection(connString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            int count = reader.GetInt32("cnt");
            bool isDeleted = reader.GetBoolean("isDeleted");

            categories.Add(new Reference(id, name, count, isDeleted));
        }
        return categories;
    }

    // -------------------------------------------------------------
    // Поставщики (зависимые объекты – товары)
    // -------------------------------------------------------------
    public static ObservableCollection<Reference> GetSuppliers()
    {
        var suppliers = new ObservableCollection<Reference>();

        string query = @"
            SELECT 
                s.id, 
                s.name, 
                COUNT(p.id) AS cnt,
                CASE WHEN COUNT(p.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM suppliers s
            LEFT JOIN products p ON p.supplier_id = s.id
            GROUP BY s.id, s.name;";

        using var connection = new MySqlConnection(connString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            int count = reader.GetInt32("cnt");
            bool isDeleted = reader.GetBoolean("isDeleted");

            suppliers.Add(new Reference(id, name, count, isDeleted));
        }
        return suppliers;
    }

    // -------------------------------------------------------------
    // Производители (зависимые объекты – товары)
    // -------------------------------------------------------------
    public static ObservableCollection<Reference> GetManufacturers()
    {
        var manufacturers = new ObservableCollection<Reference>();

        string query = @"
            SELECT 
                m.id, 
                m.name, 
                COUNT(p.id) AS cnt,
                CASE WHEN COUNT(p.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM manufacturers m
            LEFT JOIN products p ON p.manufacturer_id = m.id
            GROUP BY m.id, m.name;";

        using var connection = new MySqlConnection(connString);
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            int count = reader.GetInt32("cnt");
            bool isDeleted = reader.GetBoolean("isDeleted");

            manufacturers.Add(new Reference(id, name, count, isDeleted));
        }
        return manufacturers;
    }
}