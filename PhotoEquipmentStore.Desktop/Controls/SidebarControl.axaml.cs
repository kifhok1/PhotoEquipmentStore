using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.Controls;

public partial class SidebarControl : UserControl
{

    public static readonly StyledProperty<ObservableCollection<NavigationMenuItem>> MenuItemsProperty =
        AvaloniaProperty.Register<SidebarControl, ObservableCollection<NavigationMenuItem>>(
            nameof(MenuItems));

    public static readonly StyledProperty<NavigationMenuItem> SelectedMenuItemProperty =
        AvaloniaProperty.Register<SidebarControl, NavigationMenuItem>(
            nameof(SelectedMenuItem));

    public static readonly StyledProperty<UserInfo> UsernameProperty =
        AvaloniaProperty.Register<SidebarControl, UserInfo>(
            nameof(Username));

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> LogoutCommandProperty =
        AvaloniaProperty.Register<SidebarControl, ReactiveCommand<Unit, Unit>>(
            nameof(LogoutCommand));

    public ObservableCollection<NavigationMenuItem> MenuItems
    {
        get => GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    public NavigationMenuItem SelectedMenuItem
    {
        get => GetValue(SelectedMenuItemProperty);
        set
        {
            if (MenuItems != null)
            {
                SetValue(SelectedMenuItemProperty, value);
            }
        }
    }

    public UserInfo Username
    {
        get => GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    public ReactiveCommand<Unit, Unit> LogoutCommand
    {
        get => GetValue(LogoutCommandProperty);
        set => SetValue(LogoutCommandProperty, value);
    }

    public SidebarControl()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            MenuItems =
            [
                new NavigationMenuItem( "Бекап", "DataBaseBackup"),
                new NavigationMenuItem( "Импорт данных", "DataBaseImport"),
                new NavigationMenuItem( "Экспорт данных", "DataBaseExport"),
                new NavigationMenuItem( "База данных", "DataBase"),
                new NavigationMenuItem( "Пользователи", "Users"),
                new NavigationMenuItem( "Пользователь", "User"),
                new NavigationMenuItem( "Добавить пользователя", "UserAdd"),
                new NavigationMenuItem( "Роли", "Role"),
                new NavigationMenuItem( "Категории", "Category"),
                new NavigationMenuItem( "Производители", "Manufacturers"),
                new NavigationMenuItem( "Поставщики", "Suppliers"),
                new NavigationMenuItem( "Статусы заказа", "Statuses"),
                new NavigationMenuItem( "Клиенты", "Clients"),
                new NavigationMenuItem( "Клиент", "Client"),
                new NavigationMenuItem( "Добавить клиента", "ClientAdd"),
                new NavigationMenuItem( "Товары", "Product"),
                new NavigationMenuItem( "Добавить товар", "ProductAdd"),
                new NavigationMenuItem( "Справочники", "Reference"),
                new NavigationMenuItem( "Отчёты", "Records"),
                new NavigationMenuItem( "Оформить заказ", "AddOrder"),
                new NavigationMenuItem( "Заказы", "Order"),
            ];

            Username = new UserInfo(0, "Иван Макаровouhpiuh phpihpyugpiuy",
                "Админстратор",
                new Bitmap("/Users/ivanbarysev/RiderProjects/PhotoEquipmentStore/PhotoEquipmentStore.Desktop/Assets/user-test.jpg"));

            LogoutCommand = ReactiveCommand.Create(() => { });
        }

        if (MenuItems != null)
        {
            SelectedMenuItem = MenuItems[0];
        }
    }
}
