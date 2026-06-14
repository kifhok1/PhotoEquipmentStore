using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.Helper;

namespace PhotoEquipmentStore.Behaviors;

/// <summary>
/// Поведение числового ввода: разрешает только цифры в <see cref="TextBox"/>.
/// Обрабатывает событие <see cref="InputElement.TextInputEvent"/>.
/// </summary>
public class NumericInputBehavior : Behavior<TextBox>
{
    private const string ErrorText = "Только цифры";

    /// <summary>
    /// Подписывается на ввод текста в туннельном режиме.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.AddHandler(
            InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Отписывается от обработчика ввода текста.
    /// </summary>
    protected override void OnDetaching()
    {
        AssociatedObject!.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
        base.OnDetaching();
    }

    /// <summary>
    /// Блокирует нецифровые символы и обновляет сообщение об ошибке.
    /// </summary>
    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text)) return;

        foreach (var c in e.Text)
        {
            if (!char.IsDigit(c))
            {
                e.Handled = true;
                InputValidation.SetInputError(AssociatedObject!, ErrorText);
                return;
            }
        }

        InputValidation.SetInputError(AssociatedObject!, string.Empty);
    }
}
