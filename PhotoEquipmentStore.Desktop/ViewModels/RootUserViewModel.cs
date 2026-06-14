using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Pages.Admin;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels;

public class RootUserViewModel : ViewModelBase
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
    
    public RootUserViewModel(MainViewModel mainViewModel, UserInfo userInfo)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);
        
        // Инициализация команд навигации
        ReactiveCommand<Unit, Unit> goToAddUserCommand = ReactiveCommand.Create(GoToAddUser);
        ReactiveCommand<Unit, Unit> goToDataBaseCommand = ReactiveCommand.Create(GoToDataBase);
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd", goToAddUserCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase", goToDataBaseCommand)
        };
        
        _currentUser = userInfo;
        _currentViewModel = new RootWelcomeViewModel(_currentUser);
    }
    
     // Конструктор для дизайнера
    [Obsolete("Design-time only")]
    public RootUserViewModel()
    {
        // Инициализация для дизайна (без MainViewModel)
        LogoutCommand = ReactiveCommand.Create(() => { }); // Пустая команда для дизайна
        
        ReactiveCommand<Unit, Unit> goToAddUserCommand = ReactiveCommand.Create(GoToAddUser);
        ReactiveCommand<Unit, Unit> goToDataBaseCommand = ReactiveCommand.Create(GoToDataBase);
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Создание пользователя", "UserAdd", goToAddUserCommand),
            new NavigationMenuItem("Работа с базой данных", "DataBase", goToDataBaseCommand)
        }; 
        
        _currentUser = new UserInfo(0, "Ианов Иван", "Админ",
            new Bitmap("/Users/ivanbarysev/RiderProjects/PhotoEquipmentStore/PhotoEquipmentStore.Desktop/Assets/user-test.jpg"));
        _currentViewModel = new RootWelcomeViewModel(_currentUser);
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

    private void GoToDataBase()
    {
        CurrentViewModel = new DataBaseViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Работа с базой данных");
    }
}