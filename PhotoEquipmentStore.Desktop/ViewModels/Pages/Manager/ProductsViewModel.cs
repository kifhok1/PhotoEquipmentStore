using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;/// <summary>
/// ViewModel каталога товаров с поиском, фильтрами и сортировкой.
/// </summary>


public class ProductsViewModel : ViewModelBase
{
    private readonly ProductsService _productsService = new ProductsService();
    private readonly List<ProductsShow> _allProducts = new();

    /// <summary>

    /// Отфильтрованная коллекция товаров.

    /// </summary>

    public ObservableCollection<ProductsShow> Products { get; } = new();

    private ObservableCollection<ProductsShow> _currentProducts = new();
    /// <summary>
    /// Текущая страница товаров (для пагинации).
    /// </summary>
    public ObservableCollection<ProductsShow> CurrentProducts
    {
        get => _currentProducts;
        set => this.RaiseAndSetIfChanged(ref _currentProducts, value);
    }

    private string _countProducts = string.Empty;
    /// <summary>
    /// Подпись с количеством товаров на форме.
    /// </summary>
    public string CountProducts
    {
        get => _countProducts;
        set => this.RaiseAndSetIfChanged(ref _countProducts, value);
    }

    private string _searchText = string.Empty;
    /// <summary>
    /// Строка поиска по названию товара.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set { this.RaiseAndSetIfChanged(ref _searchText, value); ApplyFilters(); }
    }

    /// <summary>

    /// Варианты сортировки по цене.

    /// </summary>

    public List<string> SortOptions { get; } = new()
    {
        "По возрастанию цены",
        "По убыванию цены"
    };

    private string? _selectedSortOption;
    /// <summary>
    /// Выбранный вариант сортировки.
    /// </summary>
    public string? SelectedSortOption
    {
        get => _selectedSortOption;
        set { this.RaiseAndSetIfChanged(ref _selectedSortOption, value); ApplyFilters(); }
    }

    /// <summary>

    /// Список категорий для фильтра.

    /// </summary>

    public ObservableCollection<string> Categories { get; } = new();

    private string? _selectedCategory;
    /// <summary>
    /// Выбранная категория фильтра.
    /// </summary>
    public string? SelectedCategory
    {
        get => _selectedCategory;
        set { this.RaiseAndSetIfChanged(ref _selectedCategory, value); ApplyFilters(); }
    }

    /// <summary>

    /// Команда перехода к редактированию записи.

    /// </summary>

    public ReactiveCommand<ProductsShow, Unit> EditCommand   { get; }
    /// <summary>
    /// Команда удаления записи.
    /// </summary>
    public ReactiveCommand<ProductsShow, Unit> DeleteCommand { get; }
    /// <summary>
    /// Команда сброса фильтров и поиска.
    /// </summary>
    public ReactiveCommand<Unit, Unit>         ResetCommand  { get; }

    public ProductsViewModel(Action<ProductsShow>? goToEdit = null)
    {
        EditCommand = ReactiveCommand.Create<ProductsShow>(
            item =>
            {
                goToEdit?.Invoke(item);
                LoadProducts();
            });

        DeleteCommand = ReactiveCommand.CreateFromTask<ProductsShow>(async item =>
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Удалить запись?",
                $"Вы уверены, что хотите удалить товар - {item.Name}? Это действие нельзя будет отменить.");

            if (confirmed)
            {
                var result = _productsService.DeleteProduct(item.Id);
                if (result.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync("Успешно", $"Товар - {item.Name} удалён.");
                    LoadProducts();
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось удалить товар - {item.Name}.");
                }
            }
        });

        ResetCommand = ReactiveCommand.Create(Reset);

        LoadProducts();
    }

    private async void LoadProducts()
    {
        _allProducts.Clear();
        Products.Clear();

        var productsFromDb = _productsService.GetProducts();
        if (productsFromDb.IsSuccess)
        {
            foreach (var product in productsFromDb.Products)
            {
                var show = new ProductsShow(
                    product.Id, product.Name, product.Price,
                    product.Discount, product.Quantity,
                    product.CategoryId, product.CategoryName,
                    product.ManufacturerId, product.ManufacturerName,
                    product.SupplierId, product.SupplierName,
                    product.Description, BitmapHelper.FromBytes(product.Image));

                _allProducts.Add(show);
                Products.Add(show);
            }

            Categories.Clear();
            foreach (var cat in _allProducts.Select(p => p.CategoryName).Distinct().Order())
                Categories.Add(cat);

            UpdateCount(Products.Count);
        }
        else
        {
            await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось загрузить список товаров.");
            UpdateCount(Products.Count);
        }

    }

    private void ApplyFilters()
    {
        var result = _allProducts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var q = SearchText.Trim().ToLower();
            result = result.Where(p => p.Name.ToLower().Contains(q));
        }

        if (!string.IsNullOrWhiteSpace(SelectedCategory))
            result = result.Where(p => p.CategoryName == SelectedCategory);

        result = SelectedSortOption switch
        {
            "По возрастанию цены" => result.OrderBy(p => ParsePrice(p.Price.ToString())),
            "По убыванию цены"    => result.OrderByDescending(p => ParsePrice(p.Price.ToString())),
            _                     => result
        };

        var list = result.ToList();

        Products.Clear();
        foreach (var p in list) Products.Add(p);

        UpdateCount(Products.Count);
    }

    private void Reset()
    {
        _searchText = string.Empty;
        this.RaisePropertyChanged(nameof(SearchText));

        _selectedSortOption = null;
        this.RaisePropertyChanged(nameof(SelectedSortOption));

        _selectedCategory = null;
        this.RaisePropertyChanged(nameof(SelectedCategory));

        ApplyFilters();
    }

    private void UpdateCount(int count) =>
        CountProducts = $"Количество элементов на форме: {count}";

    private static decimal ParsePrice(string? price)
    {
        if (string.IsNullOrWhiteSpace(price)) return 0;
        var digits = new string(price.Where(c => char.IsDigit(c) || c == ',').ToArray());
        return decimal.TryParse(digits, out var result) ? result : 0;
    }
}
