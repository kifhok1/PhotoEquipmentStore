using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;

public class ManagerWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public ManagerWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}