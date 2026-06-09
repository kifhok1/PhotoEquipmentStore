using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class OrderConfirmViewModel : ReactiveObject
{
    // ── Данные заказа ─────────────────────────────────────────────────────────

    public string OrderNumber { get; }
    public string CreatedAt   { get; }

    // Клиент
    public string ClientName  { get; }
    public string ClientPhone { get; }

    // Позиции
    public ObservableCollection<OrderConfirmShow> Items { get; }

    // Итоги
    public int Subtotal  { get; }
    public int Discount  { get; }
    public int Delivery  { get; }
    public int Total     { get; }

    public string ItemCountLabel =>
        $"{Items.Count} {PluralItems(Items.Count)}";

    // ── Команды ───────────────────────────────────────────────────────────────

    public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand  { get; }

    // ── Callbacks ─────────────────────────────────────────────────────────────

    private readonly Action _onConfirm;
    private readonly Action _onCancel;

    // ── Конструктор ───────────────────────────────────────────────────────────

    public OrderConfirmViewModel(
        ClientShow client,
        ObservableCollection<OrderCartItem> cartItems,
        int delivery,
        Action onConfirm,
        Action onCancel)
    {
        // Генерируем номер заказа — 8 случайных цифр
        OrderNumber = GenerateOrderNumber();
        CreatedAt   = DateTime.Now.ToString("d MMMM yyyy, HH:mm",
                          new System.Globalization.CultureInfo("ru-RU"));

        ClientName  = client.Name;
        ClientPhone = client.PhoneNumber;

        Items = new ObservableCollection<OrderConfirmShow>(
            cartItems.Select(OrderConfirmShow.FromCartItem));

        Subtotal = cartItems.Sum(i => i.Price * i.CartQuantity);
        Discount = cartItems.Sum(i => i.LineDiscount);
        Delivery = delivery;
        Total    = Subtotal - Discount + Delivery;

        _onConfirm = onConfirm;
        _onCancel  = onCancel;

        ConfirmCommand = ReactiveCommand.Create(() => _onConfirm());
        CancelCommand  = ReactiveCommand.Create(() => _onCancel());
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string GenerateOrderNumber()
    {
        var rng = new Random();
        return string.Concat(Enumerable.Range(0, 8).Select(_ => rng.Next(0, 10)));
    }

    private static string PluralItems(int count) => count switch
    {
        1 => "товар",
        2 or 3 or 4 => "товара",
        _ => "товаров"
    };
}