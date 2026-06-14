using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IOrderService
{
    OrderResultDto GetOrders();
    OrderResultDto GetOrderItems(string orderId);
    OrderResultDto UpdateOrderStatus(string orderArticle);
    OrderResultDto CreateOrder(CreateOrderDto dto);
}