using System;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class ClientAddViewModel : ViewModelBase
{
    private readonly Action _goBack;
    private readonly ClientShow? _editItem;

    public bool IsEdit => _editItem is not null;
    public string PageTitle => IsEdit ? "Редактировать клиента" : "Добавить клиента";

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set => this.RaiseAndSetIfChanged(ref _fullName, value);
    }

    private string _phoneNumber = string.Empty;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
    }

    private string _fullNameError = string.Empty;
    public string FullNameError
    {
        get => _fullNameError;
        private set => this.RaiseAndSetIfChanged(ref _fullNameError, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand  { get; }
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

        // Добавление → очистить поля, Редактирование → вернуться на список
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

    // Конструктор для дизайнера
    public ClientAddViewModel() : this(() => { }) { }

    private void Save()
    {
        if (IsEdit)
        {
            // TODO: обновить _editItem через сервис
        }
        else
        {
            // TODO: создать клиента через сервис
        }
        _goBack();
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