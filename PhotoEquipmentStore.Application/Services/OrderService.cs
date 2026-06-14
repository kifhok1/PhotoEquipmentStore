using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly OrderCommands _orderCommands = new();

    // ── Read ──────────────────────────────────────────────────────────────────

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

    // ── Update ────────────────────────────────────────────────────────────────

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

    // ── Create ────────────────────────────────────────────────────────────────

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