using System.Collections.ObjectModel;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;

namespace PhotoEquipmentStore.Application.Services;

public class ClientsService
{
    public static ObservableCollection<Client> GetClients()
    {
        return ClientCommands.GetClients();
    }
}