using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.SettingsUI;
using ReactiveUI;

namespace PhotoEquipmentStore.PopupComponent;

public partial class SettingsPopup : UserControl
{
    public static readonly StyledProperty<bool> BlockVisibleProperty =
        AvaloniaProperty.Register<SettingsPopup, bool>(nameof(BlockVisible));

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> CloseCommandProperty =
        AvaloniaProperty.Register<SettingsPopup, ReactiveCommand<Unit, Unit>>(
            nameof(CloseCommand));

    public static readonly DirectProperty<SettingsPopup, ObservableCollection<ThemeSettings>> ThemeProperty =
        AvaloniaProperty.RegisterDirect<SettingsPopup, ObservableCollection<ThemeSettings>>(
            nameof(Theme),
            o => o.Theme,
            (o, v) => o.Theme = v);

    public static readonly StyledProperty<Bitmap> ImageLoginFormProperty =
        AvaloniaProperty.Register<SettingsPopup, Bitmap>(
            nameof(ImageLoginForm), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> HostProperty =
        AvaloniaProperty.Register<SettingsPopup, string>(
            nameof(Host), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<string> UserProperty =
        AvaloniaProperty.Register<SettingsPopup, string>(
            nameof(User), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<string> PasswordProperty =
        AvaloniaProperty.Register<SettingsPopup, string>(
            nameof(Password), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<string> DatabaseProperty =
        AvaloniaProperty.Register<SettingsPopup, string>(
            nameof(Database), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> ConnectionErrorProperty =
        AvaloniaProperty.Register<SettingsPopup, bool>(
            nameof(ConnectionError), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<string> ConnectionMessageProperty =
        AvaloniaProperty.Register<SettingsPopup, string>(
            nameof(ConnectionMessage), defaultBindingMode: BindingMode.TwoWay);

    private ObservableCollection<ThemeSettings> _theme;

    public ObservableCollection<ThemeSettings> Theme
    {
        get => _theme;
        set => SetAndRaise(ThemeProperty, ref _theme, value);
    }

    public static readonly StyledProperty<ThemeSettings> ThemeSelectedProperty =
        AvaloniaProperty.Register<SettingsPopup, ThemeSettings>(nameof(ThemeSelected));

    public ThemeSettings ThemeSelected
    {
        get => GetValue(ThemeSelectedProperty);
        set => SetValue(ThemeSelectedProperty, value);
    }

    public Bitmap ImageLoginForm
    {
        get => GetValue(ImageLoginFormProperty);
        set => SetValue(ImageLoginFormProperty, value);
    }

    public bool BlockVisible
    {
        get => GetValue(BlockVisibleProperty);
        set => SetValue(BlockVisibleProperty, value);
    }

    public ReactiveCommand<Unit, Unit> CloseCommand
    {
        get => GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public string Host
    {
        get => GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }

    public string User
    {
        get => GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }

    public string Password
    {
        get => GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public string Database
    {
        get => GetValue(DatabaseProperty);
        set => SetValue(DatabaseProperty, value);
    }

    public bool ConnectionError
    {
        get => GetValue(ConnectionErrorProperty);
        set => SetValue(ConnectionErrorProperty,  value);
    }

    public string ConnectionMessage
    {
        get => GetValue(ConnectionMessageProperty);
        set => SetValue(ConnectionMessageProperty, value);
    }

    [RelayCommand]
    public void TestConnection()
    {
        TestConnectionDto testConnection = TestConnectionService.TestConnection(
            new ConnectionToDBSettings(Host, User, Password, Database));

        ConnectionError = testConnection.IsConnected;
        if (testConnection.IsConnected)
        {
            ConnectionMessage = "Подключено";
        }
        else
        {
            ConnectionMessage = "Не подключено";
        }
    }

    private void SaveConnectionSettings()
    {
        TestConnection();
        ConnectionServise.SaveConnection(new ConnectionToDBSettings(Host, User, Password, Database));
    }

    private void SaveUISettings()
    {
         SettingsUIFileParser.SetTheme(ThemeSelected.Theme);
         var basePath = AppContext.BaseDirectory;
         if (SettingsUIFileParser.GetTheme() == "Тёмная")
         {
             ThemeService.Toggle(true);
             ImageLoginForm = new Bitmap(Path.Combine(basePath, "Assets", "login-background-dark.jpg"));
         }
         else
         {
             ThemeService.Toggle(false);
             ImageLoginForm = new Bitmap(Path.Combine(basePath, "Assets", "login-background-light.png"));
         }
    }

    [RelayCommand]
    private void SaveAndClose()
    {
        SaveConnectionSettings();
        SaveUISettings();
        CloseCommand?.Execute(Unit.Default).Subscribe();
    }

    public SettingsPopup()
    {
        ConnectionToDBSettings connectionToDbSettings = ConnectionServise.GetConnectionSettings();
        Host = connectionToDbSettings.Host;
        User =  connectionToDbSettings.User;
        Password = connectionToDbSettings.Password;
        Database = connectionToDbSettings.Database;
        TestConnection();

        InitializeComponent();
        Theme = new ObservableCollection<ThemeSettings>
            {
                new ThemeSettings("Светлая"),
                new ThemeSettings("Тёмная"),
            };
        ThemeSelected = SettingsUIFileParser.GetTheme() == "Тёмная" ? Theme[1] : Theme[0];
    }
}
