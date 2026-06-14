namespace PhotoEquipmentStore.Application.DTO;

public class CreateOrderItemDto
{
    public int     ProductId       { get; init; }
    public int     Quantity        { get; init; }
    public decimal Price           { get; init; }
    public decimal DiscountPercent { get; init; }
}
