namespace PhotoEquipmentStore.Domain.Enums;

/// <summary>
/// Тип справочника, используемого в системе управления магазином.
/// </summary>
public enum ReferenceType
{
    /// <summary>Категория товаров.</summary>
    Category,
    /// <summary>Поставщик товаров.</summary>
    Supplier,
    /// <summary>Производитель товаров.</summary>
    Manufacturer,
    /// <summary>Роль пользователя.</summary>
    Role,
    /// <summary>Статус заказа.</summary>
    Status
}
