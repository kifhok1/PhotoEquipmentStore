using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using ReactiveUI;

namespace PhotoEquipmentStore.Controls;

/// <summary>
/// Блок ввода логина и пароля на экране авторизации.
/// </summary>


public partial class LoginBox : UserControl
{
    public static readonly StyledProperty<string> LoginTextProperty =
        AvaloniaProperty.Register<LoginBox, string>(
            nameof(LoginText),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> PasswordTextProperty =
        AvaloniaProperty.Register<LoginBox, string>(
            nameof(PasswordText),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> LoginCommandProperty =
        AvaloniaProperty.Register<SidebarControl, ReactiveCommand<Unit, Unit>>(
            nameof(LoginCommand));

    public static readonly StyledProperty<bool> ErrorProperty =
        AvaloniaProperty.Register<SidebarControl, bool>(
            nameof(Error));

    /// <summary>

    /// Введённый логин.

    /// </summary>

    public string LoginText
    {
        get => GetValue(LoginTextProperty);
        set => SetValue(LoginTextProperty, value);
    }

    /// <summary>

    /// Введённый пароль.

    /// </summary>

    public string PasswordText
    {
        get => GetValue(PasswordTextProperty);
        set => SetValue(PasswordTextProperty, value);
    }

    /// <summary>

    /// Команда входа в систему.

    /// </summary>

    public ReactiveCommand<Unit, Unit> LoginCommand
    {
        get => GetValue(LoginCommandProperty);
        set => SetValue(LoginCommandProperty, value);
    }

    /// <summary>

    /// Признак ошибки на форме входа.

    /// </summary>

    public bool Error
    {
        get => GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    public LoginBox()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            LoginText = "Login";
            PasswordText = "Password";
            LoginCommand = ReactiveCommand.Create(() => { });
        }
    }
}
