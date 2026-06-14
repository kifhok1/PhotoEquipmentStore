using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class SellerWelcomeViewModel : ViewModelBase
{
    public UserInfo CurrentUser { get; }

    public SellerWelcomeViewModel(UserInfo currentUser)
    {
        CurrentUser = currentUser;
    }
}
