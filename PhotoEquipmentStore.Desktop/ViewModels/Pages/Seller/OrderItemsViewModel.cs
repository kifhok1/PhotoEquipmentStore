using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;/// <summary>
/// ViewModel просмотра состава заказа и оформления возврата.
/// </summary>


public class OrderItemsViewModel : ViewModelBase
{
    private readonly OrderService _orderService = new();

    private OrderShow _order;
    /// <summary>
    /// Модель текущего заказа.
    /// </summary>
    public OrderShow Order
    {
        get => _order;
        set => this.RaiseAndSetIfChanged(ref _order, value);
    }

    /// <summary>

    /// Позиции состава заказа.

    /// </summary>

    public ObservableCollection<OrderItemShow> OrderItems { get; } = new();

    /// <summary>

    /// Команда возврата к оформлению заказа.

    /// </summary>

    public ReactiveCommand<Unit, Unit> GoBackCommand      { get; }
    /// <summary>
    /// Команда оформления возврата по заказу.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ReturnOrderCommand { get; }

    /// <summary>

    /// Сумма позиций без скидок.

    /// </summary>

    public decimal Subtotal =>
        OrderItems.Sum(i => i.Price * i.Quantity);

    /// <summary>

    /// Сумма скидки по товарам.

    /// </summary>

    public decimal ProductDiscount =>
        OrderItems.Sum(i => i.Price * i.Discount / 100m * i.Quantity);

    public decimal AfterProductDiscount => Subtotal - ProductDiscount;

    /// <summary>

    /// Сумма накопительной скидки клиента.

    /// </summary>

    public decimal ClientDiscount =>
        AfterProductDiscount * Order.DiscountClient / 100m;

    /// <summary>

    /// Итоговая сумма заказа.

    /// </summary>

    public decimal Total => AfterProductDiscount - ClientDiscount;

    public bool HasProductDiscount => ProductDiscount > 0;

    public bool HasClientDiscount  => Order.DiscountClient > 0;

    public bool IsStatusActive => Order.StatusId == "1";

    /// <summary>

    /// Подпись с количеством товаров в заказе.

    /// </summary>

    public string ItemCountLabel
    {
        get
        {
            int c = OrderItems.Count;
            string word = c switch { 1 => "товар", 2 or 3 or 4 => "товара", _ => "товаров" };
            return $"{c} {word}";
        }
    }

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
