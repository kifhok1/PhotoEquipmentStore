namespace PhotoEquipmentStore.Domain.Entities;

public class Client
{
    private int id;
    private string name;
    private string phoneNumber;
    private int totalPurchases;
    private int countOrders;

    public int Id
    {
        get => id; 
        set => id = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public string PhoneNumber
    {
        get => phoneNumber;
        set => phoneNumber = value;
    }

    public int TotalPurchases
    {
        get => totalPurchases;
        set => totalPurchases = value;
    }

    public int CountOrders
    {
        get => countOrders;
        set => countOrders = value;
    }
    public Client(int id, 
    string name, 
    string phoneNumber, 
    int totalPurchases,
    int countOrders)
    {
        Id = id;
        Name = name;
        PhoneNumber = phoneNumber;
        TotalPurchases = totalPurchases;
        CountOrders = countOrders;
    }
}