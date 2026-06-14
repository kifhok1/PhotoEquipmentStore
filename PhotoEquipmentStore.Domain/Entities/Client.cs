
namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Клиент магазина с контактными данными и статистикой покупок.
/// </summary>
public class Client
{
    /// <summary>
    /// Уникальный идентификатор клиента.
    /// </summary>
    public int         Id             { get; }
    /// <summary>
    /// Полное имя клиента.
    /// </summary>
    public string      FullName       { get; }
    /// <summary>
    /// Номер телефона клиента.
    /// </summary>
    public string      Phone          { get; }
    /// <summary>
    /// Общая сумма покупок клиента.
    /// </summary>
    public int         TotalPurchases { get; }
    /// <summary>
    /// Количество оформленных заказов.
    /// </summary>
    public int         CountOrders    { get; }

    /// <summary>
    /// Создаёт клиента с полной статистикой покупок.
    /// </summary>
    public Client(int id, string name, string phone, int totalPurchases, int countOrders)
    {
        Id             = id;
        FullName       = name;
        Phone          = phone;
        TotalPurchases = totalPurchases;
        CountOrders    = countOrders;
    }

    /// <summary>
    /// Создаёт клиента с идентификатором и контактными данными.
    /// </summary>
    public Client(int id, string name, string phone)
    {
        Id             = id;
        FullName       = name;
        Phone          = phone;
    }

    /// <summary>
    /// Создаёт нового клиента без идентификатора для сохранения в базу.
    /// </summary>
    public Client(string name, string phone)
    {
        FullName       = name;
        Phone          = phone;
    }
}
