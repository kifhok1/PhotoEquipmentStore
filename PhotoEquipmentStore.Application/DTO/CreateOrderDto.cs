namespace PhotoEquipmentStore.Application.DTO;

public class CreateOrderDto
{
    public string  OrderArticle    { get; init; } = "";
    public int     ClientId        { get; init; }
    public int     EmployeeId      { get; init; }
    public int     DiscountPercent { get; init; }
    public decimal TotalAmount     { get; init; }

    public List<CreateOrderItemDto> Items { get; init; } = [];
}
