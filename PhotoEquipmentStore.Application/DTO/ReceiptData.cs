namespace PhotoEquipmentStore.Application.DTO;

public class ReceiptData
{
    public string OrderNumber      { get; init; } = "";
    public string CreatedAt        { get; init; } = "";

    public string ClientName       { get; init; } = "";
    public string ClientPhone      { get; init; } = "";
    public string ClientDiscount   { get; init; } = "";

    public string SellerName       { get; init; } = "";

    public List<ReceiptItem> Items { get; init; } = [];

    public int Subtotal            { get; init; }
    public int ProductDiscount     { get; init; }
    public int ClientDiscountPct   { get; init; }
    public int ClientDiscountAmt   { get; init; }
    public int Discount            { get; init; }
    public int Delivery            { get; init; }
    public int Total               { get; init; }
}

public class ReceiptItem
{
    public string Name         { get; init; } = "";
    public int    Quantity     { get; init; }
    public int    OriginalPrice{ get; init; }
    public int    FinalPrice   { get; init; }
    public int    LineTotal    { get; init; }
}
