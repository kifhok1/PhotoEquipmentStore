using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using PhotoEquipmentStore.ViewModels.Notification;
using PhotoEquipmentStore.Views;

namespace PhotoEquipmentStore.ViewModels;

public class MainViewModel : ViewModelBase
{
    private NotificationViewModel? _notification;
    public NotificationViewModel? Notification
    {
        get => _notification;
        private set
        {
            this.RaiseAndSetIfChanged(ref _notification, value);
            this.RaisePropertyChanged(nameof(IsNotificationVisible));
        }
    }

    public bool IsNotificationVisible => _notification is not null;
    
    private ViewModelBase _currentViewModel;
    private UserInfo userInfo;

    private bool isBlocked;
    public bool IsBlocked
    {
        get => isBlocked;
        set => this.RaiseAndSetIfChanged(ref isBlocked, value);
    }

    public UserInfo CurrentUser
    {
        get => this.userInfo;
        set => this.RaiseAndSetIfChanged(ref this.userInfo, value);
    }
    
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    } 
    
    // Команды для навигации
    public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToAdminCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToSellerCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToManagerCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRootCommand { get; }
    
    public MainViewModel()
    {
        // Устанавливаем начальное представление
        _currentViewModel = new LoginViewModel(this);
        
        NotificationService.Instance.Notifications
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(vm => Notification = vm);
        
        GoToLoginCommand = ReactiveCommand.Create(GoToLogin);
        GoToAdminCommand = ReactiveCommand.Create(GoToAdmin);
        GoToManagerCommand = ReactiveCommand.Create(GoToManager);
        GoToSellerCommand = ReactiveCommand.Create(GoToSeller);
        GoToRootCommand = ReactiveCommand.Create(GoToRoot);
    }
    
    private void GoToLogin()
    {
        CurrentViewModel = new LoginViewModel(this);
    }
    
    private void GoToAdmin()
    {
        CurrentViewModel = new AdminViewModel(this, userInfo);
    }
    
    private void GoToSeller()
    {
        CurrentViewModel = new SellerViewModel(this, userInfo);
    }
    
    private void GoToManager()
    {
        CurrentViewModel = new ManagerViewModel(this, userInfo);
    }
    
    private void GoToRoot()
    {
        CurrentViewModel = new RootUserViewModel(this, userInfo);
    }
}