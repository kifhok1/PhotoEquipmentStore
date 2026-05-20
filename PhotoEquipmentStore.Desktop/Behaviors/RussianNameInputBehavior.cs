using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

public class RussianNameInputBehavior: Behavior<TextBox>
{
    private static readonly Regex RussianOrSpace =
        new(@"^[а-яёА-ЯЁ ]+$", RegexOptions.Compiled);

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

        // Только русские буквы и пробел
        if (!RussianOrSpace.IsMatch(e.Text))
        {
            e.Handled = true;
            return;
        }

        var tb = AssociatedObject!;
        var current = tb.Text ?? "";

        // Запрет двойного пробела
        if (e.Text == " " && (string.IsNullOrEmpty(current) || current.EndsWith(" ")))
        {
            e.Handled = true;
            return;
        }

        // Не более 3 слов
        var future = current.Insert(Math.Clamp(tb.CaretIndex, 0, current.Length), e.Text);
        var wordCount = future.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        if (wordCount > 3)
        {
            e.Handled = true;
        }
    }

}