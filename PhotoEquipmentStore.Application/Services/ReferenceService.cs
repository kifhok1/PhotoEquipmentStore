using System;
using System.Linq;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Domain.Enums;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис управления справочными данными (роли, категории, поставщики и т.д.).
/// </summary>
public class ReferenceService : IReferenceService
{
    private readonly ReferenceCommands _commands = new();

    private static string GetTable(ReferenceType type) => type switch
    {
        ReferenceType.Category     => "categories",
        ReferenceType.Supplier     => "suppliers",
        ReferenceType.Manufacturer => "manufacturers",
        ReferenceType.Status => "order_statuses",
        ReferenceType.Role => "roles",
        _ => throw new ArgumentOutOfRangeException(nameof(type), "Неизвестный тип справочника.")
    };

    private static string GetTypeName(ReferenceType type) => type switch
    {
        ReferenceType.Category     => "категория",
        ReferenceType.Supplier     => "поставщик",
        ReferenceType.Manufacturer => "производитель",
        ReferenceType.Status => "статус",
        ReferenceType.Role => "роль",
        _ => "справочник"
    };

    /// <summary>Возвращает список ролей пользователей.</summary>
    public ReferenceResultDto GetRoles()
    {
        try
        {
            return ReferenceResultDto.Success(ReferenceCommands.GetRoles());
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось загрузить список ролей.");
        }
    }

    /// <summary>Возвращает список статусов заказов.</summary>
    public ReferenceResultDto GetOrderStatuses()
    {
        try
        {
            return ReferenceResultDto.Success(ReferenceCommands.GetOrderStatuses());
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось загрузить список статусов заказов.");
        }
    }

    /// <summary>Возвращает список категорий товаров.</summary>
    public ReferenceResultDto GetCategories()
    {
        try
        {
            return ReferenceResultDto.Success(ReferenceCommands.GetCategories());
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось загрузить список категорий.");
        }
    }

    /// <summary>Возвращает список поставщиков.</summary>
    public ReferenceResultDto GetSuppliers()
    {
        try
        {
            return ReferenceResultDto.Success(ReferenceCommands.GetSuppliers());
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось загрузить список поставщиков.");
        }
    }

    /// <summary>Возвращает список производителей.</summary>
    public ReferenceResultDto GetManufacturers()
    {
        try
        {
            return ReferenceResultDto.Success(ReferenceCommands.GetManufacturers());
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось загрузить список производителей.");
        }
    }

    /// <summary>Создаёт новую запись в указанном справочнике.</summary>
    /// <param name="type">Тип справочника.</param>
    /// <param name="name">Наименование записи.</param>
    public ReferenceResultDto Create(ReferenceType type, string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return ReferenceResultDto.Failure("Наименование не может быть пустым.");

            string table    = GetTable(type);
            string typeName = GetTypeName(type);

            if (_commands.NameExists(table, name))
                return ReferenceResultDto.Failure($"«{name}» уже существует в списке: {typeName}.");

            _commands.CreateReference(table, name);

            var created = new Reference(0, name, 0, false);
            return ReferenceResultDto.Success(created);
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось создать запись.");
        }
    }

    /// <summary>Обновляет существующую запись справочника.</summary>
    /// <param name="type">Тип справочника.</param>
    /// <param name="reference">Данные записи для обновления.</param>
    public ReferenceResultDto Update(ReferenceType type, Reference reference)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(reference.Name))
                return ReferenceResultDto.Failure("Наименование не может быть пустым.");

            string table    = GetTable(type);
            string typeName = GetTypeName(type);

            if (_commands.NameExists(table, reference.Name, excludeId: reference.Id))
                return ReferenceResultDto.Failure($"«{reference.Name}» уже существует в списке: {typeName}.");

            _commands.UpdateReference(table, reference);
            return ReferenceResultDto.Success(reference);
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось обновить запись.");
        }
    }

    /// <summary>Удаляет запись из справочника, если нет связанных товаров.</summary>
    /// <param name="type">Тип справочника.</param>
    /// <param name="id">Идентификатор записи.</param>
    public ReferenceResultDto Delete(ReferenceType type, int id)
    {
        try
        {
            var items  = GetByType(type);
            var target = items.References.FirstOrDefault(r => r.Id == id);

            if (target is null)
                return ReferenceResultDto.Failure("Запись не найдена.");

            // Удаление запрещено, если запись используется в связанных товарах
            if (target.Count > 0)
                return ReferenceResultDto.Failure(
                    $"Нельзя удалить «{target.Name}»: есть {target.Count} связанных товаров.");

            _commands.DeleteReference(GetTable(type), id);
            return ReferenceResultDto.Success();
        }
        catch (DatabaseException)
        {
            return ReferenceResultDto.Failure("Не удалось удалить запись.");
        }
    }

    private ReferenceResultDto GetByType(ReferenceType type) => type switch
    {
        ReferenceType.Category     => GetCategories(),
        ReferenceType.Supplier     => GetSuppliers(),
        ReferenceType.Manufacturer => GetManufacturers(),
        _ => ReferenceResultDto.Failure("Неизвестный тип справочника.")
    };
}
