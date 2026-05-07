namespace PhotoEquipmentStore.Domain.Entities;

public class ConnectionToDBSettings
{
    private string host;
    private string user;
    private string password;
    private string database;
    private string connectionString;
    
    public string Host
    {
        get => host;
        set => host = value;
    }
    
    public string User
    {
        get => user;
        set => user = value;
    }
    
    public string Password
    {
        get => password;
        set => password = value;
    }

    public string Database
    {
        get => database;
        set => database = value;
    }

    public string ConnectionString
    {
        get => connectionString;
    }

    public ConnectionToDBSettings(string host, string username, string password, string database)
    {
        Host = host;
        User = username;
        Password = password;
        Database = database;
        this.connectionString = $"host={Host};uid={User};pwd={Password};database={Database}";
    }
}