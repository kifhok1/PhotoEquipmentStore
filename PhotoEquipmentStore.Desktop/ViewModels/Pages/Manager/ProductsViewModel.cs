using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;

public class ProductsViewModel : ViewModelBase
{

    public ObservableCollection<ProductsShow> Products
    {
        get; 
        private set;
    } = new();
    
    private string countProducts;
    public string CountProducts
    {
        get => countProducts;
        set => countProducts = value;
    }
    
    private ObservableCollection<ProductsShow> currentProducts = new();
    public ObservableCollection<ProductsShow> CurrentProducts
    {
        get => currentProducts;
        set => this.RaiseAndSetIfChanged(ref currentProducts, value);
    }
    
    public ProductsViewModel()
    {
        var productsToDb = ProductsService.GetProducts();
        foreach (var product in productsToDb)
        {
            Products.Add(new ProductsShow(product.Id, product.Name, product.Price, 
                product.Discount, product.Quantity, product.CategoryId, product.CategoryName,
                product.ManufacturerId, product.ManufacturerName, product.SupplierId, product.SupplierName,
                product.Description, BitmapHelper.FromBytes(product.Image)));
        }
        CountProducts = $"Количество элементов на форме: {Products.Count}";
    }
}