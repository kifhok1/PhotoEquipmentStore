using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Models;
using ReactiveUI;
using System.Windows.Input;
using Avalonia;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Infrastructure.Services;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;/// <summary>
/// ViewModel оформления заказа: выбор клиента, товаров и корзина.
/// </summary>


public class OrderAddViewModel : ViewModelBase
{

    private readonly ProductsService  _productsService  = new();
    private readonly ClientsService   _clientsService   = new();
    private readonly ReferenceService _referenceService = new();
    private readonly Action<OrderConfirmViewModel> _goToConfirm;
    private readonly Action _goBackToAdd;

    /// <summary>

    /// Признак видимости поля поиска клиента.

    /// </summary>

    public bool IsClientSearchVisible => _selectedClient == null;

    public Func<string, Task<string?>>? SaveReceiptDelegate { get; set; }

    private ObservableCollection<OrderProductShow> _allProducts = new();

    private ObservableCollection<OrderProductShow> _filteredProducts = new();
    /// <summary>
    /// Отфильтрованный список товаров.
    /// </summary>
    public ObservableCollection<OrderProductShow> FilteredProducts
    {
        get => _filteredProducts;
        set => this.RaiseAndSetIfChanged(ref _filteredProducts, value);
    }

    /// <summary>

    /// Список категорий для фильтра.

    /// </summary>

    public ObservableCollection<string> Categories    { get; } = new();
    /// <summary>
    /// Продавец, оформивший заказ.
    /// </summary>
    public UserInfo Seller { get; }
    /// <summary>
    /// Список производителей.
    /// </summary>
    public ObservableCollection<string> Manufacturers { get; } = new();

