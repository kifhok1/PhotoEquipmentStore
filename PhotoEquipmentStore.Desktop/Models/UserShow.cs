using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Модель пользователя для отображения в списке администратора.
/// </summary>


public class UserShow
{
    private int id;
    private string name;
    private string login;
    private string phoneNumber;
    private string role;
    private int roleID;
    private Bitmap? image = null;

    /// <summary>

    /// Уникальный идентификатор записи.

    /// </summary>

    public int Id
    {
        get => id;
        set => id = value;
    }

    /// <summary>

    /// Наименование или ФИО.

    /// </summary>

    public string Name
    {
        get => name;
        set => name = value;
    }

    /// <summary>

    /// Логин пользователя.

    /// </summary>

    public string Login
    {
        get => login;
        set => login = value;
    }

    /// <summary>

    /// Номер телефона.

    /// </summary>

    public string PhoneNumber
    {
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

    public Bitmap Image
    {
        get => image;
        set => image = value;
    }

    /// <summary>

    /// Признак наличия изображения.

    /// </summary>

    public bool IsVisibleImage
    {
        get => image != null;
    }

    /// <summary>

    /// Признак, что запись относится к текущему пользователю.

    /// </summary>

    public bool IsSelf { get; }

    public UserShow(
        int id,
        string name,
        string login,
        string phoneNumber,
        string role,
        int roleID,
        Bitmap image,
        bool isSelf = false)
    {
        Id = id;
        Name = name;
        Login = login;
        PhoneNumber = phoneNumber;
        Role = role;
        RoleID = roleID;
        Image = image;
        IsSelf = isSelf;
    }
}
