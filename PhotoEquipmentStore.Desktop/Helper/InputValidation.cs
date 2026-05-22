using Avalonia;
using Avalonia.Controls;

namespace PhotoEquipmentStore.Helper;

public class InputValidation
{
    public static readonly AttachedProperty<string> InputErrorProperty =
        AvaloniaProperty.RegisterAttached<InputValidation, TextBox, string>(
            "InputError", string.Empty);

    public static string GetInputError(TextBox element) =>
        element.GetValue(InputErrorProperty);

    public static void SetInputError(TextBox element, string value) =>
        element.SetValue(InputErrorProperty, value);
}