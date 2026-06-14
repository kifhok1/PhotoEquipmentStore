using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Domain.Enums;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса управления справочными данными.
/// </summary>
public interface IReferenceService
{
    /// <summary>Возвращает список ролей пользователей.</summary>
    ReferenceResultDto GetRoles();

    /// <summary>Возвращает список статусов заказов.</summary>
    ReferenceResultDto GetOrderStatuses();

    /// <summary>Возвращает список категорий товаров.</summary>
    ReferenceResultDto GetCategories();

    /// <summary>Возвращает список поставщиков.</summary>
    ReferenceResultDto GetSuppliers();

    /// <summary>Возвращает список производителей.</summary>
    ReferenceResultDto GetManufacturers();

    /// <summary>Создаёт новую запись в указанном справочнике.</summary>
    /// <param name="type">Тип справочника.</param>
    /// <param name="name">Наименование записи.</param>
    ReferenceResultDto Create(ReferenceType type, string name);

    /// <summary>Обновляет существующую запись справочника.</summary>
    /// <param name="type">Тип справочника.</param>
    /// <param name="reference">Данные записи для обновления.</param>
    ReferenceResultDto Update(ReferenceType type, Reference reference);

    /// <summary>Удаляет запись из справочника по идентификатору.</summary>
    /// <param name="type">Тип справочника.</param>
    /// <param name="id">Идентификатор записи.</param>
    ReferenceResultDto Delete(ReferenceType type, int id);

}
