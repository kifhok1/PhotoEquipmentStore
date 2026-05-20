using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class OrderItemsViewModel : ViewModelBase
{
    private OrderShow order;
    private string countOrderItems = "";

    public OrderShow Order
    {
        get => order;
        set => this.RaiseAndSetIfChanged(ref order, value);
    }

    public ObservableCollection<OrderItemShow> OrderItems
    {
        get;
        private set;
    } = new();
    
    public string CountOrderItems
    {
        get => countOrderItems;
        set => this.RaiseAndSetIfChanged(ref countOrderItems, value);
    }

    public string ClientNameShow
    {
        get => $"Клинет: {order.ClientName}";
    }
    
    public string UserNameShow
    {
        get => $"Продавец: {order.UserName}";
    }
    
    public string OrderDateShow
    {
        get => $"Дата заказа: {order.OrderDate}";
    }

    public string OrderTotalShow
    {
        get
        {
            decimal total = 0;
            foreach (OrderItemShow orderItem in OrderItems)
            {
                total += orderItem.Price * orderItem.Quantity;
            }

            return $"Общая цена: {total}";
        }
    }
    
    public string OrderTotalDiscountShow
    {
        get
        {
            decimal total = 0;
            foreach (OrderItemShow orderItem in OrderItems)
            {
                total += (orderItem.Price - orderItem.Price * orderItem.Discount / 100) * orderItem.Quantity;
            }
            return $"Общая цена cо скидкой: {total}";
        }
    }
    
    public string OrderClientDiscountShow
    {
        get => $"Скидка клиента: {order.DiscountClient}%";
    }
    
    public string OrderTotalAndShow
    {
        get
        {
            decimal total = 0;
            foreach (OrderItemShow orderItem in OrderItems)
            {
                total += (orderItem.Price - orderItem.Price * orderItem.Discount / 100) * orderItem.Quantity;
            }

            if (order.DiscountClient != 0)
            {
                total /= order.DiscountClient; 
            }
            return $"Итог: {total}";
        }
    }

    public OrderItemsViewModel(OrderShow order)
    {
        Order = order;
        var orderItems = OrderService.GetOrder(order.Id);
        foreach (var orderItem in orderItems)
        {
            OrderItems.Add(new OrderItemShow(orderItem.ProductId, orderItem.ProductName, orderItem.Quantity, 
                orderItem.Price, orderItem.Discount, BitmapHelper.FromBytes(orderItem.ProductImage)));
        }
        CountOrderItems = $"Количество элементов на форме {OrderItems.Count}";
    }
    
    public OrderItemsViewModel()
    {
        var orderItems = OrderService.GetOrder("72963458");
        foreach (var orderItem in orderItems)
        {
            OrderItems.Add(new OrderItemShow(orderItem.ProductId, orderItem.ProductName, orderItem.Quantity, 
                orderItem.Price, orderItem.Discount, BitmapHelper.FromBytes(orderItem.ProductImage)));
        }
        CountOrderItems = $"Количество элементов на форме {OrderItems.Count}";
    }
}