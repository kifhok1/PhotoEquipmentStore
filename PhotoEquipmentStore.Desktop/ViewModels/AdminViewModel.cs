using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Pages;

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
    
    public AdminViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);
        
        // Инициализация команд навигации
        ReactiveCommand<Unit, Unit> goToAddUserCommand = ReactiveCommand.Create(GoToAddUser);
        ReactiveCommand<Unit, Unit> goToUsersCommand = ReactiveCommand.Create(GoToUsers);
        ReactiveCommand<Unit, Unit> goToDataBaseCommand = ReactiveCommand.Create(GoToDataBase);
        ReactiveCommand<Unit, Unit> goToReferenceCommand = ReactiveCommand.Create(GoToReference);
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd", navigateCommand:goToAddUserCommand),
            new NavigationMenuItem("Пользователи", "Users", navigateCommand:goToUsersCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase", goToDataBaseCommand),
            new NavigationMenuItem("Справочники", "Reference", goToReferenceCommand),
        };
        
        _selectedNavigationMenuItem = NavigationMenuItems[0];
        _currentViewModel = new UserAddViewModel();
        _currentUser = new UserInfo("Ианов Иван", "Админ",
            new Bitmap("/Users/ivanbarysev/RiderProjects/PhotoEquipmentStore/PhotoEquipmentStore.Desktop/Assets/user-test.jpg"));
    }
    
     // Конструктор для дизайнера
    [Obsolete("Design-time only")]
    public AdminViewModel()
    {
        // Инициализация для дизайна (без MainViewModel)
        LogoutCommand = ReactiveCommand.Create(() => { }); // Пустая команда для дизайна
        
        // Создаем пустые команды для дизайнера
        var goToAddUserCommand = ReactiveCommand.Create(() => { });
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd", goToAddUserCommand),
            new NavigationMenuItem("Пользователи", "Users", goToAddUserCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase", goToAddUserCommand),
            new NavigationMenuItem("Справочники", "Reference", goToAddUserCommand),
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
        CurrentViewModel = new UsersViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Пользователи");
    }

    private void GoToDataBase()
    {
        CurrentViewModel = new DataBaseViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Работа с базой данных");
    }

    private void GoToReference()
    {
        CurrentViewModel = new ReferenceViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Справочники");
    }

}
