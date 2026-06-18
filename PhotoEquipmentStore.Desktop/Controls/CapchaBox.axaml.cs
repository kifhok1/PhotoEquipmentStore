using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace PhotoEquipmentStore.Controls;

/// <summary>
/// Пользовательский элемент ввода и проверки CAPTCHA при неудачной авторизации.
/// </summary>


public partial class CapchaBox : UserControl
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRTUVWXYZ2346789";
    private const int CaptchaLength = 6;
    private const double CooldownSeconds = 10.0;
    private const int TickMs = 50;

    private string _captchaText = string.Empty;
    private DispatcherTimer? _cooldownTimer;
    private double _cooldownRemaining;

    public static readonly StyledProperty<string> ErrorMessageProperty =
        AvaloniaProperty.Register<CapchaBox, string>(
            nameof(ErrorMessage),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>

    /// Текст ошибки CAPTCHA.

    /// </summary>

    public string ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public static readonly StyledProperty<bool> ErrorVisibleProperty =
        AvaloniaProperty.Register<CapchaBox, bool>(nameof(ErrorVisible));

    /// <summary>

    /// Признак видимости сообщения об ошибке.

    /// </summary>

    public bool ErrorVisible
    {
        get => GetValue(ErrorVisibleProperty);
        set => SetValue(ErrorVisibleProperty, value);
    }

    public static readonly StyledProperty<double> CooldownProgressProperty =
        AvaloniaProperty.Register<CapchaBox, double>(nameof(CooldownProgress));

    /// <summary>

    /// Прогресс таймера блокировки после неверной CAPTCHA.

    /// </summary>

    public double CooldownProgress
    {
        get => GetValue(CooldownProgressProperty);
        set => SetValue(CooldownProgressProperty, value);
    }

    public static readonly StyledProperty<bool> WindowBlockedProperty =
        AvaloniaProperty.Register<CapchaBox, bool>(
            nameof(WindowBlocked),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>

    /// Блокировка окна на время ожидания.

    /// </summary>

    public bool WindowBlocked
    {
        get => GetValue(WindowBlockedProperty);
        set => SetValue(WindowBlockedProperty, value);
    }

    public static readonly StyledProperty<bool> ShowCapchaBlockProperty =
        AvaloniaProperty.Register<CapchaBox, bool>(
            nameof(ShowCapchaBlock),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>

    /// Признак отображения блока CAPTCHA.

    /// </summary>

    public bool ShowCapchaBlock
    {
        get => GetValue(ShowCapchaBlockProperty);
        set => SetValue(ShowCapchaBlockProperty, value);
    }

    private bool canConfirm;

    /// <summary>

    /// Разрешено ли подтверждение CAPTCHA.

    /// </summary>

    public bool CanConfirm
    {
        get => canConfirm;
        set => canConfirm = value;
    }

    public CapchaBox()
    {
        InitializeComponent();
        if (Design.IsDesignMode)
        {
            ErrorVisible = true;
            ErrorMessage = "Неверный логин или пароль!";
        }
        GenerateCaptcha();
    }

    private void GenerateCaptcha()
    {
        _captchaText = BuildRandomText(CaptchaLength);
        CaptchaRenderer.SetCaptcha(_captchaText);
        InputBox.Text = string.Empty;
        InputBox.Focus();
    }

    private static string BuildRandomText(int length)
    {
        var rng = Random.Shared;
        return string.Create(length, rng, static (span, r) =>
        {
            for (int i = 0; i < span.Length; i++)
                span[i] = Alphabet[r.Next(Alphabet.Length)];
        });
    }

    private bool Verify(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return string.Equals(input.Trim(), _captchaText, StringComparison.OrdinalIgnoreCase);
    }

    private void RefreshButton_Click(object? sender, RoutedEventArgs e)
        => GenerateCaptcha();

    [RelayCommand]
    private void Confirm()
    {
        if (Verify(InputBox.Text))
        {
            ErrorVisible = false;
            ShowCapchaBlock = false;
            InputBox.Text = string.Empty;
            GenerateCaptcha();
        }
        else
        {
            ErrorVisible = true;
            ErrorMessage = "Ошибка ввода капчи, повторите попытку";
            GenerateCaptcha();
            WindowBlocked = true;
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        _cooldownRemaining = CooldownSeconds;
        CooldownProgress = 10000;
        CanConfirm = false;

        _cooldownTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(TickMs)
        };
        _cooldownTimer.Tick += OnCooldownTick;
        _cooldownTimer.Start();
    }

    private void OnCooldownTick(object? sender, EventArgs e)
    {
        _cooldownRemaining -= TickMs / 1000.0;

        CooldownProgress = Math.Max(0, _cooldownRemaining / CooldownSeconds * 10000);

        if (_cooldownRemaining <= 0)
        {
            if (_cooldownTimer is null) return;

            _cooldownTimer.Stop();
            _cooldownTimer.Tick -= OnCooldownTick;
            _cooldownTimer = null;

            CooldownProgress = 0;
            CanConfirm = true;

            WindowBlocked = false;
        }
    }
}
