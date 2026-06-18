namespace PhotoEquipmentStore.Domain.Enums;

/// <summary>
/// Режим сортировки и выборки данных в отчёте по популярности товаров.
/// </summary>
public enum PopularityMode
{
    /// <summary>Все товары, отсортированные по убыванию продаж.</summary>
    AllDesc,
    /// <summary>Все товары, отсортированные по возрастанию продаж.</summary>
    AllAsc,
    /// <summary>Топ-30 самых продаваемых товаров.</summary>
    Top30,
    /// <summary>30 наименее продаваемых товаров.</summary>
    Bottom30
}
