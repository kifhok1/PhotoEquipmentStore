namespace PhotoEquipmentStore.Domain.Entities;

public class User
{
    private int id;
    private string name;
    private string login;
    private string password;
    private string phoneNumber;
    private string role;
    private int roleID;
    private byte[] image;
    
    public int Id { 
        get => id; 
        set => id = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public string Login
    {
        get => login; 
        set  => login = value;
    }

    public string Password
    {
        get => password;
        set => password = value;
    }
    
    public string PhoneNumber { 
        get => phoneNumber;
        set => phoneNumber = value;
    }

    public string Role
    {
        get => role;
        set => role = value;
    }

    public int RoleID
    {
        get => roleID;
        set => roleID = value;
    }

    public byte[]? Image
    {
        get => image;
        set => image = value;
    }

    public User(int id, string name, string login, string password,
        string phoneNumber, string role, int roleID, byte[]? image = null)
    {
        Id = id;
        Name = name;
        Login = login;
        Password = password;
        PhoneNumber = phoneNumber;
        Role = role;
        RoleID = roleID;
        Image = image;
    }
    public User(int id, string name, string login, string phoneNumber,
        string role, int roleID, byte[]? image = null)
    {
        Id = id;
        Name = name;
        Login = login;
        PhoneNumber = phoneNumber;
        Role = role;
        RoleID = roleID;
        Image = image;
    }
    
    public User(string name, string login, string password,
        string phoneNumber, int roleID, byte[]? image = null)
    {
        Name = name;
        Login = login;
        Password = password;
        PhoneNumber = phoneNumber;
        RoleID = roleID;
        Image = image;
    }
}