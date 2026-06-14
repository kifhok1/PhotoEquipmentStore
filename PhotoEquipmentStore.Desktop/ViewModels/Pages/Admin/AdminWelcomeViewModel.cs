using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;/// <summary>
/// Приветственный экран раздела администратора.
/// </summary>


public class AdminWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public AdminWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}
