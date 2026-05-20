using System;
using Avalonia.Media;
using PhotoEquipmentStore.Helper;

namespace PhotoEquipmentStore.Models;

public class OrderShow
{
    private string id;
    private string clientId;
    private string clientName;
    private string clientPhoneNumber;
    private int discountClient;
    private int userId;
    private string userName;
    private string statusId;
    private string statusName;
    private DateTime orderDate;
    private decimal totalSum;

    public string Id
    {
        get => id;
        set => id = value;
    }

    public string ClientId
    {
        get => clientId;
        set => clientId = value;
    }

    public string ClientName
    {
        get => clientName;
        set => clientName = value;
    }

    public string ClientPhoneNumber
    {
        get => clientPhoneNumber;
        set => clientPhoneNumber = value;
    }

    public int DiscountClient
    {
        get => discountClient;
        set => discountClient = value;
    }
    
    public string DiscountClientShow
    {
        get => $"{discountClient}%";
    }

    public int UserId
    {
        get => userId;
        set => userId = value;
    }

    public string UserName
    {
        get => userName;
        set => userName = value;
    }

    public string StatusId
    {
        get => statusId;
        set => statusId = value;
    }

    public string StatusName
    {
        get => statusName;
        set => statusName = value;
    }

    public DateTime OrderDate
    {
        get => orderDate;
        set => orderDate = value;
    }
    
    public decimal TotalSum
    {
        get => totalSum;
        set => totalSum = value;
    }

    public string ClientNameShow
    {
        get => MaskClientsData.MaskFullName(ClientName);
    }

    public string ClientPhoneNumberShow
    {
        get => MaskClientsData.MaskPhoneNumber(ClientPhoneNumber);
    }

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
        Id = orderId;
        ClientId = clientId;
        ClientName = clientName;
        ClientPhoneNumber = clientPhoneNumber;
        DiscountClient = discountClient;
        UserId = userId;
        UserName = userName;
        StatusId = statusId;
        StatusName = statusName;
        OrderDate = orderDate;
        TotalSum = totalSum;
    }
}