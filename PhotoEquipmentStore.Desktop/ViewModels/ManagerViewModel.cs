using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Pages.Manager;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels;/// <summary>
/// ViewModel раздела менеджера: товары и отчёты.
/// </summary>


public class ManagerViewModel : ViewModelBase
{

    private readonly MainViewModel _mainViewModel;
    private ViewModelBase _currentViewModel;
    private NavigationMenuItem _selectedNavigationMenuItem;
    private UserInfo _currentUser;

    /// <summary>

    /// Команда выхода из системы.

    /// </summary>

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    /// <summary>

    /// Выбранный пункт бокового меню.

    /// </summary>

    public NavigationMenuItem SelectedNavigationMenuItem
    {
        get => _selectedNavigationMenuItem;
        set => this.RaiseAndSetIfChanged(ref _selectedNavigationMenuItem, value);
    }

    /// <summary>

    /// Коллекция пунктов навигационного меню.

    /// </summary>

    public ObservableCollection<NavigationMenuItem> NavigationMenuItems { get; }

    /// <summary>

    /// Активная дочерняя ViewModel (экран входа или раздел роли).

    /// </summary>

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    /// <summary>

    /// Информация о текущем пользователе раздела.

    /// </summary>

    public UserInfo CurrentUser
    {
        get => _currentUser;
        set => this.RaiseAndSetIfChanged(ref _currentUser, value);
    }

    public ManagerViewModel(MainViewModel mainViewModel, UserInfo userInfo)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);

        ReactiveCommand<Unit, Unit> goToReportsCommand = ReactiveCommand.Create(GoToReports);
        ReactiveCommand<Unit, Unit> goToProductsCommand = ReactiveCommand.Create(GoToProducts);
        ReactiveCommand<Unit, Unit> goToProductAddCommand = ReactiveCommand.Create(GoToProductAdd);

        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Формирование отчёта", "Records",  goToReportsCommand),
            new NavigationMenuItem("Товары", "Product", goToProductsCommand),
            new NavigationMenuItem("Создание товара", "ProductAdd", goToProductAddCommand),
        };
        _currentUser = userInfo;
        _currentViewModel = new ManagerWelcomeViewModel(_currentUser);
    }

    [Obsolete("Design-time only")]
    public ManagerViewModel()
    {

        LogoutCommand = ReactiveCommand.Create(() => { });

        ReactiveCommand<Unit, Unit> goToReportsCommand = ReactiveCommand.Create(GoToReports);
        ReactiveCommand<Unit, Unit> goToProductsCommand = ReactiveCommand.Create(GoToProducts);
        ReactiveCommand<Unit, Unit> goToProductAddCommand = ReactiveCommand.Create(GoToProductAdd);

        NavigationMenuItems = new ObservableCollection<NavigationMenuItem>
        {
            new NavigationMenuItem("Формирование отчёта", "Records",  goToReportsCommand),
            new NavigationMenuItem("Товары", "Product", goToProductsCommand),
            new NavigationMenuItem("Создание товара", "ProductAdd", goToProductAddCommand),
        };

        _selectedNavigationMenuItem = NavigationMenuItems[0];
        _currentViewModel = new ReportsViewModel();
        _currentUser = new UserInfo(0, "Ианов Иван", "Админ",
            new Bitmap("/Users/ivanbarysev/RiderProjects/PhotoEquipmentStore/PhotoEquipmentStore.Desktop/Assets/user-test.jpg"));
    }

    private void Logout()
    {
        _mainViewModel.GoToLoginCommand.Execute().Subscribe();
    }

    private void GoToReports()
    {
        CurrentViewModel = new ReportsViewModel();
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Формирование отчёта");
    }

    private void GoToProducts()
    {
        CurrentViewModel = new ProductsViewModel(GoToEditProduct);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Товары");
    }

    private void GoToProductAdd()
    {
        CurrentViewModel = new ProductAddViewModel(goBack: GoToProducts);
        SelectedNavigationMenuItem = NavigationMenuItems.First(item => item.Title == "Создание товара");
    }

    private void GoToEditProduct(ProductsShow item)
    {
        CurrentViewModel = new ProductAddViewModel(goBack: GoToProducts, editItem: item);
    }
}
