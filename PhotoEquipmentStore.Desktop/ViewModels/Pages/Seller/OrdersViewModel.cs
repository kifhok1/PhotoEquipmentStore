using System;
using System.Collections.ObjectModel;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class OrdersViewModel : ViewModelBase
{
    public ObservableCollection<OrderShow> Orders { get; private set; } = new();

    private string _countOrders = string.Empty;
    private ObservableCollection<OrderShow> _currentOrders = new();

    public ObservableCollection<OrderShow> CurrentOrders
    {
        get => _currentOrders;
        set => this.RaiseAndSetIfChanged(ref _currentOrders, value);
    }

    public string CountOrders
    {
        get => _countOrders;
        set => this.RaiseAndSetIfChanged(ref _countOrders, value);
    }

    // Команда принимает выбранный заказ и вызывает колбэк навигации
    public ReactiveCommand<OrderShow, Unit> ViewOrderItemsCommand { get; }

    public OrdersViewModel(Action<OrderShow> onViewOrderItems)
    {
        ViewOrderItemsCommand = ReactiveCommand.Create<OrderShow>(onViewOrderItems);

        var ordersDB = OrderService.GetOrders();
        foreach (var order in ordersDB)
        {
            Orders.Add(new OrderShow(order.OrderId, order.ClientId, order.ClientName,
                order.ClientPhoneNumber, order.DiscountClient, order.UserId, order.UserName,
                order.StatusId, order.StatusName, order.OrderDate, order.TotalSum));
        }

        CountOrders = $"Количество элементов на форме: {Orders.Count}";
    }
    
    [Obsolete("Design-time only")]
    public OrdersViewModel()
    {
        ViewOrderItemsCommand = ReactiveCommand.Create<OrderShow>(_ => { });

        var ordersDB = OrderService.GetOrders();
        foreach (var order in ordersDB)
        {
            Orders.Add(new OrderShow(order.OrderId, order.ClientId, order.ClientName,
                order.ClientPhoneNumber, order.DiscountClient, order.UserId, order.UserName,
                order.StatusId, order.StatusName, order.OrderDate, order.TotalSum));
        }

        CountOrders = $"Количество элементов на форме: {Orders.Count}";
    }
}