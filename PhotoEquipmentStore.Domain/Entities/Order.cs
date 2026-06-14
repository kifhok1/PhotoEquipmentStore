using System;

namespace PhotoEquipmentStore.Domain.Entities;

public class Order
{
    private string orderId;
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

    public string OrderId
    {
        get => orderId;
        set => orderId = value;
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

    public Order(
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
        OrderId = orderId;
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
