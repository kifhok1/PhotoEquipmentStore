using System;
using System.Reactive;
using System.Text.RegularExpressions;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public class ReferenceAddViewModel : ViewModelBase
{
    private readonly Action _goBack;
    private readonly ReferenceShow? _editItem;   // null → режим создания

    public ReferenceType ReferenceType { get; }
    public string PageTitle  { get; }
    public string FieldLabel { get; }
    public bool   IsEdit     => _editItem is not null;

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand  { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    /// <param name="goBack">Вызывается при «Сбросить» и после успешного сохранения</param>
    /// <param name="type">Тип справочника</param>
    /// <param name="editItem">Не null → режим редактирования</param>
    public ReferenceAddViewModel(Action goBack, ReferenceType type, ReferenceShow? editItem = null)
    {
        _goBack   = goBack;
        _editItem = editItem;
        ReferenceType = type;

        // Предзаполнение при редактировании
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
        ResetCommand = ReactiveCommand.Create(_goBack);   // ← сразу возврат на список
    }

    private void Save()
    {
        if (IsEdit)
        {
            // TODO: обновить _editItem.Id через сервис
        }
        else
        {
            // TODO: создать новую запись через сервис
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