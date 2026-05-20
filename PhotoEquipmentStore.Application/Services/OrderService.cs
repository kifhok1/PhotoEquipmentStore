using System.Collections.ObjectModel;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;

namespace PhotoEquipmentStore.Application.Services;

public class OrderService
{
    public static ObservableCollection<Order> GetOrders()
    {
        return OrderCommands.GetOrders();
    }

    public static ObservableCollection<OrderItem> GetOrder(string orderId)
    {
        return OrderCommands.GetOrderItems(orderId);
    }
}