using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.Exceptions;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class OrderService
{
    public static ObservableCollection<Order> GetOrders()
    {
        try
        {
            return OrderCommands.GetOrders();
        }
        catch (DatabaseException ex)
        {
            throw new ServiceException("Не удалось загрузить список заказов.", ex);
        }
    }

    public static ObservableCollection<OrderItem> GetOrder(string orderId)
    {
        try
        {
            return OrderCommands.GetOrderItems(orderId);
        }
        catch (DatabaseException ex)
        {
            throw new ServiceException("Не удалось загрузить позиции заказа.", ex);
        }
    }
}