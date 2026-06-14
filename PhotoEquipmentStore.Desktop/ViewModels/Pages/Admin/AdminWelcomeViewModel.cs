using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public class AdminWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public AdminWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}
