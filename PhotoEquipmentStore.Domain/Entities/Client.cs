
namespace PhotoEquipmentStore.Domain.Entities;

public class Client
{
    public int         Id             { get; }
    public string      FullName       { get; }
    public string      Phone          { get; }
    public int         TotalPurchases { get; }
    public int         CountOrders    { get; }

    public Client(int id, string name, string phone, int totalPurchases, int countOrders)
    {
        Id             = id;
        FullName       = name;
        Phone          = phone;
        TotalPurchases = totalPurchases;
        CountOrders    = countOrders;
    }

    public Client(int id, string name, string phone)
    {
        Id             = id;
        FullName       = name;
        Phone          = phone;
    }

    public Client(string name, string phone)
    {
        FullName       = name;
        Phone          = phone;
    }
}
