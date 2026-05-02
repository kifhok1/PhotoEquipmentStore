using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using ReactiveUI.Validation.Helpers;

namespace PhotoEquipmentStore.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainViewModel mainViewModel;
    private string login;
    private string password;
    private string errorMessage;
    private bool errorVisible;
    
    public Interaction<Unit, Unit> Close { get; } = new Interaction<Unit, Unit>();
     
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public string LoginText
    {
        get => login;
        set => this.RaiseAndSetIfChanged(ref login, value);
    }

    public string PasswordText
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }

    public bool ErrorVisible
    {
        get => errorVisible;
        set => this.RaiseAndSetIfChanged(ref errorVisible, value);
    }
    
    public LoginViewModel(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel; 
        ErrorVisible = false;
        //Команда авторизации
        LoginCommand = ReactiveCommand.Create(Login);
        
        // Команда для закрытия окна авторизации
        CloseCommand = ReactiveCommand.CreateFromTask(async () => await Close.Handle(Unit.Default));
    }
    
    // Конструктор для дизайнера
    [Obsolete("Design-time only")]
    public LoginViewModel()
    {
        ErrorVisible = true;
        ErrorMessage = "Неверный логин или пароль";
        // Инициализируем команду закрытия для дизайнера (без MainViewModel)
        CloseCommand = ReactiveCommand.CreateFromTask(async () => await Close.Handle(Unit.Default));
    }
    
    private void Login()
    {
        // Здесь логика входа
        // После успешной авторизации переходим на форму в зависимости от роли пользователя
        var result = AuthorizationService.Authenticate(LoginText, PasswordText);

        if (!result.IsSuccess)
        {
            ErrorVisible = true;
            ErrorMessage = result.ErrorMessage;
            return;
        }

        string name = result.User.Name;
        string role = result.User.RoleName;
        Bitmap userImage = BitmapHelper.FromBytes(result.User.UserImage);
        UserInfo userInfo = new UserInfo(name, role, userImage);
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
}