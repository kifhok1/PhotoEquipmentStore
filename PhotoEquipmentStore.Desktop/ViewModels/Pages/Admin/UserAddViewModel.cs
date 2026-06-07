using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public partial class UserAddViewModel : ViewModelBase, IValidatableViewModel
{
    public IValidationContext ValidationContext { get; } = new ValidationContext();
    private ReferenceService _referenceService = new ReferenceService();
    private UsersService     _usersService     = new UsersService();

    private ValidationHelper _userNameValidation = null!;
    private ValidationHelper _phoneValidation    = null!;
    private ValidationHelper _loginValidation    = null!;
    private ValidationHelper _passwordValidation = null!;
    private ValidationHelper _roleValidation     = null!;

    private readonly Action    _goBack;
    private readonly UserShow? _editItem;

    public bool   IsEdit    => _editItem is not null;
    public string PageTitle => IsEdit ? "Редактировать пользователя" : "Создать пользователя";

    // ── Свойства ──────────────────────────────────────────────────────────────

    private string _userName = string.Empty;
    public string UserName
    {
        get => _userName;
        set => this.RaiseAndSetIfChanged(ref _userName, value);
    }

    private string _phoneNumber = string.Empty;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
    }

    private string _login = string.Empty;
    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    private ReferenceShow? _selectedRole;
    public ReferenceShow? SelectedRole
    {
        get => _selectedRole;
        set => this.RaiseAndSetIfChanged(ref _selectedRole, value);
    }

    private Bitmap? _userImage;
    public Bitmap? UserImage
    {
        get => _userImage;
        set => this.RaiseAndSetIfChanged(ref _userImage, value);
    }

    private string _userNameError = string.Empty;
    public string UserNameError
    {
        get => _userNameError;
        private set => this.RaiseAndSetIfChanged(ref _userNameError, value);
    }

    public ObservableCollection<ReferenceShow> Roles { get; } = new();

    // ── Команды ───────────────────────────────────────────────────────────────

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    // ── Конструктор ───────────────────────────────────────────────────────────

    public UserAddViewModel(Action goBack, UserShow? editItem = null)
    {
        _goBack   = goBack;
        _editItem = editItem;

        var userRoles = _referenceService.GetRoles().References;
        foreach (var role in userRoles)
            Roles.Add(new ReferenceShow(role.Id, role.Name, role.Count, role.IsDeleted));

        if (editItem is not null)
        {
            _userName    = editItem.Name;
            _phoneNumber = editItem.PhoneNumber;
            _login       = editItem.Login;
            _selectedRole = Roles.FirstOrDefault(r => r.Id == editItem.RoleID);
            UserImage    = editItem.Image;
        }

        var canSave = this.WhenAnyValue(
            x => x.UserName,
            x => x.PhoneNumber,
            x => x.Login,
            x => x.Password,
            x => x.SelectedRole,
            (name, phone, login, pass, role) =>
                IsValidPartialName(name)                                               &&
                IsValidPhone(phone)                                                    &&
                !string.IsNullOrWhiteSpace(login)                                     &&
                (IsEdit || (!string.IsNullOrWhiteSpace(pass) && pass.Length >= 6))    &&
                (string.IsNullOrWhiteSpace(pass) || IsValidLatinText(pass) && pass.Length >= 6) &&
                role != null);

        SaveCommand = ReactiveCommand.Create(Save, canSave);

        SetupValidation();
        SubscribeToChanges();
    }

    public UserAddViewModel() : this(() => { }) { }

    // ── Валидация ─────────────────────────────────────────────────────────────

    private void SetupValidation()
    {
        _userNameValidation = this.ValidationRule(
            vm => vm.UserName,
            v => !string.IsNullOrWhiteSpace(v) && IsValidFullName(v),
            "ФИО: три слова, каждое с заглавной русской буквы");

        _phoneValidation = this.ValidationRule(
            vm => vm.PhoneNumber,
            v => !string.IsNullOrEmpty(v) && IsValidPhone(v),
            "Формат: +7(XXX) XXX-XX-XX");

        _loginValidation = this.ValidationRule(
            vm => vm.Login,
            v => !string.IsNullOrEmpty(v) && IsValidLatinText(v),
            "Только латиница, цифры и спецсимволы");

        _passwordValidation = this.ValidationRule(
            vm => vm.Password,
            v => IsEdit
                ? string.IsNullOrEmpty(v) || (IsValidLatinText(v) && v.Length >= 6)
                : !string.IsNullOrEmpty(v) && IsValidLatinText(v) && v.Length >= 6,
            "Латиница, цифры, спецсимволы — минимум 6 символов");

        _roleValidation = this.ValidationRule(
            vm => vm.SelectedRole,
            r => r != null,
            "Выберите роль пользователя");
    }

    private void SubscribeToChanges()
    {
        this.WhenAnyValue(x => x.UserName)
            .Subscribe(v =>
            {
                if (!string.IsNullOrEmpty(v))
                {
                    var cap = string.Join(' ', v.Split(' ')
                        .Select(w => w.Length > 0
                            ? char.ToUpperInvariant(w[0]) + w[1..]
                            : w));
                    if (cap != v) { UserName = cap; return; }
                }

                UserNameError = string.IsNullOrEmpty(v) || IsValidPartialName(v)
                    ? string.Empty
                    : "Минимум два слова, каждое с заглавной русской буквы";
            });
    }

    // ── Реализация команд ─────────────────────────────────────────────────────

    private async void Save()
    {
        byte[]? imageBytes = UserImage is not null
            ? BitmapHelper.ToBytes(UserImage)
            : null;

        if (IsEdit)
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Редактировать запись?",
                "Вы действительно хотите изменить данные пользователя? Это действие нельзя будет отменить.");

            if (!confirmed) return;

            try
            {
                var updatedUser = new User(
                    _editItem!.Id,
                    UserName,
                    Login,
                    PhoneNumber,
                    _editItem.Role,
                    SelectedRole!.Id,
                    imageBytes);

                UserResultDto result = string.IsNullOrWhiteSpace(Password)
                    ? _usersService.UpdateUser(updatedUser)
                    : _usersService.UpdateUser(updatedUser, Password);

                if (result.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync(
                        "Успешно", $"Данные пользователя «{UserName}» изменены.");
                    _goBack();
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync(
                        "Ошибка", $"Не удалось изменить данные пользователя. {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                await NotificationService.Instance.ShowErrorAsync("Ошибка", ex.Message);
            }
        }
        else
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Создать запись?",
                "Вы действительно хотите добавить пользователя?");

            if (!confirmed) return;

            try
            {
                var newUser = new User(
                    UserName,
                    Login,
                    Password,
                    PhoneNumber,
                    SelectedRole!.Id,
                    imageBytes);

                UserResultDto result = _usersService.CreateUser(newUser, Password);

                if (result.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync(
                        "Успешно", $"Пользователь «{UserName}» добавлен.");
                    _goBack();
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync(
                        "Ошибка", $"Не удалось добавить пользователя. {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                await NotificationService.Instance.ShowErrorAsync("Ошибка", ex.Message);
            }
        }
    }

    [RelayCommand]
    private void Reset()
    {
        if (IsEdit) { _goBack(); return; }

        UserName     = string.Empty;
        PhoneNumber  = string.Empty;
        Login        = string.Empty;
        Password     = string.Empty;
        SelectedRole = null;
        UserImage    = null;
    }

    [RelayCommand]
    private void GeneratePassword()
    {
        const string lower   = "abcdefghijklmnopqrstuvwxyz";
        const string upper   = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits  = "0123456789";
        const string special = "!@#$%^&*()-_=+";
        const string all     = lower + upper + digits + special;

        var sb = new StringBuilder();
        sb.Append(lower  [RandomNumberGenerator.GetInt32(lower.Length)]);
        sb.Append(upper  [RandomNumberGenerator.GetInt32(upper.Length)]);
        sb.Append(digits [RandomNumberGenerator.GetInt32(digits.Length)]);
        sb.Append(special[RandomNumberGenerator.GetInt32(special.Length)]);
        for (int i = 0; i < 8; i++)
            sb.Append(all[RandomNumberGenerator.GetInt32(all.Length)]);

        var arr = sb.ToString().ToCharArray();
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        Password = new string(arr);
    }

    // ── Хелперы ───────────────────────────────────────────────────────────────

    private static bool IsValidPartialName(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return false;
        var words = v.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length >= 2 &&
               Array.TrueForAll(words, w => Regex.IsMatch(w, @"^[А-ЯЁ][а-яё]+$"));
    }

    private static bool IsValidFullName(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return false;
        var words = v.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length == 3 &&
               Array.TrueForAll(words, w => Regex.IsMatch(w, @"^[А-ЯЁ][а-яё]+$"));
    }

    private static bool IsValidPhone(string? v) =>
        !string.IsNullOrEmpty(v) &&
        Regex.IsMatch(v, @"^\+7\(\d{3}\) \d{3}-\d{2}-\d{2}$");

    private static bool IsValidLatinText(string? v) =>
        !string.IsNullOrEmpty(v) &&
        Regex.IsMatch(v, @"^[a-zA-Z0-9!@#$%^&*()\-_=+\[\]{};':"",./<>?\\|`~]+$");
}