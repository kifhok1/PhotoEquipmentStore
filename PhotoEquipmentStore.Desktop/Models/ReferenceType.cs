namespace PhotoEquipmentStore.Models;/// <summary>
/// Тип справочной записи в административном разделе.
/// </summary>


public enum ReferenceType
{
    /// <summary>Справочник ролей.</summary>
    Role,
    /// <summary>Справочник производителей.</summary>
    Manufacturers,
    /// <summary>Справочник категорий.</summary>
    Category,
    /// <summary>Справочник поставщиков.</summary>
    Suppliers,
    /// <summary>Справочник статусов заказа.</summary>
    OrderStatuses
}
