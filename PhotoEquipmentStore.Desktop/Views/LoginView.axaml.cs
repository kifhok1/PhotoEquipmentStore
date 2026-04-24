using System.Reactive;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using PhotoEquipmentStore.ViewModels;
using ReactiveUI;

namespace PhotoEquipmentStore.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            // Обработчик для закрытия окна
            d(ViewModel.Close.RegisterHandler(interaction =>
            {
                interaction.SetOutput(Unit.Default);
                var window = this.FindAncestorOfType<Window>();
                window?.Close();
            }));
        });
    }
}
