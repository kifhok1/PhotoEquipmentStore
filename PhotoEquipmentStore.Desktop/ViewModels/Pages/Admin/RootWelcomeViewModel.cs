using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;/// <summary>
/// Приветственный экран системного пользователя root.
/// </summary>


public class RootWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public RootWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}
