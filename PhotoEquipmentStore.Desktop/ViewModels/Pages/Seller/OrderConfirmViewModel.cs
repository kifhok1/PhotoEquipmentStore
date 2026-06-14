// ViewModels/Pages/Seller/OrderConfirmViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Behaviors;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class OrderConfirmViewModel : ViewModelBase, IStorageProviderReceiver
{
    // ── Данные заказа ─────────────────────────────────────────────────────────

    public string OrderNumber { get; }
    public string CreatedAt   { get; }

    public string ClientName          { get; }
    public string ClientPhone         { get; }
    public string ClientDiscountLabel { get; }

    public ObservableCollection<OrderConfirmShow> Items { get; }

    public int Subtotal          { get; }
    public int ProductDiscount   { get; }
    public int ClientDiscount    { get; }
    public int ClientDiscountPct { get; }
    public int Discount          { get; }
    public int Delivery          { get; }
    public int Total             { get; }
    public UserInfo Seller       { get; }

    public string ItemCountLabel =>
        $"{Items.Count} {PluralItems(Items.Count)}";

    // ── Состояние ─────────────────────────────────────────────────────────────

    private bool _isConfirmed;
    public bool IsConfirmed
    {
        get => _isConfirmed;
        private set => this.RaiseAndSetIfChanged(ref _isConfirmed, value);
    }

    private bool _receiptSaved;
    public bool ReceiptSaved
    {
        get => _receiptSaved;
        private set => this.RaiseAndSetIfChanged(ref _receiptSaved, value);
    }

    // ── IStorageProviderReceiver ──────────────────────────────────────────────

    private IStorageProvider? _storageProvider;
    public IStorageProvider? StorageProvider
    {
        get => _storageProvider;
        set => this.RaiseAndSetIfChanged(ref _storageProvider, value);
    }

    // ── Команды ───────────────────────────────────────────────────────────────

    public ReactiveCommand<Unit, Unit> ConfirmCommand     { get; }
    public ReactiveCommand<Unit, Unit> GoBackCommand      { get; }
    public ReactiveCommand<Unit, Unit> SaveReceiptCommand { get; }

    // ── Зависимости ───────────────────────────────────────────────────────────

    private readonly int                 _clientId;
    private readonly List<OrderCartItem> _cartSnapshot;
    private readonly Action              _onConfirm;
    private readonly Action              _goBack;
    private readonly OrderService        _orderService  = new();
    private readonly OrderCommands       _orderCommands = new();
    private readonly IReceiptPdfService  _receiptPdfService;

    // ── Конструктор ───────────────────────────────────────────────────────────

    public OrderConfirmViewModel(
        ClientShow client,
        ObservableCollection<OrderCartItem> cartItems,
        int delivery,
        UserInfo seller,
        Action onConfirm,
        Action goBack,
        IReceiptPdfService receiptPdfService)
    {
        _cartSnapshot = cartItems.ToList();

        CreatedAt           = DateTime.Now.ToString("d MMMM yyyy, HH:mm",
                                  new System.Globalization.CultureInfo("ru-RU"));
        _clientId           = client.Id;
        ClientName          = client.Name;
        ClientPhone         = client.PhoneNumber;
        ClientDiscountLabel = client.ClientDiscountLabel;
        ClientDiscountPct   = client.ClientDiscountPercent;
        Seller              = seller;

        Items = new ObservableCollection<OrderConfirmShow>(
            _cartSnapshot.Select(OrderConfirmShow.FromCartItem));

        Subtotal        = _cartSnapshot.Sum(i => i.Price * i.CartQuantity);
        ProductDiscount = _cartSnapshot.Sum(i => i.LineDiscount);
        ClientDiscount  = (Subtotal - ProductDiscount) * ClientDiscountPct / 100;
        Discount        = ProductDiscount + ClientDiscount;
        Delivery        = delivery;
        Total           = Subtotal - Discount + Delivery;

        _onConfirm         = onConfirm;
        _goBack            = goBack;
        _receiptPdfService = receiptPdfService;

        // Номер генерируется последним — делает запрос в БД
        OrderNumber = GenerateUniqueOrderNumber();

        ConfirmCommand = ReactiveCommand.CreateFromTask(
            ExecuteConfirmAsync,
            this.WhenAnyValue(x => x.IsConfirmed, confirmed => !confirmed));

        GoBackCommand = ReactiveCommand.Create(() => _goBack());

        SaveReceiptCommand = ReactiveCommand.CreateFromTask(
            ExecuteSaveReceiptAsync,
            this.WhenAnyValue(x => x.IsConfirmed));
    }

    // ── Подтверждение заказа ─────────────────────────────────────────────────

    private async Task ExecuteConfirmAsync()
    {
        bool confirmed = await NotificationService.Instance.ShowWarningAsync(
            "Подтвердить заказ?",
            $"Будет создан заказ #{OrderNumber} на сумму {Total:N0} ₽.\n" +
            $"Товары будут списаны со склада. Продолжить?");

        if (!confirmed) return;

        try
        {
            var dto = new CreateOrderDto
            {
                OrderArticle    = OrderNumber,
                ClientId        = _clientId,
                EmployeeId      = Seller.UserId,
                DiscountPercent = ClientDiscountPct,
                TotalAmount     = Total,
                Items = _cartSnapshot.Select(i => new CreateOrderItemDto
                {
                    ProductId       = i.ProductId,
                    Quantity        = i.CartQuantity,
                    Price           = i.Price,
                    DiscountPercent = i.Discount
                }).ToList()
            };

            var result = _orderService.CreateOrder(dto);

            if (!result.IsSuccess)
            {
                await NotificationService.Instance.ShowErrorAsync(
                    "Ошибка создания заказа",
                    result.ErrorMessage!);
                return;
            }

            IsConfirmed = true;
            _onConfirm();

            await NotificationService.Instance.ShowInfoAsync(
                "Заказ создан",
                $"Заказ #{OrderNumber} успешно оформлен.\n" +
                $"Клиент: {ClientName}\n" +
                $"Итого: {Total:N0} ₽");
        }
        catch (Exception ex)
        {
            await NotificationService.Instance.ShowErrorAsync(
                "Непредвиденная ошибка",
                $"Не удалось создать заказ.\n{ex.Message}");
        }
    }

    // ── Сохранение чека ───────────────────────────────────────────────────────

    private async Task ExecuteSaveReceiptAsync()
    {
        if (_storageProvider is null)
        {
            await NotificationService.Instance.ShowErrorAsync(
                "Ошибка",
                "Не удалось открыть диалог сохранения файла.");
            return;
        }

        string defaultDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "ФотоМагазин");

        try { Directory.CreateDirectory(defaultDir); }
        catch { /* не критично */ }

        IStorageFolder? startFolder = null;
        try { startFolder = await _storageProvider.TryGetFolderFromPathAsync(defaultDir); }
        catch { /* откроется в папке по умолчанию */ }

        IStorageFile? file;
        try
        {
            file = await _storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title                  = "Сохранить чек",
                SuggestedFileName      = $"Чек_{OrderNumber}.pdf",
                DefaultExtension       = "pdf",
                SuggestedStartLocation = startFolder,
                FileTypeChoices        =
                [
                    new FilePickerFileType("PDF документ") { Patterns = ["*.pdf"] }
                ]
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OrderConfirmVM] SaveFilePicker error: {ex}");
            await NotificationService.Instance.ShowErrorAsync(
                "Ошибка",
                "Не удалось открыть диалог сохранения.");
            return;
        }

        if (file is null) return;

        try
        {
            string savePath = file.Path.LocalPath;
            var (success, error) = await _receiptPdfService.SaveReceiptAsync(BuildReceiptData(), savePath);

            if (success)
            {
                ReceiptSaved = true;
                await NotificationService.Instance.ShowInfoAsync(
                    "Чек сохранён",
                    $"Файл успешно сохранён:\n{savePath}");
            }
            else
            {
                await NotificationService.Instance.ShowErrorAsync(
                    "Ошибка сохранения",
                    error ?? "Не удалось создать PDF-файл чека.");
            }
        }
        catch (Exception ex)
        {
            await NotificationService.Instance.ShowErrorAsync(
                "Непредвиденная ошибка",
                $"Не удалось сохранить чек.\n{ex.Message}");
        }
    }

    // ── Построение данных чека ────────────────────────────────────────────────

    private ReceiptData BuildReceiptData() => new()
    {
        OrderNumber       = OrderNumber,
        CreatedAt         = CreatedAt,
        ClientName        = ClientName,
        ClientPhone       = ClientPhone,
        ClientDiscount    = ClientDiscountLabel,
        SellerName        = Seller.UserName,
        Items             = Items.Select(i => new ReceiptItem
        {
            Name          = i.Name,
            Quantity      = i.Quantity,
            OriginalPrice = i.Price,
            FinalPrice    = i.FinalPrice,
            LineTotal     = i.LineTotal
        }).ToList(),
        Subtotal          = Subtotal,
        ProductDiscount   = ProductDiscount,
        ClientDiscountPct = ClientDiscountPct,
        ClientDiscountAmt = ClientDiscount,
        Discount          = Discount,
        Delivery          = Delivery,
        Total             = Total
    };

    // ── Helpers ───────────────────────────────────────────────────────────────

    private string GenerateUniqueOrderNumber()
    {
        var rng = new Random();

        string Candidate() =>
            string.Concat(Enumerable.Range(0, 8).Select(_ => rng.Next(0, 10)));

        string article;
        do { article = Candidate(); }
        while (_orderCommands.ArticleExists(article));

        return article;
    }

    private static string PluralItems(int count) => count switch
    {
        1           => "товар",
        2 or 3 or 4 => "товара",
        _           => "товаров"
    };
}