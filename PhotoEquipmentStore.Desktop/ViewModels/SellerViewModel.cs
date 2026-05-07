using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Pages;
using PhotoEquipmentStore.ViewModels.Pages.Manager;
using PhotoEquipmentStore.ViewModels.Pages.Seller;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels;

public class SellerViewModel : ViewModelBase
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
    
    public SellerViewModel(MainViewModel mainViewModel, UserInfo userInfo)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);
        
        // Инициализация команд навигации
        ReactiveCommand<Unit, Unit> goToClientCommand = ReactiveCommand.Create(GoToReports);
        ReactiveCommand<Unit, Unit> goToClientAddCommand = ReactiveCommand.Create(GoToDashboard);
        ReactiveCommand<Unit, Unit> goToOrderCommand = ReactiveCommand.Create(GoToProducts);
        ReactiveCommand<Unit, Unit> goToOrderAddCommand = ReactiveCommand.Create(GoToProductAdd);
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            
            new NavigationMenuItem("Клиенты", "Client",  goToClientCommand),
            new NavigationMenuItem("Добавление клиента", "ClientAdd",  goToClientAddCommand),
            new NavigationMenuItem("Заказы", "Order", goToOrderCommand),
            new NavigationMenuItem("Создание заказа", "AddOrder", goToOrderAddCommand),
        };
        
        _selectedNavigationMenuItem = NavigationMenuItems[0];
        _currentViewModel = new UserAddViewModel();
        _currentUser = userInfo;
    }
    
     // Конструктор для дизайнера
    [Obsolete("Design-time only")]
    public SellerViewModel()
    {
        // Инициализация для дизайна (без MainViewModel)
        LogoutCommand = ReactiveCommand.Create(() => { }); // Пустая команда для дизайна
        
        // Создаем пустые команды для дизайнера
        ReactiveCommand<Unit, Unit> goToClientCommand = ReactiveCommand.Create(GoToReports);
        ReactiveCommand<Unit, Unit> goToClientAddCommand = ReactiveCommand.Create(GoToDashboard);
        ReactiveCommand<Unit, Unit> goToOrderCommand = ReactiveCommand.Create(GoToProducts);
        ReactiveCommand<Unit, Unit> goToOrderAddCommand = ReactiveCommand.Create(GoToProductAdd);
        
        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            
            new NavigationMenuItem("Клиенты", "Client",  goToClientCommand),
            new NavigationMenuItem("Добавление клиента", "ClientAdd",  goToClientAddCommand),
            new NavigationMenuItem("Заказы", "Order", goToOrderCommand),
            new NavigationMenuItem("Создание заказа", "OrderAdd", goToOrderAddCommand),
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

    private void GoToDashboard()
    {
        CurrentViewModel = new ClientsViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Клиенты");
    }

    private void GoToReports()
    {
        CurrentViewModel = new ClientAddViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Добавление клиента");
    }

    private void GoToProducts()
    {
        CurrentViewModel = new OrdersViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Заказы");
    }

    private void GoToProductAdd()
    {
        CurrentViewModel = new OrderAddViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Создание заказа");
    }
}