using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ReactiveUI;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Behaviors;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Domain.Enums;

namespace PhotoEquipmentStore.ViewModels.Pages.Manager;/// <summary>
/// ViewModel формирования отчётов и сводной статистики.
/// </summary>


public class ReportsViewModel : ViewModelBase, IStorageProviderReceiver
{
    private readonly ReportService    _reportService    = new();
    private readonly ReferenceService _referenceService = new();

    /// <summary>

    /// Провайдер диалогов выбора файлов и папок.

    /// </summary>

    public IStorageProvider? StorageProvider { get; set; }

    private string _ordersToday  = "—";
    private string _itemsToday   = "—";
    private string _revenueToday = "—";
    private string _ordersMonth  = "—";
    private string _itemsMonth   = "—";
    private string _revenueMonth = "—";

    /// <summary>

    /// Количество заказов за сегодня.

    /// </summary>

    public string OrdersToday  { get => _ordersToday;  private set => this.RaiseAndSetIfChanged(ref _ordersToday,  value); }
    /// <summary>
    /// Количество позиций за сегодня.
    /// </summary>
    public string ItemsToday   { get => _itemsToday;   private set => this.RaiseAndSetIfChanged(ref _itemsToday,   value); }
    /// <summary>
    /// Выручка за сегодня.
    /// </summary>
    public string RevenueToday { get => _revenueToday; private set => this.RaiseAndSetIfChanged(ref _revenueToday, value); }
    /// <summary>
    /// Количество заказов за месяц.
    /// </summary>
    public string OrdersMonth  { get => _ordersMonth;  private set => this.RaiseAndSetIfChanged(ref _ordersMonth,  value); }
    /// <summary>
    /// Количество позиций за месяц.
    /// </summary>
    public string ItemsMonth   { get => _itemsMonth;   private set => this.RaiseAndSetIfChanged(ref _itemsMonth,   value); }
    /// <summary>
    /// Выручка за месяц.
    /// </summary>
    public string RevenueMonth { get => _revenueMonth; private set => this.RaiseAndSetIfChanged(ref _revenueMonth, value); }

    /// <summary>

    /// Минимально допустимая дата отчёта по продажам.

    /// </summary>

    public DateTimeOffset? SalesMinDate { get; private set; }
    /// <summary>
    /// Максимально допустимая дата отчёта по продажам.
    /// </summary>
    public DateTimeOffset? SalesMaxDate { get; private set; }

    /// <summary>

    /// Верхняя граница для поля «дата с».

    /// </summary>

    public DateTimeOffset? SalesDateFromMax => _salesDateTo   ?? SalesMaxDate;
    /// <summary>
    /// Нижняя граница для поля «дата по».
    /// </summary>
    public DateTimeOffset? SalesDateToMin   => _salesDateFrom ?? SalesMinDate;

    private DateTimeOffset? _salesDateFrom;
    private DateTimeOffset? _salesDateTo;

    /// <summary>

    /// Начало периода отчёта по продажам.

    /// </summary>

    public DateTimeOffset? SalesDateFrom
    {
        get => _salesDateFrom;
        set
        {
            this.RaiseAndSetIfChanged(ref _salesDateFrom, value);
            if (value.HasValue && _salesDateTo.HasValue && value.Value > _salesDateTo.Value)
                SalesDateTo = null;
            this.RaisePropertyChanged(nameof(SalesDateToMin));
        }
    }

    /// <summary>

    /// Конец периода отчёта по продажам.

    /// </summary>

    public DateTimeOffset? SalesDateTo
    {
        get => _salesDateTo;
        set
        {
            this.RaiseAndSetIfChanged(ref _salesDateTo, value);
            if (value.HasValue && _salesDateFrom.HasValue && value.Value < _salesDateFrom.Value)
                SalesDateFrom = null;
            this.RaisePropertyChanged(nameof(SalesDateFromMax));
        }
    }

    /// <summary>

    /// Категории для отчёта по остаткам.

    /// </summary>

    public ObservableCollection<Reference> StockCategories { get; } = new();

