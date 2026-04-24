using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace PhotoEquipmentStore.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    public Interaction<Unit, Unit> Close { get; } = new Interaction<Unit, Unit>();
     
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    
    public LoginViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
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
        // После успешной авторизации переходим в Dashboard
        _mainViewModel.GoToAdminCommand.Execute().Subscribe();
    }
}