using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;

public class ProductAddViewModel : ViewModelBase
{
    private readonly Action        _goBack;
    private readonly ProductsShow? _editItem;

    public bool   IsEdit    => _editItem is not null;
    public string PageTitle => IsEdit ? "Редактировать товар" : "Создать товар";

    // ── Свойства ─────────────────────────────────────────────────────────────

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    private string _price = string.Empty;
    public string Price
    {
        get => _price;
        set => this.RaiseAndSetIfChanged(ref _price, value);
    }

    private string _quantity = string.Empty;
    public string Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    private string _discount = string.Empty;
    public string Discount
    {
        get => _discount;
        set => this.RaiseAndSetIfChanged(ref _discount, value);
    }

    private ReferenceShow? _selectedCategory;
    public ReferenceShow? SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    private ReferenceShow? _selectedManufacturer;
    public ReferenceShow? SelectedManufacturer
    {
        get => _selectedManufacturer;
        set => this.RaiseAndSetIfChanged(ref _selectedManufacturer, value);
    }

    private ReferenceShow? _selectedSupplier;
    public ReferenceShow? SelectedSupplier
    {
        get => _selectedSupplier;
        set => this.RaiseAndSetIfChanged(ref _selectedSupplier, value);
    }

    private Bitmap? _productImage;
    public Bitmap? ProductImage
    {
        get => _productImage;
        set => this.RaiseAndSetIfChanged(ref _productImage, value);
    }

    // ── Коллекции ────────────────────────────────────────────────────────────

    public ObservableCollection<ReferenceShow> Categories    { get; } = new();
    public ObservableCollection<ReferenceShow> Manufacturers { get; } = new();
    public ObservableCollection<ReferenceShow> Suppliers     { get; } = new();

    // ── Команды ──────────────────────────────────────────────────────────────

    public ReactiveCommand<Unit, Unit> SaveCommand  { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    public ProductAddViewModel(Action goBack, ProductsShow? editItem = null)
    {
        _goBack   = goBack;
        _editItem = editItem;

        foreach (var item in ReferenceService.GetCategories())
            Categories.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));
        foreach (var item in ReferenceService.GetManufacturers())
            Manufacturers.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));
        foreach (var item in ReferenceService.GetSuppliers())
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

    // Конструктор для дизайнера
    public ProductAddViewModel() : this(() => { }) { }

    private void Save()
    {
        byte[]? imageBytes = ProductImage is not null
            ? BitmapHelper.ToBytes(ProductImage)
            : null;

        if (IsEdit)
        {
            // TODO: ProductsService.Update(_editItem!.Id, ...)
        }
        else
        {
            // TODO: ProductsService.Create(...)
        }

        _goBack();
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