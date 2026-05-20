using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;

public class UserShow
{
    private int id;
    private string name;
    private string login;
    private string phoneNumber;
    private string role;
    private int roleID;
    private Bitmap? image = null;

    public int Id
    {
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
        set => login = value;
    }

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
    
    public bool IsVisibleImage
    {
        get => image != null;
    }

    public UserShow(
        int id,
        string name,
        string login,
        string phoneNumber,
        string role,
        int roleID,
        Bitmap image)
    {
        Id = id;
        Name = name;
        Login = login;
        PhoneNumber = phoneNumber;
        Role = role;
        RoleID = roleID;
        Image = image;
        
    }
    
    public UserShow(
        int id,
        string name,
        string login,
        string phoneNumber,
        string role,
        int roleID)
    {
        Id = id;
        Name = name;
        Login = login;
        PhoneNumber = phoneNumber;
        Role = role;
        RoleID = roleID;
    }
}