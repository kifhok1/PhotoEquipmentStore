using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using PhotoEquipmentStore.Application.Services;

namespace PhotoEquipmentStore.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainViewModel mainViewModel;
    private string login;
    private string password;
    
    public Interaction<Unit, Unit> Close { get; } = new Interaction<Unit, Unit>();
     
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public string LoginText
    {
        get => login;
        set => login = value;
    }

    public string PasswordText
    {
        get => password;
        set => password = value;
    }
    
    public LoginViewModel(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel; 
        //Команда авторизации
        LoginCommand = ReactiveCommand.Create(Login);
        
        // Команда для закрытия окна авторизации
        CloseCommand = ReactiveCommand.CreateFromTask(async () => await Close.Handle(Unit.Default));
    }
    
    // Конструктор для дизайнера
    [Obsolete("Design-time only")]
    public LoginViewModel()
    {
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
            // TODO: показать result.ErrorMessage пользователю
            return;
        }

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