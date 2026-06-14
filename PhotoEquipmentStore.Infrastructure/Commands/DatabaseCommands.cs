using System.Data;
using MySql.Data.MySqlClient;
using PhotoEquipmentStore.Infrastructure.Connection;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Infrastructure.Commands;

public class DatabaseCommands
{
    private static string ConnString => ConnectionSettingsParser.Load().ToString();

    public static readonly IReadOnlyList<string> AllTables =
    [
        "categories",
        "clients",
        "manufacturers",
        "order_items",
        "order_statuses",
        "orders",
        "products",
        "roles",
        "suppliers",
        "users"
    ];

    // ── Backup ────────────────────────────────────────────────────────────────

    public async Task CreateBackupAsync(string outputPath)
    {
        try
        {
            await Task.Run(() =>
            {
                using var connection = new MySqlConnection(ConnString);
                using var command = new MySqlCommand {
                    Connection = connection 
                };
                using var backup     = new MySqlBackup(command);
                connection.Open();
                backup.ExportToFile(outputPath);
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка при создании резервной копии.", ex);
        }
    }

    // ── Restore ───────────────────────────────────────────────────────────────

    public async Task RestoreStructureAsync(string sqlFilePath)
    {
        try
        {
            await Task.Run(() =>
            {
                using var connection = new MySqlConnection(ConnString);
                using var command = new MySqlCommand {
                    Connection = connection 
                };
                using var backup     = new MySqlBackup(command);
                connection.Open();
                backup.ImportFromFile(sqlFilePath);
            });
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ошибка при восстановлении базы данных.", ex);
        }
    }

    // ── Fetch table ───────────────────────────────────────────────────────────

    public async Task<DataTable> FetchTableAsync(string tableName)
    {
        if (!AllTables.Contains(tableName))
            throw new DatabaseException($"Неизвестная таблица: {tableName}.");

        try
        {
            return await Task.Run(() =>
            {
                using var connection = new MySqlConnection(ConnString);
                connection.Open();

                // Получаем список колонок, исключая MEDIUMBLOB (изображения)
                var columnsQuery = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME   = @tableName
                  AND DATA_TYPE   != 'mediumblob'
                ORDER BY ORDINAL_POSITION;";

                var columns = new List<string>();
                using (var cmd = new MySqlCommand(columnsQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                        columns.Add($"`{reader.GetString(0)}`");
                }

                if (columns.Count == 0)
                    return new DataTable();

                var selectQuery = $"SELECT {string.Join(", ", columns)} FROM `{tableName}`;";
                using var adapter = new MySqlDataAdapter(selectQuery, connection);
                var table         = new DataTable();
                adapter.Fill(table);
                return table;
            });
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Ошибка при чтении таблицы {tableName}.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Непредвиденная ошибка при чтении таблицы {tableName}.", ex);
        }
    }
}