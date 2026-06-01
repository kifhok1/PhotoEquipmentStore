using PhotoEquipmentStore.Helper;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;

public class ClientShow : ReactiveObject
{
    private int _Id;
    private string _name;
    private string _phoneNumber;
    private string _totalPurchases;
    private int _countOrders;
    private bool _isRevealed;

    public int Id
    {
        get => _Id;
        set => this.RaiseAndSetIfChanged(ref _Id, value);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
    }

    public string TotalPurchases
    {
        get => _totalPurchases;
        set => this.RaiseAndSetIfChanged(ref _totalPurchases, value);
    }

    public int CountOrders
    {
        get => _countOrders;
        set => this.RaiseAndSetIfChanged(ref _countOrders, value);
    }

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

    public string TotalPurchasesShow => TotalPurchases + " ₽";
    public string CountOrdersShow    => CountOrders + " шт.";

    public string PhoneNumberShow => IsRevealed
        ? PhoneNumber
        : MaskClientsData.MaskPhoneNumber(PhoneNumber);

    public string NameShow => IsRevealed
        ? Name
        : MaskClientsData.MaskFullName(Name);

    public ClientShow(
        int id,
        string name,
        string phoneNumber,
        string totalPurchases,
        int countOrders)
    {
        _Id        = id;
        _name           = name;
        _phoneNumber    = phoneNumber;
        _totalPurchases = totalPurchases;
        _countOrders    = countOrders;
    }
}