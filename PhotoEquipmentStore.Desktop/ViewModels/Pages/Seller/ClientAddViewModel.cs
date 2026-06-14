using System;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;/// <summary>
/// ViewModel создания и редактирования клиента.
/// </summary>


public class ClientAddViewModel : ViewModelBase
{
    private readonly Action _goBack;
    private readonly ClientShow? _editItem;
    private readonly ClientsService _clientsService = new ClientsService();

    /// <summary>

    /// Признак режима редактирования существующей записи.

    /// </summary>

    public bool IsEdit => _editItem is not null;
    /// <summary>
    /// Заголовок страницы формы.
    /// </summary>
    public string PageTitle => IsEdit ? "Редактировать клиента" : "Добавить клиента";

    private string _fullName = string.Empty;
    /// <summary>
    /// Полное имя клиента.
    /// </summary>
    public string FullName
    {
        get => _fullName;
        set => this.RaiseAndSetIfChanged(ref _fullName, value);
    }

    private string _phoneNumber = string.Empty;
    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
    }

    private string _fullNameError = string.Empty;
    /// <summary>
    /// Текст ошибки валидации ФИО клиента.
    /// </summary>
    public string FullNameError
    {
        get => _fullNameError;
        private set => this.RaiseAndSetIfChanged(ref _fullNameError, value);
    }

    /// <summary>

    /// Команда сохранения записи.

    /// </summary>

    public ReactiveCommand<Unit, Unit> SaveCommand  { get; }
    /// <summary>
    /// Команда сброса фильтров и поиска.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    public ClientAddViewModel(Action goBack, ClientShow? editItem = null)
    {
        _goBack   = goBack;
        _editItem = editItem;

        if (editItem is not null)
        {
            _fullName    = editItem.Name;
            _phoneNumber = editItem.PhoneNumber;
        }

        var canSave = this.WhenAnyValue(
            x => x.FullName,
            x => x.PhoneNumber,
            (name, phone) => IsValidPartialName(name) && IsValidPhone(phone));

        SaveCommand = ReactiveCommand.Create(Save, canSave);

        ResetCommand = ReactiveCommand.Create(IsEdit ? _goBack : Reset);

        this.WhenAnyValue(x => x.FullName)
            .Subscribe(v =>
            {
                if (!string.IsNullOrEmpty(v))
                {
                    var cap = string.Join(' ', v.Split(' ')
                        .Select(w => w.Length > 0
                            ? char.ToUpperInvariant(w[0]) + w[1..]
                            : w));
                    if (cap != v) { FullName = cap; return; }
                }

                FullNameError = string.IsNullOrEmpty(v) || IsValidPartialName(v)
                    ? string.Empty
                    : "Минимум два слова, каждое с заглавной русской буквы";
            });
    }

    private void Reset()
    {
        FullName    = string.Empty;
        PhoneNumber = string.Empty;
    }

    public ClientAddViewModel() : this(() => { }) { }

    private async void Save()
    {
        if (IsEdit)
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Редактировать запись?",
                $"Вы действительно хотите изменить данные клиента? Это действие нельзя будет отменить.");

            if (confirmed)
            {
                var clientsDb = _clientsService.UpdateClient(new Client
                (
                    _editItem!.Id,
                    FullName,
                    PhoneNumber
                ));
                if (clientsDb.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync("Успешно", $"Данные клиента - {FullName} изменены.");
                    _goBack();
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось редакировать данные клиента. {clientsDb.ErrorMessage}");
                }
            }
        }
        else
        {
             bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Создать запись?",
                $"Вы действительно хотите создать данные клиента?");

            if (confirmed)
            {
                var clientsDb = _clientsService.CreateClient(new Client
                (
                    FullName,
                    PhoneNumber
                ));
                if (clientsDb.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync("Успешно", $"Клиент - {FullName} создан.");
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось добавить клиента. {clientsDb.ErrorMessage}");
                }
            }
        }
    }

    private static bool IsValidPartialName(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return false;
        var words = v.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length >= 2 &&
               Array.TrueForAll(words, w => Regex.IsMatch(w, @"^[А-ЯЁ][а-яё]+$"));
    }

    private static bool IsValidPhone(string? v) =>
        !string.IsNullOrEmpty(v) &&
        Regex.IsMatch(v, @"^\+7\(\d{3}\) \d{3}-\d{2}-\d{2}$");
}
