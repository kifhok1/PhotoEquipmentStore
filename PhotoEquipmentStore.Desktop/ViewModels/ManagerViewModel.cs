using System;
using System.Reactive;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels;

public class ManagerViewModel : ViewModelBase
{
    
    private readonly MainViewModel _mainViewModel;
    
    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
    
    public ManagerViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);
    }
    
    private void Logout()
    {
        _mainViewModel.GoToLoginCommand.Execute().Subscribe();
    }
    
}