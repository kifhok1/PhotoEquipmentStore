using MySql.Data.MySqlClient;

namespace PhotoEquipmentStore.Infrastructure.Helpers;

/// <summary>
/// Вспомогательные методы для чтения бинарных данных (BLOB) из результата SQL-запроса.
/// </summary>
public class BlobReader
{
    /// <summary>
    /// Извлекает массив байтов из указанного столбца; возвращает null, если значение отсутствует.
    /// </summary>
    public static byte[]? ToBytes(MySqlDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(ordinal))
            return null;

        return (byte[])reader[ordinal];
    }
}
