namespace PhotoEquipmentStore.Application.DTO;

public class ReceiptData
{
    public string OrderNumber      { get; init; } = "";
    public string CreatedAt        { get; init; } = "";

    // Клиент
    public string ClientName       { get; init; } = "";
    public string ClientPhone      { get; init; } = "";
    public string ClientDiscount   { get; init; } = ""; // например "5%"

    // Продавец
    public string SellerName       { get; init; } = "";

    // Позиции
    public List<ReceiptItem> Items { get; init; } = [];

    // Суммы
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
    public int    OriginalPrice{ get; init; } // цена без скидки за штуку
    public int    FinalPrice   { get; init; } // цена со скидкой за штуку
    public int    LineTotal    { get; init; } // итого по позиции (со скидкой * кол-во)
}