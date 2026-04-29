using MySql.Data.MySqlClient;

namespace PhotoEquipmentStore.Infrastructure.Helpers;

public class BlobReader
{
    public static byte[]? ToBytes(MySqlDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        
        if (reader.IsDBNull(ordinal))
            return null;

        return (byte[])reader[ordinal];
    }
}