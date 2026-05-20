using System;
using PhotoEquipmentStore.Helper;

namespace PhotoEquipmentStore.Models;

public class ClientShow
{
    private int orderId;
    private string name;
    private string phoneNumber;
    private string totalPurchases;
    private int countOrders;

    public int OrderId
    {
        get => orderId; 
        set => orderId = value;
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
        get => totalPurchases;
        set => totalPurchases = value;
    }

    public int CountOrders
    {
        get => countOrders;
        set => countOrders = value;
    }
    
    public string TotalPurchasesShow
    {
        get => TotalPurchases + " ₽";
    }
    
    public string CountOrdersShow
    {
        get => CountOrders + " шт.";
    }

    public string PhoneNumberShow
    {
        get => MaskClientsData.MaskPhoneNumber(PhoneNumber);
    }

    public string NameShow
    {
        get => MaskClientsData.MaskFullName(Name);
    }

    public ClientShow(
        int id, 
        string name, 
        string phoneNumber, 
        string totalPurchases,
        int countOrders)
    {
        OrderId = id;
        Name = name;
        PhoneNumber = phoneNumber;
        TotalPurchases = totalPurchases;
        CountOrders = countOrders;
    }
}