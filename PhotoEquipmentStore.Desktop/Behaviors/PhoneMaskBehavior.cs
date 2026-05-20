using System;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

public class PhoneMaskBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        var tb = AssociatedObject!;
        tb.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
        tb.KeyDown    += OnKeyDown;
        tb.GotFocus   += OnGotFocus;
        tb.LostFocus  += OnLostFocus;
    }

    protected override void OnDetaching()
    {
        var tb = AssociatedObject!;
        tb.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
        tb.KeyDown    -= OnKeyDown;
        tb.GotFocus   -= OnGotFocus;
        tb.LostFocus  -= OnLostFocus;
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
        if (AssociatedObject?.Text == "+7(")
            AssociatedObject.Text = string.Empty;
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        e.Handled = true;
        var tb = AssociatedObject!;

        // Принимаем только цифры (поддерживаем вставку нескольких)
        var incoming = new string((e.Text ?? "").Where(char.IsDigit).ToArray());
        if (incoming.Length == 0) return;

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
        var digits = ExtractDigits(tb.Text ?? "");
        if (digits.Length == 0) return;

        digits = digits[..^1];
        tb.Text = digits.Length == 0 ? "+7(" : FormatPhone(digits);
        tb.CaretIndex = tb.Text?.Length ?? 0;
    }

    // Вырезаем цифры после "+7(" префикса
    private static string ExtractDigits(string text) =>
        text.Length <= 3
            ? string.Empty
            : new string(text[3..].Where(char.IsDigit).ToArray());

    // Собираем маску +7(XXX) XXX-XX-XX
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