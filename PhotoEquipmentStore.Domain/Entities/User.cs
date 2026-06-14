namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Пользователь системы (сотрудник магазина) с учётными данными и ролью.
/// </summary>
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

    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public int Id {
        get => id;
        set => id = value;
    }

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string Name
    {
        get => name;
        set => name = value;
    }

    /// <summary>
    /// Логин для входа в систему.
    /// </summary>
    public string Login
    {
        get => login;
        set  => login = value;
    }

    /// <summary>
    /// Пароль пользователя (в открытом виде, используется при создании).
    /// </summary>
    public string Password
    {
        get => password;
        set => password = value;
    }

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string PhoneNumber {
        get => phoneNumber;
        set => phoneNumber = value;
    }

    /// <summary>
    /// Наименование роли пользователя.
    /// </summary>
    public string Role
    {
        get => role;
        set => role = value;
    }

    /// <summary>
    /// Идентификатор роли пользователя.
    /// </summary>
    public int RoleID
    {
        get => roleID;
        set => roleID = value;
    }

    /// <summary>
    /// Фотография пользователя в виде массива байтов.
    /// </summary>
    public byte[]? Image
    {
        get => image;
        set => image = value;
    }

    /// <summary>
    /// Создаёт пользователя с полным набором полей, включая пароль.
    /// </summary>
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
    /// <summary>
    /// Создаёт пользователя без пароля (для отображения в списке).
    /// </summary>
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

    /// <summary>
    /// Создаёт нового пользователя без идентификатора для сохранения в базу.
    /// </summary>
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
