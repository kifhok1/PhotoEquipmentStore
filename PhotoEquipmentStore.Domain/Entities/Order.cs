using System;

namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Заказ клиента с информацией о статусе, сотруднике и итоговой сумме.
/// </summary>
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

    /// <summary>
    /// Артикул (уникальный номер) заказа.
    /// </summary>
    public string OrderId
    {
        get => orderId;
        set => orderId = value;
    }

    /// <summary>
    /// Идентификатор клиента, оформившего заказ.
    /// </summary>
    public string ClientId
    {
        get => clientId;
        set => clientId = value;
    }

    /// <summary>
    /// Полное имя клиента.
    /// </summary>
    public string ClientName
    {
        get => clientName;
        set => clientName = value;
    }

    /// <summary>
    /// Номер телефона клиента.
    /// </summary>
    public string ClientPhoneNumber
    {
        get => clientPhoneNumber;
        set => clientPhoneNumber = value;
    }

    /// <summary>
    /// Процент скидки клиента, применённый к заказу.
    /// </summary>
    public int DiscountClient
    {
        get => discountClient;
        set => discountClient = value;
    }

    /// <summary>
    /// Идентификатор сотрудника, оформившего заказ.
    /// </summary>
    public int UserId
    {
        get => userId;
        set => userId = value;
    }

    /// <summary>
    /// Полное имя сотрудника, оформившего заказ.
    /// </summary>
    public string UserName
    {
        get => userName;
        set => userName = value;
    }

    /// <summary>
    /// Идентификатор текущего статуса заказа.
    /// </summary>
    public string StatusId
    {
        get => statusId;
        set => statusId = value;
    }

    /// <summary>
    /// Наименование текущего статуса заказа.
    /// </summary>
    public string StatusName
    {
        get => statusName;
        set => statusName = value;
    }

    /// <summary>
    /// Дата и время создания заказа.
    /// </summary>
    public DateTime OrderDate
    {
        get => orderDate;
        set => orderDate = value;
    }

    /// <summary>
    /// Итоговая сумма заказа с учётом скидок.
    /// </summary>
    public decimal TotalSum
    {
        get => totalSum;
        set => totalSum = value;
    }

    /// <summary>
    /// Создаёт заказ с полным набором полей.
    /// </summary>
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
