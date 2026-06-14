namespace PhotoEquipmentStore.Application.DTO;

/// <summary>
/// Позиция товара при создании заказа.
/// </summary>
public class CreateOrderItemDto
{
    public int     ProductId       { get; init; }
    public int     Quantity        { get; init; }
    public decimal Price           { get; init; }
    public decimal DiscountPercent { get; init; }
}
