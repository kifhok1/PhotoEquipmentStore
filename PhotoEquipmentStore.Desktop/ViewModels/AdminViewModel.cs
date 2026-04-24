using System;
using ReactiveUI;
using System.Reactive;

namespace PhotoEquipmentStore.ViewModels;

public class AdminViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    
    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
    
    public AdminViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LogoutCommand = ReactiveCommand.Create(Logout);
    }
    
    private void Logout()
    {
        _mainViewModel.GoToLoginCommand.Execute().Subscribe();
    }

}