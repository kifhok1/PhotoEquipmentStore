using System.Collections.ObjectModel;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.DTO;

public class ClientResultDto
{
    public bool IsSuccess       { get; init; }
    public string? ErrorMessage { get; init; }
    public Client? Client       { get; init; }

    // Для GetClients — список
    public static ClientResultDto Success(ObservableCollection<Client> clients) =>
        new() { IsSuccess = true, Clients = clients.ToList() };

    // Для Create — возвращаем созданного клиента
    public static ClientResultDto Success(Client client) =>
        new() { IsSuccess = true, Client = client };

    // Для Update / Delete — просто успех без данных
    public static ClientResultDto Success() =>
        new() { IsSuccess = true };

    public static ClientResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };

    public List<Client> Clients { get; init; } = [];
}