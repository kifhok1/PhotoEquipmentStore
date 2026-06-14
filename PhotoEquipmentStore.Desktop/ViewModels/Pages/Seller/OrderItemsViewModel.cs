using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class OrderItemsViewModel : ViewModelBase
{
    private readonly OrderService _orderService = new();

    private OrderShow _order;
    public OrderShow Order
    {
        get => _order;
        set => this.RaiseAndSetIfChanged(ref _order, value);
    }

    public ObservableCollection<OrderItemShow> OrderItems { get; } = new();

    // ── Команды ───────────────────────────────────────────────────────────────

    public ReactiveCommand<Unit, Unit> GoBackCommand      { get; }
    public ReactiveCommand<Unit, Unit> ReturnOrderCommand { get; }

    // ── Вычисляемые итоги ─────────────────────────────────────────────────────

    public decimal Subtotal =>
        OrderItems.Sum(i => i.Price * i.Quantity);

    public decimal ProductDiscount =>
        OrderItems.Sum(i => i.Price * i.Discount / 100m * i.Quantity);

    public decimal AfterProductDiscount => Subtotal - ProductDiscount;

    public decimal ClientDiscount =>
        AfterProductDiscount * Order.DiscountClient / 100m;

    public decimal Total => AfterProductDiscount - ClientDiscount;

    public bool HasProductDiscount => ProductDiscount > 0;

    public bool HasClientDiscount  => Order.DiscountClient > 0;

    /// <summary>Статус «Оформлен» (id == "1") — активный заказ.</summary>
    public bool IsStatusActive => Order.StatusId == "1";

    public string ItemCountLabel
    {
        get
        {
            int c = OrderItems.Count;
            string word = c switch { 1 => "товар", 2 or 3 or 4 => "товара", _ => "товаров" };
            return $"{c} {word}";
        }
    }

    // ── Конструктор ───────────────────────────────────────────────────────────

    public OrderItemsViewModel(OrderShow order, Action goBack)
    {
        _order = order;

        GoBackCommand = ReactiveCommand.Create(goBack);

        ReturnOrderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Оформить возврат?",
                $"Вы уверены, что хотите оформить возврат по заказу №{Order.Id}? Это действие нельзя будет отменить.");

            if (!confirmed) return;

            var result = _orderService.UpdateOrderStatus(Order.Id.ToString());
            if (result.IsSuccess)
            {
                await NotificationService.Instance.ShowInfoAsync(
                    "Успешно", $"Возврат по заказу №{Order.Id} успешно оформлен.");

                // Обновляем статус локально, чтобы кнопка сразу скрылась
                Order.StatusId   = "2";
                Order.StatusName = "Возврат";
                this.RaisePropertyChanged(nameof(IsStatusActive));
                this.RaisePropertyChanged(nameof(Order));
            }
            else
            {
                await NotificationService.Instance.ShowErrorAsync(
                    "Ошибка", $"Не удалось оформить возврат по заказу №{Order.Id}.");
            }
        });

        LoadItems(order.Id.ToString());
    }

    [Obsolete("Design-time only")]
    public OrderItemsViewModel()
    {
        _order = new OrderShow("2", "2", "Иванова Марина Сергеевна", "+7(905)777-88-99",
                               5, 0, "Иванов И.И.", "1", "Оформлен",
                               DateTime.Now, 0);
        GoBackCommand      = ReactiveCommand.Create(() => { });
        ReturnOrderCommand = ReactiveCommand.Create(() => { });
        LoadItems("72963458");
    }

    // ── Загрузка ──────────────────────────────────────────────────────────────

    private async void LoadItems(string orderId)
    {
        var result = _orderService.GetOrderItems(orderId);
        if (result.IsSuccess)
        {
            foreach (var i in result.OrderItems)
                OrderItems.Add(new OrderItemShow(
                    i.ProductId, i.ProductName, i.Quantity,
                    i.Price, i.Discount,
                    BitmapHelper.FromBytes(i.ProductImage)));
        }
        else
        {
            await NotificationService.Instance.ShowErrorAsync(
                "Ошибка", "Не удалось загрузить позиции заказа.");
        }

        this.RaisePropertyChanged(nameof(Subtotal));
        this.RaisePropertyChanged(nameof(ProductDiscount));
        this.RaisePropertyChanged(nameof(AfterProductDiscount));
        this.RaisePropertyChanged(nameof(ClientDiscount));
        this.RaisePropertyChanged(nameof(Total));
        this.RaisePropertyChanged(nameof(HasProductDiscount));
        this.RaisePropertyChanged(nameof(HasClientDiscount));
        this.RaisePropertyChanged(nameof(IsStatusActive));
        this.RaisePropertyChanged(nameof(ItemCountLabel));
    }
}