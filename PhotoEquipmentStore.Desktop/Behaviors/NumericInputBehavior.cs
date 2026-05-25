using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.Helper;

namespace PhotoEquipmentStore.Behaviors;

public class NumericInputBehavior : Behavior<TextBox>
{
    private const string ErrorText = "Только цифры";

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