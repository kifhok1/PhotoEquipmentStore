using System;
using System.Linq;
using PhotoEquipmentStore.Helper;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Модель клиента с маскированием данных и расчётом скидки.
/// </summary>


public class ClientShow : ReactiveObject
{
    private int _Id;
    private string _name;
    private string _phoneNumber;
    private string _totalPurchases;
    private int _countOrders;
    private bool _isRevealed;

    /// <summary>

    /// Уникальный идентификатор записи.

    /// </summary>

    public int Id
    {
        get => _Id;
        set => this.RaiseAndSetIfChanged(ref _Id, value);
    }

    /// <summary>

    /// Наименование или ФИО.

    /// </summary>

    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            this.RaisePropertyChanged(nameof(DisplayLabel));
        }
    }

    /// <summary>

    /// Номер телефона.

    /// </summary>

    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            this.RaiseAndSetIfChanged(ref _phoneNumber, value);
            this.RaisePropertyChanged(nameof(DisplayLabel));
        }
    }

    /// <summary>

    /// Сумма покупок клиента.

    /// </summary>

    public string TotalPurchases
    {
        get => _totalPurchases;
        set
        {
            this.RaiseAndSetIfChanged(ref _totalPurchases, value);
            this.RaisePropertyChanged(nameof(TotalPurchasesAmount));
            this.RaisePropertyChanged(nameof(ClientDiscountPercent));
            this.RaisePropertyChanged(nameof(ClientDiscountLabel));
        }
    }

    /// <summary>

    /// Количество заказов клиента.

    /// </summary>

    public int CountOrders
    {
        get => _countOrders;
        set => this.RaiseAndSetIfChanged(ref _countOrders, value);
    }

    /// <summary>

    /// Признак раскрытия персональных данных клиента.

    /// </summary>

    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            this.RaiseAndSetIfChanged(ref _isRevealed, value);
            this.RaisePropertyChanged(nameof(NameShow));
            this.RaisePropertyChanged(nameof(PhoneNumberShow));
        }
    }

    private bool _isSelected;
    /// <summary>
    /// Признак выбора клиента в списке.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public int TotalPurchasesAmount =>
        int.TryParse(TotalPurchases?.Replace(" ", "").Replace(",", ""), out var v) ? v : 0;

    /// <summary>

    /// Процент накопительной скидки клиента.

    /// </summary>

    public int ClientDiscountPercent => TotalPurchasesAmount switch
    {
        >= 500_000 => 15,
        >= 250_000 => 10,
        >= 100_000 => 5,
        _          => 0
    };

    /// <summary>

    /// Текстовая метка скидки клиента.

    /// </summary>

    public string ClientDiscountLabel => ClientDiscountPercent switch
    {
        0 => "нет",
        _ => $"{ClientDiscountPercent}% (накопительная)"
    };

    public string TotalPurchasesShow => TotalPurchases + " ₽";
    public string CountOrdersShow    => CountOrders + " шт.";

    public string PhoneNumberShow => IsRevealed
        ? PhoneNumber
        : MaskClientsData.MaskPhoneNumber(PhoneNumber);

    public string NameShow => IsRevealed
        ? Name
        : MaskClientsData.MaskFullName(Name);

    /// <summary>

    /// Краткая подпись клиента для выпадающего списка.

    /// </summary>

    public string DisplayLabel
    {
        get
        {
            var parts = Name?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];
            string shortName = parts.Length switch
            {
                0 => "—",
                1 => parts[0],
                2 => $"{parts[0]} {parts[1][0]}.",
                _ => $"{parts[0]} {parts[1][0]}. {parts[2][0]}."
            };

            var digits = new string(PhoneNumber?.Where(char.IsDigit).ToArray() ?? []);
            string tail = digits.Length >= 4
                ? $"{digits[^4..^2]}-{digits[^2..]}"
                : digits;

            return $"{shortName}  {tail}";
        }
    }

    public ClientShow(
        int id,
        string name,
        string phoneNumber,
        string totalPurchases,
        int countOrders)
    {
        _Id             = id;
        _name           = name;
        _phoneNumber    = phoneNumber;
        _totalPurchases = totalPurchases;
        _countOrders    = countOrders;
    }
}
