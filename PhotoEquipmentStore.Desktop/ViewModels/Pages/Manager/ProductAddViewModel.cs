using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;/// <summary>
/// ViewModel создания и редактирования товара.
/// </summary>


public class ProductAddViewModel : ViewModelBase
{
    private readonly Action        _goBack;
    private readonly ProductsShow? _editItem;
    private ProductsService _productsService = new ProductsService();
    private ReferenceService _referenceService = new ReferenceService();

    /// <summary>

    /// Признак режима редактирования существующей записи.

    /// </summary>

    public bool   IsEdit    => _editItem is not null;
    /// <summary>
    /// Заголовок страницы формы.
    /// </summary>
    public string PageTitle => IsEdit ? "Редактировать товар" : "Создать товар";

    private string _name = string.Empty;
    /// <summary>
    /// Наименование или ФИО.
    /// </summary>
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _description = string.Empty;
    /// <summary>
    /// Описание товара.
    /// </summary>
    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    private string _price = string.Empty;
    /// <summary>
    /// Цена товара.
    /// </summary>
    public string Price
    {
        get => _price;
        set => this.RaiseAndSetIfChanged(ref _price, value);
    }

    private string _quantity = string.Empty;
    /// <summary>
    /// Количество на складе или в позиции.
    /// </summary>
    public string Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    private string _discount = string.Empty;
    /// <summary>
    /// Размер скидки.
    /// </summary>
    public string Discount
    {
        get => _discount;
        set => this.RaiseAndSetIfChanged(ref _discount, value);
    }

    private ReferenceShow? _selectedCategory;
    /// <summary>
    /// Выбранная категория фильтра.
    /// </summary>
    public ReferenceShow? SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    private ReferenceShow? _selectedManufacturer;
    /// <summary>
    /// Выбранный производитель товара.
    /// </summary>
    public ReferenceShow? SelectedManufacturer
    {
        get => _selectedManufacturer;
        set => this.RaiseAndSetIfChanged(ref _selectedManufacturer, value);
    }

    private ReferenceShow? _selectedSupplier;
    /// <summary>
    /// Выбранный поставщик товара.
    /// </summary>
    public ReferenceShow? SelectedSupplier
    {
        get => _selectedSupplier;
        set => this.RaiseAndSetIfChanged(ref _selectedSupplier, value);
    }

    private Bitmap? _productImage;
    /// <summary>
    /// Изображение товара.
    /// </summary>
    public Bitmap? ProductImage
    {
        get => _productImage;
        set => this.RaiseAndSetIfChanged(ref _productImage, value);
    }

    /// <summary>

    /// Список категорий для фильтра.

    /// </summary>

    public ObservableCollection<ReferenceShow> Categories    { get; } = new();
    /// <summary>
    /// Список производителей.
    /// </summary>
    public ObservableCollection<ReferenceShow> Manufacturers { get; } = new();
    /// <summary>
    /// Список поставщиков.
    /// </summary>
    public ObservableCollection<ReferenceShow> Suppliers     { get; } = new();

    /// <summary>

    /// Команда сохранения записи.

    /// </summary>

