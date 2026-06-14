using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Краткая информация об авторизованном пользователе для боковой панели.
/// </summary>


public class UserInfo
{
    private string userName;
    private string userRole;
    private Bitmap userImage;
    private int userId;

    /// <summary>

    /// Идентификатор пользователя.

    /// </summary>

    public int UserId
    {
        get
        {
            return userId;
        }
    }

    /// <summary>

    /// ФИО пользователя.

    /// </summary>

    public string UserName
    {
        get
        {
            return userName;
        }
    }

    /// <summary>

    /// Роль пользователя.

    /// </summary>

    public string UserRole
    {
        get
        {
            return userRole;
        }
    }

    /// <summary>

    /// Аватар пользователя.

    /// </summary>

    public Bitmap UserImage
    {
        get
        {
            return userImage;
        }
    }

    public UserInfo(int userID, string userName, string userRole, Bitmap userImage)
    {
        this.userId = userID;
        this.userName = userName;
        this.userRole = userRole;
        this.userImage = userImage;
    }
}
