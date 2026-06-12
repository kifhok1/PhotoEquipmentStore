using System;
using System.Linq;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Domain.Enums;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class ReferenceService : IReferenceService
{
    private readonly ReferenceCommands _commands = new();

    // ── Маппинг ───────────────────────────────────────────────────────────────

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

    // ── Read ──────────────────────────────────────────────────────────────────

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

    // ── Create ────────────────────────────────────────────────────────────────

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

    // ── Update ────────────────────────────────────────────────────────────────

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

    // ── Delete ────────────────────────────────────────────────────────────────

    public ReferenceResultDto Delete(ReferenceType type, int id)
    {
        try
        {
            var items  = GetByType(type);
            var target = items.References.FirstOrDefault(r => r.Id == id);

            if (target is null)
                return ReferenceResultDto.Failure("Запись не найдена.");

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

    // ── Private ───────────────────────────────────────────────────────────────

    private ReferenceResultDto GetByType(ReferenceType type) => type switch
    {
        ReferenceType.Category     => GetCategories(),
        ReferenceType.Supplier     => GetSuppliers(),
        ReferenceType.Manufacturer => GetManufacturers(),
        _ => ReferenceResultDto.Failure("Неизвестный тип справочника.")
    };
}