    private Reference? _selectedStockCategory;
    /// <summary>
    /// Выбранная категория отчёта по остаткам.
    /// </summary>
    public Reference? SelectedStockCategory
    {
        get => _selectedStockCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedStockCategory, value);
    }

    /// <summary>

    /// Категории для отчёта по популярности.

    /// </summary>

    public ObservableCollection<Reference> PopularityCategories { get; } = new();

    private Reference? _selectedPopularityCategory;
    /// <summary>
    /// Выбранная категория отчёта по популярности.
    /// </summary>
    public Reference? SelectedPopularityCategory
    {
        get => _selectedPopularityCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedPopularityCategory, value);
    }

    /// <summary>

    /// Варианты сортировки отчёта по популярности.

    /// </summary>

    public ObservableCollection<string> PopularityTypes { get; } = new()
    {
        "Все по убыванию популярности",
        "Все по возрастанию популярности",
        "30 самых популярных",
        "30 самых непопулярных"
    };

    private string _selectedPopularityType = "Все по убыванию популярности";
    /// <summary>
    /// Выбранный тип отчёта по популярности.
    /// </summary>
    public string SelectedPopularityType
    {
        get => _selectedPopularityType;
        set => this.RaiseAndSetIfChanged(ref _selectedPopularityType, value);
    }

    /// <summary>

    /// Команда формирования отчёта по продажам.

    /// </summary>

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> BuildSalesReportCommand      { get; }
    /// <summary>
    /// Команда формирования отчёта по остаткам.
    /// </summary>
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> BuildStockReportCommand      { get; }
    /// <summary>
    /// Команда формирования отчёта по популярности.
    /// </summary>
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> BuildPopularityReportCommand { get; }

    private string? _errorMessage;
    /// <summary>
    /// Текст ошибки CAPTCHA.
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ReportsViewModel()
    {
        BuildSalesReportCommand      = ReactiveCommand.CreateFromTask(ExecuteBuildSalesReportAsync);
        BuildStockReportCommand      = ReactiveCommand.CreateFromTask(ExecuteBuildStockReportAsync);
        BuildPopularityReportCommand = ReactiveCommand.CreateFromTask(ExecuteBuildPopularityReportAsync);

        LoadStats();
        LoadCategories();

    }

    private void LoadStats()
    {
        var stats = _reportService.GetDashboardStats();
        OrdersToday  = $"{stats.OrdersToday} за этот день";
        ItemsToday   = $"{stats.ItemsToday} за этот день";
        RevenueToday = $"{stats.RevenueToday:N0}₽ сегодня";
        OrdersMonth  = $"{stats.OrdersMonth} за этот месяц";
        ItemsMonth   = $"{stats.ItemsMonth} за этот месяц";
        RevenueMonth = $"{stats.RevenueMonth:N0}₽ за месяц";
    }

    /// <summary>

    /// Инициализирует диапазон дат отчёта по продажам из БД.

    /// </summary>

    public void InitializeDates()
    {
        try
        {
            var (min, max) = _reportService.GetOrderDateRange();

            var minOff = new DateTimeOffset(DateTime.SpecifyKind(min, DateTimeKind.Local));
            var maxOff = new DateTimeOffset(DateTime.SpecifyKind(max, DateTimeKind.Local));

            SalesMinDate   = minOff;
            SalesMaxDate   = maxOff;
            _salesDateFrom = minOff;
            _salesDateTo   = maxOff;

            this.RaisePropertyChanged(nameof(SalesMinDate));
            this.RaisePropertyChanged(nameof(SalesMaxDate));
            this.RaisePropertyChanged(nameof(SalesDateFrom));
            this.RaisePropertyChanged(nameof(SalesDateTo));
            this.RaisePropertyChanged(nameof(SalesDateFromMax));
            this.RaisePropertyChanged(nameof(SalesDateToMin));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[InitializeDates] EXCEPTION: {ex}");
        }
    }

    private void LoadCategories()
    {
        var result = _referenceService.GetCategories();
        if (!result.IsSuccess) return;

        var all = new Reference(0, "Все категории", 0, false);

        StockCategories.Clear();
        StockCategories.Add(all);
        foreach (var c in result.References) StockCategories.Add(c);
        SelectedStockCategory = all;

        PopularityCategories.Clear();
        PopularityCategories.Add(all);
        foreach (var c in result.References) PopularityCategories.Add(c);
        SelectedPopularityCategory = all;
    }

    private async Task<string?> PickSavePathAsync(string defaultPath)
    {
        if (StorageProvider is null) return null;

        string  suggestedName = System.IO.Path.GetFileName(defaultPath);
        string? startDir      = System.IO.Path.GetDirectoryName(defaultPath);

        var options = new FilePickerSaveOptions
        {
            Title             = "Сохранить отчёт",
            SuggestedFileName = suggestedName,
            DefaultExtension  = "xlsx",
            FileTypeChoices   = new[]
            {
                new FilePickerFileType("Excel файл") { Patterns = new[] { "*.xlsx" } }
            },
            SuggestedStartLocation = startDir is not null
                ? await StorageProvider.TryGetFolderFromPathAsync(startDir)
                : null
        };

        var file = await StorageProvider.SaveFilePickerAsync(options);
        return file?.TryGetLocalPath();
    }

    private async Task ExecuteBuildSalesReportAsync()
    {
        ErrorMessage = null;

        if (SalesDateFrom is null || SalesDateTo is null)
        {
            ErrorMessage = "Укажите временной промежуток для отчёта.";
            return;
        }

        var result = await _reportService.BuildSalesReportAsync(
            SalesDateFrom.Value.DateTime,
            SalesDateTo.Value.DateTime,
            PickSavePathAsync);

        HandleResult(result);
    }

    private async Task ExecuteBuildStockReportAsync()
    {
        ErrorMessage = null;

        int?   categoryId   = SelectedStockCategory?.Id == 0 ? null : SelectedStockCategory?.Id;
        string categoryName = SelectedStockCategory?.Name ?? "Все категории";

        var result = await _reportService.BuildStockReportAsync(
            categoryId, categoryName, PickSavePathAsync);

        HandleResult(result);
    }

    private async Task ExecuteBuildPopularityReportAsync()
    {
        ErrorMessage = null;

        int?   categoryId   = SelectedPopularityCategory?.Id == 0 ? null : SelectedPopularityCategory?.Id;
        string categoryName = SelectedPopularityCategory?.Name ?? "Все категории";

        var mode = SelectedPopularityType switch
        {
            "Все по убыванию популярности"    => PopularityMode.AllDesc,
            "Все по возрастанию популярности" => PopularityMode.AllAsc,
            "30 самых популярных"             => PopularityMode.Top30,
            "30 самых непопулярных"           => PopularityMode.Bottom30,
            _                                 => PopularityMode.AllDesc
        };

        var result = await _reportService.BuildPopularityReportAsync(
            categoryId, categoryName, mode, PickSavePathAsync);

        HandleResult(result);
    }

    private void HandleResult(ReportResultDto result)
    {
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage;
            return;
        }

        Process.Start(new ProcessStartInfo(result.FilePath!)
        {
            UseShellExecute = true
        });
    }
}
