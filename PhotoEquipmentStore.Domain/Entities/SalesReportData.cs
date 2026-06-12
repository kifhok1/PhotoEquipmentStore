using System;

namespace PhotoEquipmentStore.Domain.Entities;

public class SalesReportData
{
    public string   OrderId       { get; }
    public DateTime OrderDate     { get; }
    public string   ClientName    { get; }
    public string   ClientPhone   { get; }
    public string   EmployeeName  { get; }
    public string   StatusName    { get; }
    public int      Discount      { get; }
    public int      ItemsCount    { get; }
    public int      TotalQuantity { get; }
    public decimal  TotalSum      { get; }

    public SalesReportData(
        string orderId, DateTime orderDate, string clientName,
        string clientPhone, string employeeName, string statusName,
        int discount, int itemsCount, int totalQuantity, decimal totalSum)
    {
        OrderId       = orderId;
        OrderDate     = orderDate;
        ClientName    = clientName;
        ClientPhone   = clientPhone;
        EmployeeName  = employeeName;
        StatusName    = statusName;
        Discount      = discount;
        ItemsCount    = itemsCount;
        TotalQuantity = totalQuantity;
        TotalSum      = totalSum;
    }
}