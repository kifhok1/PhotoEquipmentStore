using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.Helper;

namespace PhotoEquipmentStore.Behaviors;

/// <summary>
/// Поведение ввода названия товара: буквы, цифры, пробелы и спецсимволы.
/// Обрабатывает событие <see cref="InputElement.TextInputEvent"/>.
/// </summary>
public class ProductNameInputBehavior : Behavior<TextBox>
{

    private static readonly Regex Allowed =
        new(@"^[а-яёА-ЯЁa-zA-Z0-9 !@#$%^&*()\-_=+\[\]{};':"",./<>?\\|`~.]+$",
            RegexOptions.Compiled);

    private const string ErrorText = "Только буквы, цифры, пробелы и спецсимволы";

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.AddHandler(
            InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
        base.OnDetaching();
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text)) return;

        if (!Allowed.IsMatch(e.Text))
        {
            e.Handled = true;
            InputValidation.SetInputError(AssociatedObject!, ErrorText);
        }
        else
        {
            InputValidation.SetInputError(AssociatedObject!, string.Empty);
        }
    }
}