    public ReactiveCommand<Unit, Unit> SaveCommand  { get; }
    /// <summary>
    /// Команда сброса фильтров и поиска.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    public ProductAddViewModel(Action goBack, ProductsShow? editItem = null)
    {
        _goBack   = goBack;
        _editItem = editItem;

        foreach (var item in _referenceService.GetCategories().References)
            Categories.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));
        foreach (var item in _referenceService.GetManufacturers().References)
            Manufacturers.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));
        foreach (var item in _referenceService.GetSuppliers().References)
            Suppliers.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));

        if (editItem is not null)
        {
            _name                = editItem.Name;
            _description         = editItem.Description;
            _price               = editItem.Price.ToString();
            _quantity            = editItem.Quantity.ToString();
            _discount            = editItem.Discount.ToString();
            _selectedCategory    = Categories.FirstOrDefault(c => c.Id == editItem.CategoryId);
            _selectedManufacturer = Manufacturers.FirstOrDefault(m => m.Id == editItem.ManufacturerId);
            _selectedSupplier    = Suppliers.FirstOrDefault(s => s.Id == editItem.SupplierId);
            ProductImage         = editItem.Image;
        }

        var canSave = this.WhenAnyValue(
             x => x.Name,
            x => x.SelectedCategory,
            x => x.SelectedManufacturer,
            x => x.SelectedSupplier,
            x => x.Price,
            x => x.Quantity,
            (name, cat, manuf, supp, price, qty) =>
                !string.IsNullOrWhiteSpace(name)  &&
                cat   != null                     &&
                manuf != null                     &&
                supp  != null                     &&
                !string.IsNullOrWhiteSpace(price) &&
                !string.IsNullOrWhiteSpace(qty));

        SaveCommand  = ReactiveCommand.Create(Save,  canSave);
        ResetCommand = ReactiveCommand.Create(IsEdit ? _goBack : Reset);
    }

    public ProductAddViewModel() : this(() => { }) { }

    private async void Save()
{
    if (IsEdit)
    {
        bool confirmed = await NotificationService.Instance.ShowWarningAsync(
            "Редактировать запись?",
            "Вы действительно хотите изменить данные товара? Это действие нельзя будет отменить.");

        if (!confirmed) return;

        try
        {
            ProductResultDto productDB;

            if (ProductImage is not null)
            {
                productDB = _productsService.UpdateProduct(new Product(
                    id:             _editItem!.Id,
                    name:           Name,
                    description:    Description,
                    price:          Convert.ToInt32(Price),
                    quantity:       Convert.ToInt32(Quantity),
                    discount:       Convert.ToInt32(Discount),
                    categoryId:     SelectedCategory!.Id,
                    manufacturerId: SelectedManufacturer!.Id,
                    supplierId:     SelectedSupplier!.Id,
                    image:          BitmapHelper.ToBytes(ProductImage)
                ));
            }
            else
            {
                productDB = _productsService.UpdateProduct(new Product(
                    id:             _editItem!.Id,
                    name:           Name,
                    description:    Description,
                    price:          Convert.ToInt32(Price),
                    quantity:       Convert.ToInt32(Quantity),
                    discount:       Convert.ToInt32(Discount),
                    categoryId:     SelectedCategory!.Id,
                    manufacturerId: SelectedManufacturer!.Id,
                    supplierId:     SelectedSupplier!.Id
                ));
            }

            if (productDB.IsSuccess)
            {
                await NotificationService.Instance.ShowInfoAsync("Успешно", $"Данные товара «{Name}» изменены.");
                _goBack();
            }
            else
            {
                await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось изменить данные товара. {productDB.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            await NotificationService.Instance.ShowErrorAsync("Ошибка", $"{ex.Message}");
        }
    }
    else
    {
        bool confirmed = await NotificationService.Instance.ShowWarningAsync(
            "Создать запись?",
            "Вы действительно хотите добавить товар?");

        if (!confirmed) return;

        try
        {
            ProductResultDto productDB;

            if (ProductImage is not null)
            {
                productDB = _productsService.CreateProduct(new Product(
                    name:           Name,
                    description:    Description,
                    price:          Convert.ToInt32(Price),
                    quantity:       Convert.ToInt32(Quantity),
                    discount:       Convert.ToInt32(Discount),
                    categoryId:     SelectedCategory!.Id,
                    manufacturerId: SelectedManufacturer!.Id,
                    supplierId:     SelectedSupplier!.Id,
                    image:          BitmapHelper.ToBytes(ProductImage)
                ));
            }
            else
            {
                productDB = _productsService.CreateProduct(new Product(
                    name:           Name,
                    description:    Description,
                    price:          Convert.ToInt32(Price),
                    quantity:       Convert.ToInt32(Quantity),
                    discount:       Convert.ToInt32(Discount),
                    categoryId:     SelectedCategory!.Id,
                    manufacturerId: SelectedManufacturer!.Id,
                    supplierId:     SelectedSupplier!.Id
                ));
            }

            if (productDB.IsSuccess)
            {
                await NotificationService.Instance.ShowInfoAsync("Успешно", $"Товар «{Name}» добавлен.");
            }
            else
            {
                await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось добавить товар. {productDB.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            await NotificationService.Instance.ShowErrorAsync("Ошибка", $"{ex.Message}");
        }
    }
}

    private void Reset()
    {
        Name                 = string.Empty;
        Description          = string.Empty;
        Price                = string.Empty;
        Quantity             = string.Empty;
        Discount             = string.Empty;
        SelectedCategory     = null;
        SelectedManufacturer = null;
        SelectedSupplier     = null;
        ProductImage         = null;
    }
}
