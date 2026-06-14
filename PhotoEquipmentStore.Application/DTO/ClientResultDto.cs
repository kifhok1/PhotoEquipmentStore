using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.DTO;

public class ClientResultDto
{
    public bool IsSuccess       { get; init; }
    public string? ErrorMessage { get; init; }
    public Client? Client       { get; init; }

    public static ClientResultDto Success(ObservableCollection<Client> clients) =>
        new() { IsSuccess = true, Clients = clients.ToList() };

    public static ClientResultDto Success(Client client) =>
        new() { IsSuccess = true, Client = client };

    public static ClientResultDto Success() =>
        new() { IsSuccess = true };

    public static ClientResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };

    public List<Client> Clients { get; init; } = [];
}
