using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис управления заказами: просмотр, создание и смена статуса.
/// </summary>
public class OrderService : IOrderService
{
    private readonly OrderCommands _orderCommands = new();

    /// <summary>Возвращает список всех заказов.</summary>
    public OrderResultDto GetOrders()
    {
        try
        {
            var orders = OrderCommands.GetOrders();
            return OrderResultDto.Success(orders);
        }
        catch (DatabaseException)
        {
            return OrderResultDto.Failure("Не удалось загрузить список заказов.");
        }
    }

    /// <summary>Возвращает позиции указанного заказа.</summary>
    /// <param name="orderId">Идентификатор или артикул заказа.</param>
    public OrderResultDto GetOrderItems(string orderId)
    {
        try
        {
            var items = OrderCommands.GetOrderItems(orderId);
            return OrderResultDto.Success(items);
        }
        catch (DatabaseException)
        {
            return OrderResultDto.Failure("Не удалось загрузить позиции заказа.");
        }
    }

    /// <summary>Переводит заказ на следующий статус в жизненном цикле.</summary>
    /// <param name="orderArticle">Артикул заказа.</param>
    public OrderResultDto UpdateOrderStatus(string orderArticle)
    {
        try
        {
            _orderCommands.UpdateOrderStatus(orderArticle);
            return OrderResultDto.Success();
        }
        catch (DatabaseException)
        {
            return OrderResultDto.Failure("Не удалось обновить статус заказа.");
        }
    }

    /// <summary>Создаёт новый заказ с указанными позициями.</summary>
    /// <param name="dto">Данные для создания заказа.</param>
    public OrderResultDto CreateOrder(CreateOrderDto dto)
    {
        try
        {
            _orderCommands.CreateOrder(
                dto.OrderArticle,
                dto.ClientId,
                dto.EmployeeId,
                dto.DiscountPercent,
                dto.TotalAmount,
                dto.Items.Select(i =>
                    (i.ProductId, i.Quantity, i.Price, i.DiscountPercent)).ToList()
            );
            return OrderResultDto.Success();
        }
        catch (DatabaseException ex)
        {
            return OrderResultDto.Failure(ex.Message);
        }
    }
}
