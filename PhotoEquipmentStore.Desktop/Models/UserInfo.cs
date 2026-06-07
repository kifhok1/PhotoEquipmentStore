using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;

public class UserInfo
{
    private string userName;
    private string userRole;
    private Bitmap userImage;
    private int userId;

    public int UserId
    {
        get
        {
            return userId;
        }
    }
    
    public string UserName
    {
        get
        {
            return userName;
        }
    }

    public string UserRole
    {
        get
        {
            return userRole;
        }
    }

    public Bitmap UserImage
    {
        get
        {
            return userImage;
        }
    }
    
    
    public UserInfo(string userName, string userRole, Bitmap userImage)
    {
        this.userName = userName;
        this.userRole = userRole;
        this.userImage = userImage;
    }
}