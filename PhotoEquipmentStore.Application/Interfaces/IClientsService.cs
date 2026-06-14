using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса управления клиентами магазина.
/// </summary>
public interface IClientsService
{
    /// <summary>Возвращает список всех клиентов.</summary>
    ClientResultDto GetClients();

    /// <summary>Создаёт нового клиента.</summary>
    /// <param name="client">Данные клиента.</param>
    ClientResultDto CreateClient(Client client);

    /// <summary>Обновляет данные существующего клиента.</summary>
    /// <param name="client">Обновлённые данные клиента.</param>
    ClientResultDto UpdateClient(Client client);

    /// <summary>Удаляет клиента по идентификатору.</summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    ClientResultDto DeleteClient(int clientId);
}
