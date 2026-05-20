using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

public class LatinInputBehavior : Behavior<TextBox>
{
    private static readonly Regex Allowed =
        new(@"^[a-zA-Z0-9!@#$%^&*()\-_=+\[\]{};':"",./<>?\\|`~]+$", RegexOptions.Compiled);

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
        if (!string.IsNullOrEmpty(e.Text) && !Allowed.IsMatch(e.Text))
            e.Handled = true;
    }
}