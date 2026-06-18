using System;
using System.Reactive;
using System.Text.RegularExpressions;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;/// <summary>
/// ViewModel создания и редактирования записи справочника.
/// </summary>


public class ReferenceAddViewModel : ViewModelBase
{
    private readonly Action _goBack;
    private readonly ReferenceShow? _editItem;
    private ReferenceService _referenceService = new ReferenceService();

    /// <summary>

    /// Тип редактируемого справочника.

    /// </summary>

    public ReferenceType ReferenceType { get; }
    /// <summary>
    /// Заголовок страницы формы.
    /// </summary>
    public string PageTitle  { get; }
    /// <summary>
    /// Подпись поля ввода наименования.
    /// </summary>
    public string FieldLabel { get; }
    /// <summary>
    /// Признак режима редактирования существующей записи.
    /// </summary>
    public bool   IsEdit     => _editItem is not null;

    private string _title = string.Empty;
    /// <summary>
    /// Заголовок уведомления.
    /// </summary>
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    /// <summary>

    /// Команда сохранения записи.

    /// </summary>

    public ReactiveCommand<Unit, Unit> SaveCommand  { get; }
    /// <summary>
    /// Команда сброса фильтров и поиска.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    public ReferenceAddViewModel(Action goBack, ReferenceType type, ReferenceShow? editItem = null)
    {
        _goBack   = goBack;
        _editItem = editItem;
        ReferenceType = type;

        if (editItem is not null)
            _title = editItem.Title;

        string verb = editItem is null ? "Добавить" : "Редактировать";

        (PageTitle, FieldLabel) = type switch
        {
            ReferenceType.Role          => ($"{verb} роль",             "Наименование роли"),
            ReferenceType.Category      => ($"{verb} категорию",         "Наименование категории"),
            ReferenceType.OrderStatuses => ($"{verb} статус заказа",     "Наименование статуса"),
            ReferenceType.Manufacturers => ($"{verb} производителя",     "Наименование производителя"),
            ReferenceType.Suppliers     => ($"{verb} поставщика",        "Наименование поставщика"),
            _                           => ($"{verb} запись",            "Наименование")
        };

        var canSave = this.WhenAnyValue(
            x => x.Title,
            title => !string.IsNullOrWhiteSpace(title) && IsValidForType(title, type));

        SaveCommand  = ReactiveCommand.Create(Save,  canSave);
        ResetCommand = ReactiveCommand.Create(_goBack);
    }

    private async void Save()
    {
        if (IsEdit)
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Редактровать запись?",
                $"Вы уверены, что хотите редактировать запись - {_editItem.Title}?");
            if (confirmed)
            {
                Domain.Enums.ReferenceType type;
                if (ReferenceType == ReferenceType.Suppliers)
                    type = Domain.Enums.ReferenceType.Supplier;
                else if (ReferenceType == ReferenceType.Manufacturers)
                    type = Domain.Enums.ReferenceType.Manufacturer;
                else if (ReferenceType == ReferenceType.OrderStatuses)
                    type = Domain.Enums.ReferenceType.Status;
                else if (ReferenceType == ReferenceType.Category)
                    type = Domain.Enums.ReferenceType.Category;
                else
                    type = Domain.Enums.ReferenceType.Role;
                var dto = _referenceService.Update(type, new Reference(_editItem.Id, Title));

                if (dto.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync(
                        "Успешно",
                        $"Запись - {Title} отрадактирована.");
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync(
                        "Ошибка",
                        $"Ошибка редактирования - {dto.ErrorMessage}");
                }
            }
        }
        else
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Создать запись?",
                $"Вы уверены, что хотите создать запись - {Title}?");
            if (confirmed)
            {
                Domain.Enums.ReferenceType type;
                if (ReferenceType == ReferenceType.Suppliers)
                    type = Domain.Enums.ReferenceType.Supplier;
                else if (ReferenceType == ReferenceType.Manufacturers)
                    type = Domain.Enums.ReferenceType.Manufacturer;
                else if (ReferenceType == ReferenceType.OrderStatuses)
                    type = Domain.Enums.ReferenceType.Status;
                else if (ReferenceType == ReferenceType.Category)
                    type = Domain.Enums.ReferenceType.Category;
                else
                    type = Domain.Enums.ReferenceType.Role;
                var dto = _referenceService.Create(type, Title);

                if (dto.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync(
                        "Успешно",
                        $"Запись - {Title} создана.");
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync(
                        "Ошибка",
                        $"Ошибка создания - {dto.ErrorMessage}");
                }
            }
        }

        _goBack();
    }

    public static (Regex Pattern, string ErrorMessage) GetRuleForType(ReferenceType type) =>
        type switch
        {
            ReferenceType.Role or
            ReferenceType.Category or
            ReferenceType.OrderStatuses =>
                (new Regex(@"^[а-яёА-ЯЁ\s]+$"),           "Только русские буквы"),

            ReferenceType.Manufacturers =>
                (new Regex(@"^[а-яёА-ЯЁa-zA-Z\s]+$"),     "Только русские и латинские буквы"),

            ReferenceType.Suppliers =>
                (new Regex(@"^[а-яёА-ЯЁ«»""'\s]+$"),      "Только русские буквы и кавычки"),

            _ => (new Regex(@".*"), string.Empty)
        };

    private static bool IsValidForType(string value, ReferenceType type)
    {
        var (pattern, _) = GetRuleForType(type);
        return pattern.IsMatch(value.Trim());
    }
}
