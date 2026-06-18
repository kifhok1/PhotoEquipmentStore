namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Данные авторизованного пользователя для текущей сессии работы с системой.
/// </summary>
public class UserAuth
{
    private int id;
    private string name;
    private string login;
    private string heshPassword;
    private byte[]? userImage;
    private int roleId;
    private string roleName;
    private int timeOfLogout;

    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public int Id { get => id; set => id = value; }
    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string Name { get => name; set => name = value; }
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login { get => login; set => login = value; }
    /// <summary>
    /// Хеш пароля пользователя из базы данных.
    /// </summary>
    public string HeshPassword { get => heshPassword; set => heshPassword = value; }
    /// <summary>
    /// Фотография пользователя.
    /// </summary>
    public byte[]? UserImage { get => userImage; set => userImage = value; }
    /// <summary>
    /// Идентификатор роли пользователя.
    /// </summary>
    public int RoleId { get => roleId; set => roleId = value; }
    /// <summary>
    /// Наименование роли пользователя.
    /// </summary>
    public string RoleName { get => roleName; set => roleName = value; }
    /// <summary>
    /// Время автоматического выхода из системы (в часах).
    /// </summary>
    public int TimeOfLogout { get => timeOfLogout; set => timeOfLogout = value; }

    /// <summary>
    /// Создаёт объект авторизованного пользователя с полным набором данных.
    /// </summary>
    public UserAuth(int id,
        string name,
        string login,
        string heshPassword,
        byte[]? userImage,
        int roleId,
        string roleName,
        int timeOfLogout)
    {
        this.id = id;
        this.name = name;
        this.login = login;
        this.heshPassword = heshPassword;
        this.userImage = userImage;
        this.roleId = roleId;
        this.roleName = roleName;
        this.timeOfLogout = timeOfLogout;
    }
}
