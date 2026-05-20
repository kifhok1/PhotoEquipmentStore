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
        
        // Инициализация команд навигации
        ReactiveCommand<Unit, Unit> goToAddUserCommand = ReactiveCommand.Create(GoToAddUser);
        ReactiveCommand<Unit, Unit> goToUsersCommand = ReactiveCommand.Create(GoToUsers);
        ReactiveCommand<Unit, Unit> goToDataBaseCommand = ReactiveCommand.Create(GoToDataBase);
        ReactiveCommand<Unit, Unit> goToReferenceCommand = ReactiveCommand.Create(GoToReference);
        ReactiveCommand<Unit, Unit> goToReferenceAddCommand = ReactiveCommand.Create(GoToAddReference);
        ReactiveCommand<Unit, Unit> goToReferenceEditCommand = ReactiveCommand.Create(GoToEditReference);
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd", goToAddUserCommand),
            new NavigationMenuItem("Пользователи", "Users", goToUsersCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase", goToDataBaseCommand),
            new NavigationMenuItem("Справочники", "Reference", goToReferenceCommand),
        };
        
        _selectedNavigationMenuItem = NavigationMenuItems[0];
        _currentViewModel = new UserAddViewModel();
        _currentUser = userInfo;
    }
    
     // Конструктор для дизайнера
    [Obsolete("Design-time only")]
    public AdminViewModel()
    {
        // Инициализация для дизайна (без MainViewModel)
        LogoutCommand = ReactiveCommand.Create(() => { }); // Пустая команда для дизайна
        
        ReactiveCommand<Unit, Unit> goToAddUserCommand = ReactiveCommand.Create(GoToAddUser);
        ReactiveCommand<Unit, Unit> goToUsersCommand = ReactiveCommand.Create(GoToUsers);
        ReactiveCommand<Unit, Unit> goToDataBaseCommand = ReactiveCommand.Create(GoToDataBase);
        ReactiveCommand<Unit, Unit> goToReferenceCommand = ReactiveCommand.Create(GoToReference);
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd", goToAddUserCommand),
            new NavigationMenuItem("Пользователи", "Users", goToUsersCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase", goToDataBaseCommand),
            new NavigationMenuItem("Справочники", "Reference", goToReferenceCommand),
        };
        
        _selectedNavigationMenuItem = NavigationMenuItems[0];
        _currentViewModel = new UserAddViewModel();
        _currentUser = new UserInfo("Ианов Иван", "Админ",
            new Bitmap("/Users/ivanbarysev/RiderProjects/PhotoEquipmentStore/PhotoEquipmentStore.Desktop/Assets/user-test.jpg"));
    }
    
    
    private void Logout()
    {
        _mainViewModel.GoToLoginCommand.Execute().Subscribe();
    }

    private void GoToAddUser()
    {
        CurrentViewModel = new UserAddViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Создание пользователя");
    }

    private void GoToUsers()
    {
        CurrentViewModel = new UsersViewModel(this);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Пользователи");
    }

    private void GoToDataBase()
    {
        CurrentViewModel = new DataBaseViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Работа с базой данных");
    }

    private void GoToReference()
    {
        
        ReactiveCommand<Unit, Unit> goToAddReferenceCommand = ReactiveCommand.Create(GoToAddReference);
        ReactiveCommand<Unit, Unit> goToEditReferenceCommand = ReactiveCommand.Create(GoToEditReference);
        CurrentViewModel = new ReferenceViewModel(goToAddReferenceCommand, goToEditReferenceCommand);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Справочники");
    }
    
    private void GoToAddReference()
    {
        CurrentViewModel = new ReferenceAddViewModel(this);
    }
    
    private void GoToEditReference()
    {
        CurrentViewModel = new ReferenceAddViewModel(this);
    }
}
