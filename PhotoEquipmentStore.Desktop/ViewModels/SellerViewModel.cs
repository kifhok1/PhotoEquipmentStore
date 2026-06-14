using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Timers;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Pages.Seller;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels;

public class SellerViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private ViewModelBase _currentViewModel;
    private NavigationMenuItem _selectedNavigationMenuItem;
    private UserInfo _currentUser;

    private readonly Timer _inactivityTimer;
    private const double InactivityTimeout = 3 * 60 * 1000;

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

    public void ResetInactivityTimer()
    {
        _inactivityTimer.Stop();
        _inactivityTimer.Start();
    }

    private void OnInactivityElapsed(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _inactivityTimer.Stop();
            _mainViewModel.GoToLoginCommand.Execute().Subscribe();
        });
    }

    public SellerViewModel(MainViewModel mainViewModel, UserInfo userInfo)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);

        ReactiveCommand<Unit, Unit> goToClientCommand    = ReactiveCommand.Create(GoToClients);
        ReactiveCommand<Unit, Unit> goToClientAddCommand = ReactiveCommand.Create(GoToAddClients);
        ReactiveCommand<Unit, Unit> goToOrderCommand     = ReactiveCommand.Create(GoToOrders);
        ReactiveCommand<Unit, Unit> goToOrderAddCommand  = ReactiveCommand.Create(GoToAddOrder);

        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Клиенты",            "Client",    goToClientCommand),
            new NavigationMenuItem("Добавление клиента", "ClientAdd", goToClientAddCommand),
            new NavigationMenuItem("Заказы",             "Order",     goToOrderCommand),
            new NavigationMenuItem("Создание заказа",    "AddOrder",  goToOrderAddCommand),
        };

        _currentUser = userInfo;
        _currentViewModel = new SellerWelcomeViewModel(_currentUser);

        _inactivityTimer = new Timer(InactivityTimeout) { AutoReset = false };
        _inactivityTimer.Elapsed += OnInactivityElapsed;
        _inactivityTimer.Start();
    }

    private void Logout()
    {
        _inactivityTimer.Stop();
        _mainViewModel.GoToLoginCommand.Execute().Subscribe();
    }

    private void GoToClients()
    {
        CurrentViewModel = new ClientsViewModel(GoToEditClient);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Клиенты");
    }

    private void GoToAddClients()
    {
        CurrentViewModel = new ClientAddViewModel(goBack: GoToClients);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Добавление клиента");
    }

    private void GoToEditClient(ClientShow item)
    {
        CurrentViewModel = new ClientAddViewModel(goBack: GoToClients, editItem: item);
    }

    private void GoToOrders()
    {
        CurrentViewModel = new OrdersViewModel(GoToOrderItems);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Заказы");
    }

    private void GoToAddOrder()
    {
        CurrentViewModel = new OrderAddViewModel(
            goToConfirm: GoToOrderConfirm,
            goBackToAdd: GoToAddOrder,
            seller: _currentUser);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Создание заказа");
    }

    private void GoToOrderItems(OrderShow order)
    {
        CurrentViewModel = new OrderItemsViewModel(
            order,
            goBack: GoToOrders);
    }

    private void GoToOrderConfirm(OrderConfirmViewModel confirmVm)
    {
        CurrentViewModel = confirmVm;
    }
}
