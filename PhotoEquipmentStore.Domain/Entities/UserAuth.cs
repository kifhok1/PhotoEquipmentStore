namespace PhotoEquipmentStore.Domain.Entities;

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
    
    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Login { get => login; set => login = value; }
    public string HeshPassword { get => heshPassword; set => heshPassword = value; }
    public byte[]? UserImage { get => userImage; set => userImage = value; }
    public int RoleId { get => roleId; set => roleId = value; }
    public string RoleName { get => roleName; set => roleName = value; }
    public int TimeOfLogout { get => timeOfLogout; set => timeOfLogout = value; }

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