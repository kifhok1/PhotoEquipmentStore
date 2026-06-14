using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IClientsService
{
    ClientResultDto GetClients();
    ClientResultDto CreateClient(Client client);
    ClientResultDto UpdateClient(Client client);
    ClientResultDto DeleteClient(int clientId);
}
