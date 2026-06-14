using System;
using Avalonia.Media;
using PhotoEquipmentStore.Helper;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;

public class OrderShow : ReactiveObject
{
    private string _id;
    private string _clientId;
    private string _clientName;
    private string _clientPhoneNumber;
    private int _discountClient;
    private int _userId;
    private string _userName;
    private string _statusId;
    private string _statusName;
    private DateTime _orderDate;
    private decimal _totalSum;
    private bool _isRevealed;

    public string Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public string ClientId
    {
        get => _clientId;
        set => this.RaiseAndSetIfChanged(ref _clientId, value);
    }

    public string ClientName
    {
        get => _clientName;
        set => this.RaiseAndSetIfChanged(ref _clientName, value);
    }

    public string ClientPhoneNumber
    {
        get => _clientPhoneNumber;
        set => this.RaiseAndSetIfChanged(ref _clientPhoneNumber, value);
    }

    public int DiscountClient
    {
        get => _discountClient;
        set => this.RaiseAndSetIfChanged(ref _discountClient, value);
    }

    public string DiscountClientShow => $"{_discountClient}%";

    public int UserId
    {
        get => _userId;
        set => this.RaiseAndSetIfChanged(ref _userId, value);
    }

    public string UserName
    {
        get => _userName;
        set => this.RaiseAndSetIfChanged(ref _userName, value);
    }

    public string StatusId
    {
        get => _statusId;
        set => this.RaiseAndSetIfChanged(ref _statusId, value);
    }

    public string StatusName
    {
        get => _statusName;
        set => this.RaiseAndSetIfChanged(ref _statusName, value);
    }

    public DateTime OrderDate
    {
        get => _orderDate;
        set => this.RaiseAndSetIfChanged(ref _orderDate, value);
    }

    public decimal TotalSum
    {
        get => _totalSum;
        set => this.RaiseAndSetIfChanged(ref _totalSum, value);
    }

    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            this.RaiseAndSetIfChanged(ref _isRevealed, value);
            this.RaisePropertyChanged(nameof(ClientNameShow));
            this.RaisePropertyChanged(nameof(ClientPhoneNumberShow));
        }
    }

    public bool IsReturnStatus => StatusId == "2";

    public string ClientNameShow => IsRevealed
        ? ClientName
        : MaskClientsData.MaskFullName(ClientName);

    public string ClientPhoneNumberShow => IsRevealed
        ? ClientPhoneNumber
        : MaskClientsData.MaskPhoneNumber(ClientPhoneNumber);

    public IBrush PriceColor
    {
        get
        {
            if (TotalSum > 400000)
                return ColorProvider.GetBrush("accent", Colors.White);
            if (TotalSum > 200000)
                return ColorProvider.GetBrush("success", Colors.White);
            if (TotalSum > 100000)
                return ColorProvider.GetBrush("info", Colors.White);

            return ColorProvider.GetBrush("secondary_text", Colors.White);
        }
    }

    public OrderShow(
        string orderId,
        string clientId,
        string clientName,
        string clientPhoneNumber,
        int discountClient,
        int userId,
        string userName,
        string statusId,
        string statusName,
        DateTime orderDate,
        decimal totalSum)
    {
        _id                 = orderId;
        _clientId           = clientId;
        _clientName         = clientName;
        _clientPhoneNumber  = clientPhoneNumber;
        _discountClient     = discountClient;
        _userId             = userId;
        _userName           = userName;
        _statusId           = statusId;
        _statusName         = statusName;
        _orderDate          = orderDate;
        _totalSum           = totalSum;
    }
}
