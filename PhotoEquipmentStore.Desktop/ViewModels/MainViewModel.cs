using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using PhotoEquipmentStore.ViewModels.Notification;
using PhotoEquipmentStore.Views;

namespace PhotoEquipmentStore.ViewModels;/// <summary>
/// Корневая ViewModel приложения: навигация между ролями и глобальные уведомления.
/// </summary>


public class MainViewModel : ViewModelBase
{
    private NotificationViewModel? _notification;
    /// <summary>
    /// Текущее активное уведомление или null, если диалог скрыт.
    /// </summary>
    public NotificationViewModel? Notification
    {
        get => _notification;
        private set
        {
            this.RaiseAndSetIfChanged(ref _notification, value);
            this.RaisePropertyChanged(nameof(IsNotificationVisible));
        }
    }

    /// <summary>

    /// Признак видимости блока уведомления.

    /// </summary>

    public bool IsNotificationVisible => _notification is not null;

    private ViewModelBase _currentViewModel;
    private UserInfo userInfo;

    private bool isBlocked;
    /// <summary>
    /// Блокировка главного окна (например, при ошибке капчи).
    /// </summary>
    public bool IsBlocked
    {
        get => isBlocked;
        set => this.RaiseAndSetIfChanged(ref isBlocked, value);
    }

    /// <summary>

    /// Информация о текущем пользователе раздела.

    /// </summary>

    public UserInfo CurrentUser
    {
        get => this.userInfo;
        set => this.RaiseAndSetIfChanged(ref this.userInfo, value);
    }

    /// <summary>

    /// Активная дочерняя ViewModel (экран входа или раздел роли).

    /// </summary>

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    /// <summary>

    /// Команда перехода на экран входа.

    /// </summary>

    public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }
    /// <summary>
    /// Команда перехода в раздел администратора.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoToAdminCommand { get; }
    /// <summary>
    /// Команда перехода в раздел продавца.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoToSellerCommand { get; }
    /// <summary>
    /// Команда перехода в раздел менеджера.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoToManagerCommand { get; }
    /// <summary>
    /// Команда перехода в раздел системного пользователя.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoToRootCommand { get; }

    public MainViewModel()
    {

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
