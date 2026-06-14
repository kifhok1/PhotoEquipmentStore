namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Параметры подключения к базе данных MySQL и готовая строка соединения.
/// </summary>
public class ConnectionToDBSettings
{
    private string host;
    private string user;
    private string password;
    private string database;
    private string connectionString;

    /// <summary>
    /// Адрес хоста сервера базы данных.
    /// </summary>
    public string Host
    {
        get => host;
        set => host = value;
    }

    /// <summary>
    /// Имя пользователя для подключения к базе данных.
    /// </summary>
    public string User
    {
        get => user;
        set => user = value;
    }

    /// <summary>
    /// Пароль для подключения к базе данных.
    /// </summary>
    public string Password
    {
        get => password;
        set => password = value;
    }

    /// <summary>
    /// Имя базы данных.
    /// </summary>
    public string Database
    {
        get => database;
        set => database = value;
    }

    /// <summary>
    /// Готовая строка подключения в формате MySQL Connector.
    /// </summary>
    public string ConnectionString
    {
        get => connectionString;
    }

    /// <summary>
    /// Создаёт настройки подключения и формирует строку соединения.
    /// </summary>
    public ConnectionToDBSettings(string host, string username, string password, string database)
    {
        Host = host;
        User = username;
        Password = password;
        Database = database;
        // Формирование строки подключения в формате, ожидаемом MySqlConnection
        this.connectionString = $"host={Host};uid={User};pwd={Password};database={Database}";
    }
}
