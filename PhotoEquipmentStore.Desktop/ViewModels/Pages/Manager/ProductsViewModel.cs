using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;

public class ProductsViewModel : ViewModelBase
{
    // Полный неизменяемый список — источник истины
    private readonly List<ProductsShow> _allProducts = new();

    // Пагинатор работает с этой коллекцией
    public ObservableCollection<ProductsShow> Products { get; } = new();

    private ObservableCollection<ProductsShow> _currentProducts = new();
    public ObservableCollection<ProductsShow> CurrentProducts
    {
        get => _currentProducts;
        set => this.RaiseAndSetIfChanged(ref _currentProducts, value);
    }

    private string _countProducts = string.Empty;
    public string CountProducts
    {
        get => _countProducts;
        set => this.RaiseAndSetIfChanged(ref _countProducts, value);
    }

    // ── Поиск ──────────────────────────────────────────────────────────
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { this.RaiseAndSetIfChanged(ref _searchText, value); ApplyFilters(); }
    }

    // ── Сортировка ─────────────────────────────────────────────────────
    public List<string> SortOptions { get; } = new()
    {
        "По возрастанию цены",
        "По убыванию цены"
    };

    private string? _selectedSortOption;
    public string? SelectedSortOption
    {
        get => _selectedSortOption;
        set { this.RaiseAndSetIfChanged(ref _selectedSortOption, value); ApplyFilters(); }
    }

    // ── Фильтр по категории ────────────────────────────────────────────
    public ObservableCollection<string> Categories { get; } = new();

    private string? _selectedCategory;
    public string? SelectedCategory
    {
        get => _selectedCategory;
        set { this.RaiseAndSetIfChanged(ref _selectedCategory, value); ApplyFilters(); }
    }

    // ── Команды ────────────────────────────────────────────────────────
    public ReactiveCommand<ProductsShow, Unit> EditCommand   { get; }
    public ReactiveCommand<ProductsShow, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit>         ResetCommand  { get; }

    public ProductsViewModel(Action<ProductsShow>? goToEdit = null)
    {
        EditCommand = ReactiveCommand.Create<ProductsShow>(
            item => goToEdit?.Invoke(item));

        DeleteCommand = ReactiveCommand.Create<ProductsShow>(item =>
        {
            _allProducts.Remove(item);
            Products.Remove(item);
            CurrentProducts.Remove(item);
            UpdateCount(Products.Count);
        });

        ResetCommand = ReactiveCommand.Create(Reset);

        var productsFromDb = ProductsService.GetProducts();
        foreach (var product in productsFromDb)
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

        // Уникальные категории для ComboBox
        foreach (var cat in _allProducts.Select(p => p.CategoryName).Distinct().Order())
            Categories.Add(cat);

        UpdateCount(Products.Count);
    }

    private void ApplyFilters()
    {
        var result = _allProducts.AsEnumerable();

        // 1. Поиск по названию
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var q = SearchText.Trim().ToLower();
            result = result.Where(p => p.Name.ToLower().Contains(q));
        }

        // 2. Фильтр по категории
        if (!string.IsNullOrWhiteSpace(SelectedCategory))
            result = result.Where(p => p.CategoryName == SelectedCategory);

        // 3. Сортировка по цене
        // Price — строка вида "15 000 ₽", поэтому парсим только цифры
        result = SelectedSortOption switch
        {
            "По возрастанию цены"  => result.OrderBy(p => ParsePrice(p.Price.ToString())),
            "По убыванию цены"     => result.OrderByDescending(p => ParsePrice(p.Price.ToString())),
            _                      => result
        };

        var list = result.ToList();

        Products.Clear();
        foreach (var p in list) Products.Add(p);

        UpdateCount(Products.Count);
    }

    private void Reset()
    {
        // Сначала глушим реакции, чтобы ApplyFilters не вызывался трижды
        _searchText = string.Empty;
        this.RaisePropertyChanged(nameof(SearchText));

        _selectedSortOption = null;
        this.RaisePropertyChanged(nameof(SelectedSortOption));

        _selectedCategory = null;
        this.RaisePropertyChanged(nameof(SelectedCategory));

        // Один вызов фильтрации после сброса всех полей
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