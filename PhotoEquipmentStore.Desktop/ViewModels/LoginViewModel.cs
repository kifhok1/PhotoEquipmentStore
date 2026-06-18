using System;
using System.IO;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.SettingsUI;

namespace PhotoEquipmentStore.ViewModels;/// <summary>
/// ViewModel экрана входа: авторизация, капча и настройки.
/// </summary>


public partial class LoginViewModel : ViewModelBase
{
    private readonly MainViewModel mainViewModel;
    private string login;
    private string password;
    private string errorMessage;
    private bool errorVisible;
    private bool capchaVisible = false;
    private bool settingsVisible = false;
    private Bitmap imageLoginForm;
    private bool errorConnection = false;

    /// <summary>

    /// Interaction для закрытия приложения.

    /// </summary>

    public Interaction<Unit, Unit> Close { get; } = new Interaction<Unit, Unit>();

    /// <summary>

    /// Команда закрытия панели настроек.

    /// </summary>

    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
    /// <summary>
    /// Команда входа в систему.
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    /// <summary>

    /// Команда открытия панели настроек.

    /// </summary>

    public ReactiveCommand<Unit, Unit> ShowSettingsCommand { get; }
    /// <summary>
    /// Команда закрытия панели настроек.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CloseSettingsCommand { get; }

    /// <summary>

    /// Введённый логин.

    /// </summary>

    public string LoginText
    {
        get => login;
        set => this.RaiseAndSetIfChanged(ref login, value);
    }

    /// <summary>

    /// Введённый пароль.

    /// </summary>

    public string PasswordText
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    /// <summary>

    /// Текст ошибки CAPTCHA.

    /// </summary>

    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }

    /// <summary>

    /// Признак видимости сообщения об ошибке.

    /// </summary>

    public bool ErrorVisible
    {
        get => errorVisible;
        set => this.RaiseAndSetIfChanged(ref errorVisible, value);
    }

    /// <summary>

    /// Признак отображения блока CAPTCHA.

    /// </summary>

    public bool CapchaVisible
    {
        get => capchaVisible;
        set => this.RaiseAndSetIfChanged(ref capchaVisible, value);
    }

    /// <summary>

    /// Признак отображения панели настроек.

    /// </summary>

    public bool SettingsVisible
    {
        get => settingsVisible;
        set => this.RaiseAndSetIfChanged(ref settingsVisible, value);
    }

    /// <summary>

    /// Блокировка окна на время ожидания.

    /// </summary>

    public bool WindowBlocked
    {
        get => mainViewModel.IsBlocked;
        set => mainViewModel.IsBlocked = value;
    }

    /// <summary>

    /// Фоновое изображение формы входа в настройках.

    /// </summary>

    public Bitmap ImageLoginForm
    {
        get => imageLoginForm;
        set => this.RaiseAndSetIfChanged(ref imageLoginForm, value);
    }

    /// <summary>

    /// Признак ошибки подключения к базе данных.

    /// </summary>

    public bool ErrorConnection
    {
        get => errorConnection;
        set => this.RaiseAndSetIfChanged(ref errorConnection, value);
    }

    public LoginViewModel(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel;
        ErrorVisible = false;

        LoginCommand = ReactiveCommand.Create(Login);

        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            try
            {
                var folder   = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "ФотоМагазин", "Файлы базы данных");
                var fileName = $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
                var filePath = Path.Combine(folder, fileName);

                var service = new DatabaseService();
                await service.CreateBackupAsync(filePath);
            }
            catch
            {

            }
            return await Close.Handle(Unit.Default);
        });

        ShowSettingsCommand = ReactiveCommand.Create(ShowSettings);
        CloseSettingsCommand = ReactiveCommand.Create(CloseSettings);

        var basePath = AppContext.BaseDirectory;
        if (SettingsUIFileParser.GetTheme() == "Тёмная")
            ImageLoginForm = new Bitmap(Path.Combine(basePath, "Assets", "login-background-dark.jpg"));
        else
            ImageLoginForm = new Bitmap(Path.Combine(basePath, "Assets", "login-background-light.png"));

    }

    [Obsolete("Design-time only")]
    public LoginViewModel()
    {
        ErrorVisible = true;
        ErrorMessage = "Неверный логин или пароль";

        CloseCommand = ReactiveCommand.CreateFromTask(async () => await Close.Handle(Unit.Default));

        ShowSettingsCommand = ReactiveCommand.Create(ShowSettings);
        CloseSettingsCommand = ReactiveCommand.Create(CloseSettings);

        var basePath = AppContext.BaseDirectory;
        if (SettingsUIFileParser.GetTheme() == "Тёмная")
            ImageLoginForm = new Bitmap(Path.Combine(basePath, "Assets", "login-background-dark.jpg"));
        else
            ImageLoginForm = new Bitmap(Path.Combine(basePath, "Assets", "login-background-light.png"));

    }

    private void Login()
    {
        if (LoginText == "root" && PasswordText == "root")
        {
            UserInfo userRoot = new UserInfo(0, "Системный пользователь", "Администратор", null);
            mainViewModel.CurrentUser = userRoot;
            mainViewModel.GoToRootCommand.Execute().Subscribe();
            return;
        }

        var result = AuthorizationService.Authenticate(LoginText, PasswordText);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage == "Ошибка подключения")
            {
                ErrorConnection = true;
                return;
            }
            else
            {
                ErrorVisible = true;
                ErrorMessage = result.ErrorMessage;
                CapchaVisible = true;
                return;
            }
        }

        int userID = result.User.Id;
        string name = result.User.Name;
        string role = result.User.RoleName;
        Bitmap userImage = BitmapHelper.FromBytes(result.User.UserImage);
        UserInfo userInfo = new UserInfo(userID, name, role, userImage);
        mainViewModel.CurrentUser = userInfo;
        switch (result.User!.RoleId)
        {
            case 1:
                mainViewModel.GoToAdminCommand.Execute().Subscribe();
                break;
            case 2:
                mainViewModel.GoToManagerCommand.Execute().Subscribe();
                break;
            case 3:
                mainViewModel.GoToSellerCommand.Execute().Subscribe();
                break;
        }
    }

    private void ShowSettings()
    {
        SettingsVisible = true;
    }

    private void CloseSettings()
    {
        SettingsVisible = false;
    }
}
