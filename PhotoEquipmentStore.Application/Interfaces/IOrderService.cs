using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса управления заказами.
/// </summary>
public interface IOrderService
{
    /// <summary>Возвращает список всех заказов.</summary>
    OrderResultDto GetOrders();

    /// <summary>Возвращает позиции указанного заказа.</summary>
    /// <param name="orderId">Идентификатор или артикул заказа.</param>
    OrderResultDto GetOrderItems(string orderId);

    /// <summary>Переводит заказ на следующий статус.</summary>
    /// <param name="orderArticle">Артикул заказа.</param>
    OrderResultDto UpdateOrderStatus(string orderArticle);

    /// <summary>Создаёт новый заказ с позициями.</summary>
    /// <param name="dto">Данные для создания заказа.</param>
    OrderResultDto CreateOrder(CreateOrderDto dto);
}
