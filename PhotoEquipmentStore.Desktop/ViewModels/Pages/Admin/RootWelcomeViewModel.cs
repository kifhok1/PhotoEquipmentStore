using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public class RootWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public RootWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}