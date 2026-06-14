using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.Controls;

/// <summary>
/// Компонент постраничной навигации по коллекции элементов.
/// </summary>


public partial class PaginationButtons : UserControl
{
    public static readonly StyledProperty<IList> ItemsProperty =
        AvaloniaProperty.Register<PaginationButtons, IList>(
            nameof(Items),
            defaultValue: new object[0]);

    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<PaginationButtons, int>(
            nameof(PageSize),
            defaultValue: 10,
            coerce: (_, v) => Math.Max(1, v));

    public static readonly StyledProperty<int> CurrentPageProperty =
        AvaloniaProperty.Register<PaginationButtons, int>(
            nameof(CurrentPage),
            defaultValue: 1);

    private IList _currentPageItems = new object[0];

    public static readonly DirectProperty<PaginationButtons, IList> CurrentPageItemsProperty =
        AvaloniaProperty.RegisterDirect<PaginationButtons, IList>(
            nameof(CurrentPageItems),
            o => o.CurrentPageItems,
            (o, v) => o.CurrentPageItems = v);

    private string _pageCountText = string.Empty;

    public static readonly DirectProperty<PaginationButtons, string> PageCountTextProperty =
        AvaloniaProperty.RegisterDirect<PaginationButtons, string>(
            nameof(PageCountText),
            o => o.PageCountText,
            (o, v) => o.PageCountText = v);

    /// <summary>

    /// Текстовая подпись о количестве записей на форме.

    /// </summary>

    public string PageCountText
    {
        get => _pageCountText;
        set => SetAndRaise(PageCountTextProperty, ref _pageCountText, value);
    }

    /// <summary>

    /// Полная коллекция элементов для постраничного отображения.

    /// </summary>

    public IList Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>

    /// Количество элементов на одной странице.

    /// </summary>

    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }

    /// <summary>

    /// Номер текущей страницы.

    /// </summary>

    public int CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, Math.Clamp(value, 1, Math.Max(1, TotalPages)));
    }

    /// <summary>

    /// Элементы, отображаемые на текущей странице.

    /// </summary>

    public IList CurrentPageItems
    {
        get => _currentPageItems;
        set => SetAndRaise(CurrentPageItemsProperty, ref _currentPageItems, value);
    }

    private int TotalPages => ComputeTotalPages();

    /// <summary>

    /// Кнопки номеров страниц для навигации.

    /// </summary>

    public ObservableCollection<PageButton> PageButtons { get; } = [];

    /// <summary>

    /// Команда перехода на предыдущую страницу.

    /// </summary>

    public ReactiveCommand<Unit, Unit> PreviousCommand { get; }
    /// <summary>
    /// Команда перехода на следующую страницу.
    /// </summary>
    public ReactiveCommand<Unit, Unit> NextCommand     { get; }
    /// <summary>
    /// Команда перехода на указанную страницу.
    /// </summary>
    public ReactiveCommand<int,  Unit> GoToPageCommand { get; }

    private IDisposable? _collectionChangedSubscription;
    private readonly Subject<Unit> _collectionContentChanged = new();

    public PaginationButtons()
    {
        var anyChange = Observable.Merge(
            this.GetObservable(ItemsProperty).Select(_ => Unit.Default),
            this.GetObservable(PageSizeProperty).Select(_ => Unit.Default),
            this.GetObservable(CurrentPageProperty).Select(_ => Unit.Default),
            _collectionContentChanged
        );

        var canPrev = anyChange
            .Select(_ => CurrentPage > 1)
            .StartWith(false)
            .DistinctUntilChanged();

        var canNext = anyChange
            .Select(_ => CurrentPage < ComputeTotalPages())
            .StartWith(false)
            .DistinctUntilChanged();

        PreviousCommand = ReactiveCommand.Create(() => { CurrentPage--; }, canPrev);
        NextCommand     = ReactiveCommand.Create(() => { CurrentPage++; }, canNext);
        GoToPageCommand = ReactiveCommand.Create<int>(page => { CurrentPage = page; });

        InitializeComponent();

        Observable.Merge(
                this.GetObservable(PageSizeProperty).Select(_ => Unit.Default),
                this.GetObservable(CurrentPageProperty).Select(_ => Unit.Default),
                _collectionContentChanged)
            .Subscribe(_ => Refresh());

        this.GetObservable(ItemsProperty).Subscribe(OnItemsChanged);
    }

    private void OnItemsChanged(IList? newItems)
    {
        _collectionChangedSubscription?.Dispose();

        if (newItems is INotifyCollectionChanged observable)
        {
            _collectionChangedSubscription =
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => observable.CollectionChanged += h,
                        h => observable.CollectionChanged -= h)
                    .Select(_ => Unit.Default)
                    .Subscribe(u => _collectionContentChanged.OnNext(u));
        }

        Refresh();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsProperty || change.Property == PageSizeProperty)
        {
            var clamped = Math.Clamp(CurrentPage, 1, Math.Max(1, ComputeTotalPages()));
            if (clamped != CurrentPage)
                CurrentPage = clamped;
        }
    }

    private void Refresh()
    {
        var totalPages = ComputeTotalPages();
        var clampedPage = Math.Clamp(CurrentPage, 1, Math.Max(1, totalPages));
        if (clampedPage != CurrentPage)
            CurrentPage = clampedPage;

        CurrentPageItems = Items
            .Cast<object>()
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        var total = Items?.Count ?? 0;
        var offset = (CurrentPage - 1) * PageSize;
        var to = offset + CurrentPageItems.Count;
        PageCountText = $"Количество записей на форме {to} из {total}";

        var buttons = GenerateButtons(ComputeTotalPages(), CurrentPage);
        PageButtons.Clear();
        foreach (var btn in buttons)
            PageButtons.Add(btn);
    }

    private int ComputeTotalPages() =>
        Items is null || Items.Count == 0
            ? 1
            : (int)Math.Ceiling((double)Items.Count / PageSize);

    private static IReadOnlyList<PageButton> GenerateButtons(int total, int current)
    {
        if (total <= 1) return [];

        var result = new List<PageButton>();

        if (total <= 7)
        {
            for (var i = 1; i <= total; i++)
                result.Add(PageButton.Of(i, i == current));
            return result;
        }

        result.Add(PageButton.Of(1, current == 1));

        var winStart = Math.Max(2, current - 2);
        var winEnd   = Math.Min(total - 1, current + 2);

        if (winStart > 2) result.Add(PageButton.Dots());
        for (var i = winStart; i <= winEnd; i++)
            result.Add(PageButton.Of(i, i == current));
        if (winEnd < total - 1) result.Add(PageButton.Dots());

        result.Add(PageButton.Of(total, current == total));

        return result;
    }
}
