using Avalonia;
using Avalonia.Controls;

namespace PhotoEquipmentStore.Helper;/// <summary>
/// Прикрепляемое свойство сообщения об ошибке валидации для <see cref="TextBox"/>.
/// </summary>


public class InputValidation
{
    public static readonly AttachedProperty<string> InputErrorProperty =
        AvaloniaProperty.RegisterAttached<InputValidation, TextBox, string>(
            "InputError", string.Empty);

    /// <summary>

    /// Возвращает текст ошибки валидации для текстового поля.

    /// </summary>

    public static string GetInputError(TextBox element) =>
        element.GetValue(InputErrorProperty);

    /// <summary>

    /// Устанавливает текст ошибки валидации для текстового поля.

    /// </summary>

    public static void SetInputError(TextBox element, string value) =>
        element.SetValue(InputErrorProperty, value);
}
