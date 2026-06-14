using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Pages.Admin;

namespace PhotoEquipmentStore.ViewModels;

public class AdminViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private ViewModelBase _currentViewModel;
    private NavigationMenuItem _selectedNavigationMenuItem;
    private UserInfo _currentUser;

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public NavigationMenuItem SelectedNavigationMenuItem
    {
        get => _selectedNavigationMenuItem;
        set => this.RaiseAndSetIfChanged(ref _selectedNavigationMenuItem, value);
    }

    public ObservableCollection<NavigationMenuItem> NavigationMenuItems { get; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public UserInfo CurrentUser
    {
        get => _currentUser;
        set => this.RaiseAndSetIfChanged(ref _currentUser, value);
    }

    public AdminViewModel(MainViewModel mainViewModel, UserInfo userInfo)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);

        ReactiveCommand<Unit, Unit> goToAddUserCommand   = ReactiveCommand.Create(GoToAddUser);
        ReactiveCommand<Unit, Unit> goToUsersCommand     = ReactiveCommand.Create(GoToUsers);
        ReactiveCommand<Unit, Unit> goToDataBaseCommand  = ReactiveCommand.Create(GoToDataBase);
        ReactiveCommand<Unit, Unit> goToReferenceCommand = ReactiveCommand.Create(GoToReference);

        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd",              goToAddUserCommand),
            new NavigationMenuItem("Пользователи",          "Users",                goToUsersCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase",             goToDataBaseCommand),
            new NavigationMenuItem("Справочники",           "Reference",            goToReferenceCommand),
        };
        _currentUser                = userInfo;
        _currentViewModel = new AdminWelcomeViewModel(_currentUser);
    }

    [Obsolete("Design-time only")]
    public AdminViewModel()
    {
        LogoutCommand = ReactiveCommand.Create(() => { });

        ReactiveCommand<Unit, Unit> goToAddUserCommand   = ReactiveCommand.Create(GoToAddUser);
        ReactiveCommand<Unit, Unit> goToUsersCommand     = ReactiveCommand.Create(GoToUsers);
        ReactiveCommand<Unit, Unit> goToDataBaseCommand  = ReactiveCommand.Create(GoToDataBase);
        ReactiveCommand<Unit, Unit> goToReferenceCommand = ReactiveCommand.Create(GoToReference);

        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd",   goToAddUserCommand),
            new NavigationMenuItem("Пользователи",          "Users",     goToUsersCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase",  goToDataBaseCommand),
            new NavigationMenuItem("Справочники",           "Reference", goToReferenceCommand),
        };

        _selectedNavigationMenuItem = NavigationMenuItems[0];
        _currentViewModel = new UserAddViewModel(GoToUsers);
        _currentUser                = new UserInfo(0, "Иванов Иван", "Админ",
            new Bitmap("/Users/ivanbarysev/RiderProjects/PhotoEquipmentStore/PhotoEquipmentStore.Desktop/Assets/user-test.jpg"));
    }

    private void Logout()
    {
        _mainViewModel.GoToLoginCommand.Execute().Subscribe();
    }

    private void GoToAddUser()
    {
        CurrentViewModel = new UserAddViewModel(GoToUsers);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Создание пользователя");
    }

    private void GoToUsers()
    {
        CurrentViewModel = new UsersViewModel(GoToEditUser, _currentUser.UserId);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Пользователи");
    }

    private void GoToEditUser(UserShow item)
    {
        CurrentViewModel = new UserAddViewModel(GoToUsers, item);
    }

    private void GoToDataBase()
    {
        CurrentViewModel = new DataBaseViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Работа с базой данных");
    }

    private void GoToReference()
    {
        CurrentViewModel = new ReferenceViewModel(GoToAddReference, GoToEditReference);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Справочники");
    }

    private void GoToAddReference(ReferenceType type)
    {
        CurrentViewModel = new ReferenceAddViewModel(
            goBack: GoToReference,
            type:   type);
    }

    private void GoToEditReference(ReferenceType type, ReferenceShow item)
    {
        CurrentViewModel = new ReferenceAddViewModel(
            goBack:   GoToReference,
            type:     type,
            editItem: item);
    }
}
