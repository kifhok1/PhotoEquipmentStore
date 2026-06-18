
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

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;/// <summary>
/// ViewModel подтверждения заказа и сохранения PDF-чека.
/// </summary>


public class OrderConfirmViewModel : ViewModelBase, IStorageProviderReceiver
{

    /// <summary>

    /// Уникальный номер заказа.

    /// </summary>

    public string OrderNumber { get; }
    /// <summary>
    /// Дата и время создания заказа.
    /// </summary>
    public string CreatedAt   { get; }

    /// <summary>

    /// Имя клиента заказа.

    /// </summary>

    public string ClientName          { get; }
    /// <summary>
    /// Телефон клиента заказа.
    /// </summary>
    public string ClientPhone         { get; }
    /// <summary>
    /// Текстовая метка скидки клиента.
    /// </summary>
    public string ClientDiscountLabel { get; }

    /// <summary>

    /// Полная коллекция элементов для постраничного отображения.

    /// </summary>

    public ObservableCollection<OrderConfirmShow> Items { get; }

    /// <summary>

    /// Сумма позиций без скидок.

    /// </summary>

    public int Subtotal          { get; }
    /// <summary>
    /// Сумма скидки по товарам.
    /// </summary>
    public int ProductDiscount   { get; }
    /// <summary>
    /// Сумма накопительной скидки клиента.
    /// </summary>
    public int ClientDiscount    { get; }
    public int ClientDiscountPct { get; }
    /// <summary>
    /// Размер скидки.
    /// </summary>
    public int Discount          { get; }
    public int Delivery          { get; }
    /// <summary>
    /// Итоговая сумма заказа.
    /// </summary>
    public int Total             { get; }
    /// <summary>
    /// Продавец, оформивший заказ.
    /// </summary>
    public UserInfo Seller       { get; }

    /// <summary>

    /// Подпись с количеством товаров в заказе.

    /// </summary>

    public string ItemCountLabel =>
        $"{Items.Count} {PluralItems(Items.Count)}";

    private bool _isConfirmed;
    /// <summary>
    /// Признак успешного создания заказа.
    /// </summary>
    public bool IsConfirmed
    {
        get => _isConfirmed;
        private set => this.RaiseAndSetIfChanged(ref _isConfirmed, value);
    }

    private bool _receiptSaved;
    /// <summary>
    /// Признак сохранения чека на диск.
    /// </summary>
    public bool ReceiptSaved
    {
        get => _receiptSaved;
        private set => this.RaiseAndSetIfChanged(ref _receiptSaved, value);
    }

    private IStorageProvider? _storageProvider;
    /// <summary>
    /// Провайдер диалогов выбора файлов и папок.
    /// </summary>
    public IStorageProvider? StorageProvider
    {
        get => _storageProvider;
        set => this.RaiseAndSetIfChanged(ref _storageProvider, value);
    }

    /// <summary>

    /// Команда подтверждения и создания заказа в БД.

    /// </summary>

    public ReactiveCommand<Unit, Unit> ConfirmCommand     { get; }
    /// <summary>
    /// Команда возврата к оформлению заказа.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoBackCommand      { get; }
    /// <summary>
    /// Команда сохранения PDF-чека.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveReceiptCommand { get; }

    private readonly int                 _clientId;
    private readonly List<OrderCartItem> _cartSnapshot;
    private readonly Action              _onConfirm;
    private readonly Action              _goBack;
    private readonly OrderService        _orderService  = new();
    private readonly OrderCommands       _orderCommands = new();
    private readonly IReceiptPdfService  _receiptPdfService;

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

        OrderNumber = GenerateUniqueOrderNumber();

        ConfirmCommand = ReactiveCommand.CreateFromTask(
            ExecuteConfirmAsync,
            this.WhenAnyValue(x => x.IsConfirmed, confirmed => !confirmed));

        GoBackCommand = ReactiveCommand.Create(() => _goBack());

        SaveReceiptCommand = ReactiveCommand.CreateFromTask(
            ExecuteSaveReceiptAsync,
            this.WhenAnyValue(x => x.IsConfirmed));
    }

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
                    DiscountPercent = i.Price > 0
                        ? Math.Round((decimal)i.Discount / i.Price * 100, 2)
                        : 0
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
        catch {  }

        IStorageFolder? startFolder = null;
        try { startFolder = await _storageProvider.TryGetFolderFromPathAsync(defaultDir); }
        catch {  }

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
