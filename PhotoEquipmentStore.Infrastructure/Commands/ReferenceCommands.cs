using System;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Exceptions;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class ReferenceCommands
{
    private static readonly string ConnString = ConnectionSettingsParser.Load().ToString();

    public static ObservableCollection<Reference> GetRoles()
    {
        const string query = @"
            SELECT
                r.id,
                r.name,
                COUNT(u.id) AS cnt,
                CASE WHEN COUNT(u.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM roles r
            LEFT JOIN users u ON u.role_id = r.id AND u.is_deleted = 0
            GROUP BY r.id, r.name;";

        return ExecuteReferenceQuery(query, "Ошибка при получении списка ролей.");
    }

    public static ObservableCollection<Reference> GetOrderStatuses()
    {
        const string query = @"
            SELECT
                os.id,
                os.name,
                COUNT(o.article) AS cnt,
                CASE WHEN COUNT(o.article) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM order_statuses os
            LEFT JOIN orders o ON o.status_id = os.id
            GROUP BY os.id, os.name;";

        return ExecuteReferenceQuery(query, "Ошибка при получении списка статусов заказов.");
    }

    public static ObservableCollection<Reference> GetCategories()
    {
        const string query = @"
            SELECT
                cat.id,
                cat.name,
                COUNT(p.id) AS cnt,
                CASE WHEN COUNT(p.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM categories cat
            LEFT JOIN products p ON p.category_id = cat.id
            GROUP BY cat.id, cat.name;";

        return ExecuteReferenceQuery(query, "Ошибка при получении списка категорий.");
    }

    public static ObservableCollection<Reference> GetSuppliers()
    {
        const string query = @"
            SELECT
                s.id,
                s.name,
                COUNT(p.id) AS cnt,
                CASE WHEN COUNT(p.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM suppliers s
            LEFT JOIN products p ON p.supplier_id = s.id
            GROUP BY s.id, s.name;";

        return ExecuteReferenceQuery(query, "Ошибка при получении списка поставщиков.");
    }

    public static ObservableCollection<Reference> GetManufacturers()
    {
        const string query = @"
            SELECT
                m.id,
                m.name,
                COUNT(p.id) AS cnt,
                CASE WHEN COUNT(p.id) = 0 THEN 1 ELSE 0 END AS isDeleted
            FROM manufacturers m
            LEFT JOIN products p ON p.manufacturer_id = m.id
            GROUP BY m.id, m.name;";

        return ExecuteReferenceQuery(query, "Ошибка при получении списка производителей.");
    }

    public bool CreateReference(string table, string name)
    {
        try
        {
            string query = $"INSERT INTO {table} (name) VALUES (@name);";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", name);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Ошибка при создании записи в {table}.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Непредвиденная ошибка при создании записи в {table}.", ex);
        }
    }

    public bool UpdateReference(string table, Reference reference)
    {
        try
        {
            string query = $"UPDATE {table} SET name = @name WHERE id = @id;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", reference.Name);
            command.Parameters.AddWithValue("@id",   reference.Id);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Ошибка при обновлении записи в {table}.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Непредвиденная ошибка при обновлении записи в {table}.", ex);
        }
    }

    public bool DeleteReference(string table, int id)
    {
        try
        {
            string query = $"DELETE FROM {table} WHERE id = @id;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Ошибка при удалении записи из {table}.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Непредвиденная ошибка при удалении записи из {table}.", ex);
        }
    }

    public bool NameExists(string table, string name, int? excludeId = null)
    {
        try
        {
            string query = excludeId.HasValue
                ? $"SELECT COUNT(*) FROM {table} WHERE name = @name AND id != @excludeId;"
                : $"SELECT COUNT(*) FROM {table} WHERE name = @name;";

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", name);
            if (excludeId.HasValue)
                command.Parameters.AddWithValue("@excludeId", excludeId.Value);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Ошибка при проверке уникальности в {table}.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Непредвиденная ошибка при проверке уникальности в {table}.", ex);
        }
    }

    private static ObservableCollection<Reference> ExecuteReferenceQuery(string query, string errorMessage)
    {
        try
        {
            var result = new ObservableCollection<Reference>();

            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var reader  = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Reference(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetInt32("cnt"),
                    reader.GetBoolean("isDeleted")
                ));
            }

            return result;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException(errorMessage, ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Непредвиденная ошибка. {errorMessage}", ex);
        }
    }
}
