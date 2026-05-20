using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public partial class UsersViewModel : ViewModelBase
{
    public ObservableCollection<UserShow> Users
    {
        get;
        private set;
    } = new();

    private string usersCount;
    public string UsersCount
    {
        get => usersCount;
        set => usersCount = value;
    }
    
    public ViewModelBase CurrentViewModel
    {
        get; 
        set;
    }
    
    public UsersViewModel(ViewModelBase currentViewModel)
    {
        CurrentViewModel = currentViewModel;
        
        var usersDb = UsersService.GetUsers();
        foreach (var user in usersDb)
        {
            Users.Add(new UserShow(
                user.Id,
                user.Name,
                user.Login,
                user.PhoneNumber,
                user.Role, 
                user.RoleID, 
                BitmapHelper.FromBytes(user.Image)));
        }
        UsersCount = $"Количество элементов на форме: {Users.Count}";
    }

    [RelayCommand]
    private void EditUser()
    {
        CurrentViewModel = new UserAddViewModel();
    }
}