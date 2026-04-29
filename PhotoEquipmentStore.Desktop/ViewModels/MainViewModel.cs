using ReactiveUI;
using System.Reactive;
using PhotoEquipmentStore.Views;

namespace PhotoEquipmentStore.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    
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
    
    public MainViewModel()
    {
        // Устанавливаем начальное представление
        _currentViewModel = new LoginViewModel(this);
        
        GoToLoginCommand = ReactiveCommand.Create(GoToLogin);
        GoToAdminCommand = ReactiveCommand.Create(GoToAdmin);
        GoToManagerCommand = ReactiveCommand.Create(GoToManager);
        GoToSellerCommand = ReactiveCommand.Create(GoToSeller);
    }
    
    private void GoToLogin()
    {
        CurrentViewModel = new LoginViewModel(this);
    }
    
    private void GoToAdmin()
    {
        CurrentViewModel = new AdminViewModel(this);
    }
    
    private void GoToSeller()
    {
        CurrentViewModel = new SellerViewModel(this);
    }
    
    private void GoToManager()
    {
        CurrentViewModel = new ManagerViewModel(this);
    }
}