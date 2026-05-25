using System;
using System.Collections.ObjectModel;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;

public class ProductsViewModel : ViewModelBase
{
    public ObservableCollection<ProductsShow> Products { get; } = new();

    private string _countProducts = string.Empty;
    public string CountProducts
    {
        get => _countProducts;
        set => this.RaiseAndSetIfChanged(ref _countProducts, value);
    }

    private ObservableCollection<ProductsShow> _currentProducts = new();
    public ObservableCollection<ProductsShow> CurrentProducts
    {
        get => _currentProducts;
        set => this.RaiseAndSetIfChanged(ref _currentProducts, value);
    }

    public ReactiveCommand<ProductsShow, Unit> EditCommand   { get; }
    public ReactiveCommand<ProductsShow, Unit> DeleteCommand { get; }

    public ProductsViewModel(Action<ProductsShow>? goToEdit = null)
    {
        EditCommand = ReactiveCommand.Create<ProductsShow>(
            item => goToEdit?.Invoke(item));

        DeleteCommand = ReactiveCommand.Create<ProductsShow>(item =>
        {
            // TODO: ProductsService.Delete(item.Id)
            Products.Remove(item);
            CurrentProducts.Remove(item);
            CountProducts = $"Количество элементов на форме: {Products.Count}";
        });

        var productsToDb = ProductsService.GetProducts();
        foreach (var product in productsToDb)
            Products.Add(new ProductsShow(
                product.Id, product.Name, product.Price,
                product.Discount, product.Quantity,
                product.CategoryId, product.CategoryName,
                product.ManufacturerId, product.ManufacturerName,
                product.SupplierId, product.SupplierName,
                product.Description, BitmapHelper.FromBytes(product.Image)));

        CountProducts = $"Количество элементов на форме: {Products.Count}";
    }
}