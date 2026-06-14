using System.Collections.ObjectModel;
using System.Linq;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class ClientsService : IClientsService
{
    private readonly ClientCommands _clientCommands = new();
    private readonly Phone          _phoneCommands  = new();

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
