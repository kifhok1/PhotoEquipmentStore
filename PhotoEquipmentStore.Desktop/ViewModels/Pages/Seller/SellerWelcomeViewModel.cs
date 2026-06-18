using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;/// <summary>
/// Приветственный экран раздела продавца.
/// </summary>


public class SellerWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public SellerWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}
