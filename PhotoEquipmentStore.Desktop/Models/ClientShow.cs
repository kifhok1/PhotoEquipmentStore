using System;

namespace PhotoEquipmentStore.Models;

public class ClientShow : IPaginationShow
{
    private int id;
    private string name;
    private string phoneNumber;
    private string totalPurchases;
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

    public string TotalPurchases
    {
        get => totalPurchases + " ₽";
        set => totalPurchases = value;
    }

    public string CountOrders
    {
        get => countOrders + " шт.";
        set => countOrders = Convert.ToInt32(value);
    }

    public ClientShow(
        int id, 
        string name, 
        string phoneNumber, 
        string totalPurchases,
        int countOrders)
    {
        Id = id;
        Name = name;
        PhoneNumber = phoneNumber;
        TotalPurchases = totalPurchases;
        CountOrders = countOrders.ToString();
    }
}