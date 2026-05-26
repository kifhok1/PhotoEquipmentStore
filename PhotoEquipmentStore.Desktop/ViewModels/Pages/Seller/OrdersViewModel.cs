using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class OrdersViewModel : ViewModelBase
{
    private readonly List<OrderShow> _allOrders = new();
    public ObservableCollection<OrderShow> Orders { get; } = new();

    private ObservableCollection<OrderShow> _currentOrders = new();
    public ObservableCollection<OrderShow> CurrentOrders
    {
        get => _currentOrders;
        set => this.RaiseAndSetIfChanged(ref _currentOrders, value);
    }

    private OrderShow? _selectedOrder;
    public OrderShow? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedOrder, value);

            if (value is null || value.IsRevealed) return;

            value.IsRevealed = true;

            Observable.Timer(TimeSpan.FromSeconds(15))
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Subscribe(_ => value.IsRevealed = false);
        }
    }

    private string _countOrders = string.Empty;
    public string CountOrders
    {
        get => _countOrders;
        set => this.RaiseAndSetIfChanged(ref _countOrders, value);
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            ApplySearch(value);
        }
    }

    public ReactiveCommand<OrderShow, Unit> ViewOrderItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetSearchCommand         { get; }

    public OrdersViewModel(Action<OrderShow> onViewOrderItems)
    {
        ViewOrderItemsCommand = ReactiveCommand.Create<OrderShow>(onViewOrderItems);
        ResetSearchCommand = ReactiveCommand.Create(() => { SearchText = string.Empty; });
        LoadOrders();
    }

    [Obsolete("Design-time only")]
    public OrdersViewModel()
    {
        ViewOrderItemsCommand = ReactiveCommand.Create<OrderShow>(_ => { });
        ResetSearchCommand = ReactiveCommand.Create(() => { SearchText = string.Empty; });
        LoadOrders();
    }

    private void LoadOrders()
    {
        var ordersDB = OrderService.GetOrders();
        foreach (var order in ordersDB)
        {
            var show = new OrderShow(
                order.OrderId, order.ClientId, order.ClientName,
                order.ClientPhoneNumber, order.DiscountClient, order.UserId, order.UserName,
                order.StatusId, order.StatusName, order.OrderDate, order.TotalSum);

            _allOrders.Add(show);
            Orders.Add(show);
        }

        UpdateCountOrders(Orders.Count);
    }

    private void ApplySearch(string query)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? _allOrders
            : _allOrders.Where(o => o.Id.ToString().Contains(query.Trim())).ToList();

        Orders.Clear();
        foreach (var o in result) Orders.Add(o);

        UpdateCountOrders(Orders.Count);
    }

    private void UpdateCountOrders(int count) =>
        CountOrders = $"Количество элементов на форме: {count}";
}