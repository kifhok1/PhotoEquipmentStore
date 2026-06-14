using System;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.Helper;

namespace PhotoEquipmentStore.Behaviors;

public class PhoneMaskBehavior : Behavior<TextBox>
{
    private const string ErrorText = "Только цифры";

    protected override void OnAttached()
    {
        base.OnAttached();
        var tb = AssociatedObject!;
        tb.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
        tb.KeyDown   += OnKeyDown;
        tb.GotFocus  += OnGotFocus;
        tb.LostFocus += OnLostFocus;
    }

    protected override void OnDetaching()
    {
        var tb = AssociatedObject!;
        tb.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
        tb.KeyDown   -= OnKeyDown;
        tb.GotFocus  -= OnGotFocus;
        tb.LostFocus -= OnLostFocus;
        base.OnDetaching();
    }

    private void OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        var tb = AssociatedObject!;
        if (string.IsNullOrEmpty(tb.Text))
        {
            tb.Text = "+7(";
            tb.CaretIndex = 3;
        }
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var tb = AssociatedObject!;
        if (tb.Text == "+7(")
            tb.Text = string.Empty;
        InputValidation.SetInputError(tb, string.Empty);
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        e.Handled = true;
        var tb = AssociatedObject!;
        var input = e.Text ?? "";

        var incoming = new string(input.Where(char.IsDigit).ToArray());

        if (string.IsNullOrEmpty(incoming) && input.Length > 0)
        {
            InputValidation.SetInputError(tb, ErrorText);
            return;
        }

        InputValidation.SetInputError(tb, string.Empty);

        var raw = tb.Text ?? "";
        if (!raw.StartsWith("+7(")) raw = "+7(";

        var digits = ExtractDigits(raw) + incoming;
        if (digits.Length > 10) digits = digits[..10];

        tb.Text = FormatPhone(digits);
        tb.CaretIndex = tb.Text.Length;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Back) return;
        e.Handled = true;

        var tb = AssociatedObject!;
        InputValidation.SetInputError(tb, string.Empty);

        var digits = ExtractDigits(tb.Text ?? "");
        if (digits.Length == 0) return;

        digits = digits[..^1];
        tb.Text = digits.Length == 0 ? "+7(" : FormatPhone(digits);
        tb.CaretIndex = tb.Text?.Length ?? 0;
    }

    private static string ExtractDigits(string text) =>
        text.Length <= 3
            ? string.Empty
            : new string(text[3..].Where(char.IsDigit).ToArray());

    private static string FormatPhone(string digits)
    {
        var sb = new StringBuilder("+7(");
        for (int i = 0; i < Math.Min(digits.Length, 10); i++)
        {
            sb.Append(digits[i]);
            switch (i)
            {
                case 2: sb.Append(") "); break;
                case 5: sb.Append('-');  break;
                case 7: sb.Append('-');  break;
            }
        }
        return sb.ToString();
    }
}
