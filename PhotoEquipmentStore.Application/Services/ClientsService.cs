using System.Collections.ObjectModel;
using System.Linq;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис управления клиентами магазина.
/// </summary>
public class ClientsService : IClientsService
{
    private readonly ClientCommands _clientCommands = new();
    private readonly Phone          _phoneCommands  = new();

    /// <summary>Возвращает список всех клиентов.</summary>
    public ClientResultDto GetClients()
    {
        try
        {
            var clients = ClientCommands.GetClients();
            return ClientResultDto.Success(clients);
        }
        catch (DatabaseException ex)
        {
            return ClientResultDto.Failure("Не удалось загрузить список клиентов.");
        }
    }

    /// <summary>Создаёт нового клиента с проверкой уникальности телефона.</summary>
    /// <param name="client">Данные клиента.</param>
    public ClientResultDto CreateClient(Client client)
    {
        try
        {
            var phoneCheck = EnsurePhoneIsUnique(client.Phone, excludeClientId: null);
            if (!phoneCheck.IsSuccess)
                return phoneCheck;

            _clientCommands.CreateClient(client);
            return ClientResultDto.Success();
        }
        catch (DatabaseException)
        {
            return ClientResultDto.Failure("Не удалось создать клиента.");
        }
    }

    /// <summary>Обновляет данные существующего клиента.</summary>
    /// <param name="client">Обновлённые данные клиента.</param>
    public ClientResultDto UpdateClient(Client client)
    {
        try
        {
            var phoneCheck = EnsurePhoneIsUnique(client.Phone, excludeClientId: client.Id);
            if (!phoneCheck.IsSuccess)
                return phoneCheck;

            _clientCommands.UpdateClient(client);
            return ClientResultDto.Success();
        }
        catch (DatabaseException)
        {
            return ClientResultDto.Failure("Не удалось обновить данные клиента.");
        }
    }

    /// <summary>Удаляет клиента по идентификатору.</summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    public ClientResultDto DeleteClient(int clientId)
    {
        try
        {
            _clientCommands.DeleteClient(clientId);
            return ClientResultDto.Success();
        }
        catch (DatabaseException)
        {
            return ClientResultDto.Failure("Не удалось удалить клиента.");
        }
    }

    private ClientResultDto EnsurePhoneIsUnique(string phone, int? excludeClientId)
    {
        try
        {
            var allPhones = _phoneCommands.GetAllPhoneNumbers();

            // При обновлении исключаем текущий номер клиента из проверки
            if (excludeClientId.HasValue)
            {
                var current = ClientCommands.GetClients()
                    .FirstOrDefault(c => c.Id == excludeClientId.Value);
                if (current is not null)
                    allPhones.Remove(current.Phone);
            }

            if (allPhones.Contains(phone))
                return ClientResultDto.Failure(
                    $"Номер {phone} уже используется другим пользователем или клиентом.");

            return ClientResultDto.Success();
        }
        catch (DatabaseException)
        {
            return ClientResultDto.Failure("Не удалось проверить уникальность номера телефона.");
        }
    }
}
