using System.Collections.ObjectModel;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class ClientsViewModel : ViewModelBase
{
    public ObservableCollection<ClientShow> Clients
    {
        get;
        private set;
    } = new();
    
    private string countClients;
    private ObservableCollection<ClientShow> currentClients = new();
    public ObservableCollection<ClientShow> CurrentClients
    {
        get => currentClients;
        set => this.RaiseAndSetIfChanged(ref currentClients, value);
    }

    public string CountClients
    {
        get => countClients;
        set => this.RaiseAndSetIfChanged(ref countClients, value);
    }

    public ClientsViewModel()
    {
        var clientsDb= ClientsService.GetClients();
        foreach (var client in clientsDb)
        {
            Clients.Add(new ClientShow(client.Id, client.Name, client.PhoneNumber, client.TotalPurchases.ToString(), client.CountOrders));
        }
        CountClients = $"Количесто записей на форме: {Clients.Count}";
    }
}