    private string _searchText = string.Empty;
    /// <summary>
    /// Строка поиска по названию товара.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            ApplyFilters();
        }
    }

    private CornerRadius _clientDropDownCornerRadius = new CornerRadius(6);
    public CornerRadius ClientDropDownCornerRadius
    {
        get => _clientDropDownCornerRadius;
        private set => this.RaiseAndSetIfChanged(ref _clientDropDownCornerRadius, value);
    }

    private string? _selectedCategory;
    /// <summary>
    /// Выбранная категория фильтра.
    /// </summary>
    public string? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCategory, value);
            ApplyFilters();
        }
    }

    private string? _selectedManufacturer;
    /// <summary>
    /// Выбранный производитель товара.
    /// </summary>
    public string? SelectedManufacturer
    {
        get => _selectedManufacturer;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedManufacturer, value);
            ApplyFilters();
        }
    }

    private OrderProductShow? _selectedProduct;
    /// <summary>
    /// Выбранный товар для детального просмотра.
    /// </summary>
    public OrderProductShow? SelectedProduct
    {
        get => _selectedProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
    }

    private bool _isProductPanelVisible;
    /// <summary>
    /// Признак видимости панели деталей товара.
    /// </summary>
    public bool IsProductPanelVisible
    {
        get => _isProductPanelVisible;
        set => this.RaiseAndSetIfChanged(ref _isProductPanelVisible, value);
    }

    /// <summary>

    /// Позиции корзины заказа.

    /// </summary>

    public ObservableCollection<OrderCartItem> CartItems { get; } = new();

    private bool _isCartPanelVisible = true;
    /// <summary>
    /// Признак видимости панели корзины.
    /// </summary>
    public bool IsCartPanelVisible
    {
        get => _isCartPanelVisible;
        set => this.RaiseAndSetIfChanged(ref _isCartPanelVisible, value);
    }

    private int _cartItemCount;
    /// <summary>
    /// Количество позиций в корзине.
    /// </summary>
    public int CartItemCount
    {
        get => _cartItemCount;
        private set => this.RaiseAndSetIfChanged(ref _cartItemCount, value);
    }

    private int _cartSubtotal;
    /// <summary>
    /// Сумма корзины без учёта скидок.
    /// </summary>
    public int CartSubtotal
    {
        get => _cartSubtotal;
        private set => this.RaiseAndSetIfChanged(ref _cartSubtotal, value);
    }

    private int _cartDiscount;
    /// <summary>
    /// Сумма скидок по корзине.
    /// </summary>
    public int CartDiscount
    {
        get => _cartDiscount;
        private set => this.RaiseAndSetIfChanged(ref _cartDiscount, value);
    }

    private int _cartTotal;
    /// <summary>
    /// Итоговая сумма корзины.
    /// </summary>
    public int CartTotal
    {
        get => _cartTotal;
        private set => this.RaiseAndSetIfChanged(ref _cartTotal, value);
    }

    private List<ClientShow> _allClients = new();
    /// <summary>
    /// Отфильтрованный список клиентов.
    /// </summary>
    public ObservableCollection<ClientShow> FilteredClients { get; } = new();

    private ClientShow? _selectedClient;
    /// <summary>
    /// Выбранный клиент заказа.
    /// </summary>
    public ClientShow? SelectedClient
    {
        get => _selectedClient;
        set
        {
            if (_selectedClient != null)
                _selectedClient.IsSelected = false;

            this.RaiseAndSetIfChanged(ref _selectedClient, value);

            if (_selectedClient != null)
                _selectedClient.IsSelected = true;

            _clientSearchText = value?.DisplayLabel ?? string.Empty;
            this.RaisePropertyChanged(nameof(ClientSearchText));
            this.RaisePropertyChanged(nameof(IsClientSearchVisible));
            IsClientDropDownOpen = false;
        }
    }

    private string _clientSearchText = string.Empty;
    /// <summary>
    /// Строка поиска клиента.
    /// </summary>
    public string ClientSearchText
    {
        get => _clientSearchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _clientSearchText, value);
            _selectedClient = null;
            this.RaisePropertyChanged(nameof(SelectedClient));
            IsClientDropDownOpen = true;
            ApplyClientFilter();
        }
    }

    private bool _isClientDropDownOpen;
    /// <summary>
    /// Признак открытого выпадающего списка клиентов.
    /// </summary>
    public bool IsClientDropDownOpen
    {
        get => _isClientDropDownOpen;
        set
        {
            this.RaiseAndSetIfChanged(ref _isClientDropDownOpen, value);
            ClientDropDownCornerRadius = value
                ? new CornerRadius(6, 6, 0, 0)
                : new CornerRadius(6);
        }
    }

    private string? _errorMessage;
    /// <summary>
    /// Текст ошибки CAPTCHA.
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            this.RaiseAndSetIfChanged(ref _errorMessage, value);
            this.RaisePropertyChanged(nameof(HasError));
        }
    }

    /// <summary>

    /// Признак наличия ошибки на форме.

    /// </summary>

    public bool HasError => !string.IsNullOrEmpty(_errorMessage);

    /// <summary>

    /// Команда выбора товара для детального просмотра.

    /// </summary>

    public ICommand SelectProductCommand      { get; }
    /// <summary>
    /// Команда сброса выбранного товара.
    /// </summary>
    public ICommand ClearSelectionCommand     { get; }
    /// <summary>
    /// Команда добавления выбранного товара в корзину.
    /// </summary>
    public ICommand AddSelectedToCartCommand  { get; }
    /// <summary>
    /// Команда добавления товара в корзину.
    /// </summary>
    public ICommand AddToCartCommand          { get; }
    /// <summary>
    /// Команда удаления позиции из корзины.
    /// </summary>
    public ICommand RemoveFromCartCommand     { get; }
    /// <summary>
    /// Команда увеличения количества в корзине.
    /// </summary>
    public ICommand IncreaseQuantityCommand   { get; }
    /// <summary>
    /// Команда уменьшения количества в корзине.
    /// </summary>
    public ICommand DecreaseQuantityCommand   { get; }
    /// <summary>
    /// Команда очистки корзины.
    /// </summary>
    public ICommand ClearCartCommand          { get; }
    /// <summary>
    /// Команда сброса фильтров поиска товаров.
    /// </summary>
    public ICommand ClearSearchCommand        { get; }
    /// <summary>
    /// Команда перехода к подтверждению заказа.
    /// </summary>
    public ICommand ConfirmOrderCommand       { get; }
    /// <summary>
    /// Команда сворачивания/разворачивания панели товара.
    /// </summary>
    public ICommand ToggleProductPanelCommand { get; }
    /// <summary>
    /// Команда сворачивания/разворачивания панели корзины.
    /// </summary>
    public ICommand ToggleCartPanelCommand    { get; }
    /// <summary>
    /// Команда выбора клиента из списка.
    /// </summary>
    public ICommand SelectClientCommand       { get; }

    public OrderAddViewModel(Action<OrderConfirmViewModel> goToConfirm, Action goBackToAdd, UserInfo seller)
    {
        _goToConfirm  = goToConfirm;
        _goBackToAdd  = goBackToAdd;
        Seller = seller;

        SelectProductCommand = ReactiveCommand.Create<OrderProductShow>(p =>
        {
            SelectedProduct = p;
            IsProductPanelVisible = true;
        });

        ClearSelectionCommand = ReactiveCommand.Create(() =>
        {
            SelectedProduct = null;
            IsProductPanelVisible = false;
        });

        AddSelectedToCartCommand = ReactiveCommand.Create(() =>
        {
            if (SelectedProduct != null)
                AddToCart(SelectedProduct);
        });

        AddToCartCommand        = ReactiveCommand.Create<OrderProductShow>(AddToCart);
        RemoveFromCartCommand   = ReactiveCommand.Create<OrderCartItem>(RemoveFromCart);
        IncreaseQuantityCommand = ReactiveCommand.Create<OrderCartItem>(IncreaseQuantity);
        DecreaseQuantityCommand = ReactiveCommand.Create<OrderCartItem>(DecreaseQuantity);
        ClearCartCommand        = ReactiveCommand.Create(ClearCart);
        ClearSearchCommand      = ReactiveCommand.Create(ClearSearch);
        ConfirmOrderCommand     = ReactiveCommand.Create(ConfirmOrder);

        ToggleProductPanelCommand = ReactiveCommand.Create(
            () => IsProductPanelVisible = !IsProductPanelVisible);
        ToggleCartPanelCommand = ReactiveCommand.Create(
            () => IsCartPanelVisible = !IsCartPanelVisible);

        SelectClientCommand = ReactiveCommand.Create<ClientShow>(c => SelectedClient = c);

        LoadData();
    }

    private void LoadData()
    {
        LoadProducts();
        LoadClients();
        LoadReferenceFilters();
    }

    private void LoadProducts()
    {
        var result = _productsService.GetProducts();
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage;
            return;
        }

        var shows = result.Products
            .Select(p => new OrderProductShow(p, TryLoadBitmap(p.Image)))
            .ToList();

        _allProducts = new ObservableCollection<OrderProductShow>(shows);
        ApplyFilters();
    }

    private void LoadClients()
    {
        var result = _clientsService.GetClients();
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage;
            return;
        }

        _allClients = result.Clients
            .Select(c => new ClientShow(
                c.Id,
                c.FullName,
                c.Phone,
                c.TotalPurchases.ToString() ?? "0",
                c.CountOrders))
            .ToList();

        ApplyClientFilter();
    }

    private void LoadReferenceFilters()
    {
        var catResult = _referenceService.GetCategories();
        Categories.Clear();
        Categories.Add("Все категории");
        if (catResult.IsSuccess)
            foreach (var r in catResult.References)
                Categories.Add(r.Name);
        SelectedCategory = Categories[0];

        var mfrResult = _referenceService.GetManufacturers();
        Manufacturers.Clear();
        Manufacturers.Add("Все бренды");
        if (mfrResult.IsSuccess)
            foreach (var r in mfrResult.References)
                Manufacturers.Add(r.Name);
        SelectedManufacturer = Manufacturers[0];
    }

    private static Bitmap? TryLoadBitmap(byte[]? imageData)
    {
        if (imageData == null || imageData.Length == 0) return null;
        try
        {
            using var ms = new MemoryStream(imageData);
            return new Bitmap(ms);
        }
        catch { return null; }
    }

    private void ApplyFilters()
    {
        var result = _allProducts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            result = result.Where(p =>
                p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        if (SelectedCategory != null && SelectedCategory != "Все категории")
            result = result.Where(p => p.CategoryName == SelectedCategory);

        if (SelectedManufacturer != null && SelectedManufacturer != "Все бренды")
            result = result.Where(p => p.ManufacturerName == SelectedManufacturer);

        FilteredProducts = new ObservableCollection<OrderProductShow>(result);
    }

    private void ApplyClientFilter()
    {
        FilteredClients.Clear();

        var query = string.IsNullOrWhiteSpace(ClientSearchText)
            ? _allClients
            : _allClients.Where(c =>
                c.Name.Contains(ClientSearchText, StringComparison.OrdinalIgnoreCase) ||
                c.PhoneNumber.Contains(ClientSearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var c in query)
            FilteredClients.Add(c);
    }

    private void AddToCart(OrderProductShow product)
    {
        ErrorMessage = null;

        if (product.IsMaxReached)
            return;

        var existing = CartItems.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
            existing.CartQuantity++;
        else
            CartItems.Add(new OrderCartItem(product));

        product.CartQuantity++;
        IsCartPanelVisible = true;
        RecalcTotals();
    }

    private void RemoveFromCart(OrderCartItem item)
    {
        var product = _allProducts.FirstOrDefault(p => p.Id == item.ProductId);
        if (product != null)
            product.CartQuantity = 0;

        CartItems.Remove(item);
        RecalcTotals();
    }

    private void IncreaseQuantity(OrderCartItem item)
    {
        ErrorMessage = null;

        var product = _allProducts.FirstOrDefault(p => p.Id == item.ProductId);
        if (product != null && product.IsMaxReached)
            return;

        item.CartQuantity++;
        if (product != null)
            product.CartQuantity++;

        RecalcTotals();
    }

    private void DecreaseQuantity(OrderCartItem item)
    {
        ErrorMessage = null;

        var product = _allProducts.FirstOrDefault(p => p.Id == item.ProductId);

        if (item.CartQuantity > 1)
        {
            item.CartQuantity--;
            if (product != null)
                product.CartQuantity--;
        }
        else
        {
            if (product != null)
                product.CartQuantity = 0;
            CartItems.Remove(item);
        }

        RecalcTotals();
    }

    private void ClearCart()
    {
        foreach (var item in CartItems)
        {
            var product = _allProducts.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
                product.CartQuantity = 0;
        }

        CartItems.Clear();
        ErrorMessage = null;
        RecalcTotals();
    }

    private void RecalcTotals()
    {
        CartItemCount = CartItems.Count;
        CartSubtotal  = CartItems.Sum(i => i.Price * i.CartQuantity);
        CartDiscount  = CartItems.Sum(i => i.LineDiscount);
        CartTotal     = CartItems.Sum(i => i.LineTotal);
    }

    private void ClearSearch()
    {
        SearchText           = string.Empty;
        SelectedCategory     = Categories.FirstOrDefault();
        SelectedManufacturer = Manufacturers.FirstOrDefault();
    }

    private void ConfirmOrder()
    {
        ErrorMessage = null;

        if (SelectedClient == null)
        {
            ErrorMessage = "Выберите клиента перед подтверждением заказа.";
            return;
        }

        if (!CartItems.Any())
        {
            ErrorMessage = "Корзина пуста.";
            return;
        }

        var vm = new OrderConfirmViewModel(
            client:            SelectedClient,
            cartItems:         CartItems,
            delivery:          0,
            seller:            Seller,
            onConfirm:         OnOrderConfirmed,
            goBack:            _goBackToAdd,
            receiptPdfService: new ReceiptPdfService()
        );

        _goToConfirm(vm);
    }

    private void OnOrderConfirmed()
    {
        ClearCart();
        _clientSearchText = string.Empty;
        this.RaisePropertyChanged(nameof(ClientSearchText));
        this.RaisePropertyChanged(nameof(IsClientSearchVisible));
        SelectedClient = null;
    }
}
