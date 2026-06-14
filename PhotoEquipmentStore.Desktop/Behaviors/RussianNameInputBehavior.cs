using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.Helper;

namespace PhotoEquipmentStore.Behaviors;

public class RussianNameInputBehavior : Behavior<TextBox>
{
    private static readonly Regex RussianOrSpace =
        new(@"^[а-яёА-ЯЁ ]+$", RegexOptions.Compiled);

    private const string ErrorText = "Только русские буквы";

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

        if (!RussianOrSpace.IsMatch(e.Text))
        {
            e.Handled = true;
            InputValidation.SetInputError(AssociatedObject!, ErrorText);
            return;
        }

        var tb = AssociatedObject!;
        var current = tb.Text ?? "";

        if (e.Text == " " && (string.IsNullOrEmpty(current) || current.EndsWith(" ")))
        {
            e.Handled = true;
            InputValidation.SetInputError(tb, ErrorText);
            return;
        }

        var future = current.Insert(Math.Clamp(tb.CaretIndex, 0, current.Length), e.Text);
        if (future.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 3)
        {
            e.Handled = true;
            InputValidation.SetInputError(tb, "Не более трёх слов");
            return;
        }

        InputValidation.SetInputError(tb, string.Empty);
    }
}
