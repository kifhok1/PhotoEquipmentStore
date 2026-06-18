using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;/// <summary>
/// Приветственный экран раздела менеджера.
/// </summary>


public class ManagerWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public ManagerWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}
