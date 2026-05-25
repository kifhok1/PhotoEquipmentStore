using System;
using System.Collections.ObjectModel;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class ClientsViewModel : ViewModelBase
{
    public ObservableCollection<ClientShow> Clients { get; } = new();

    private ObservableCollection<ClientShow> _currentClients = new();
    public ObservableCollection<ClientShow> CurrentClients
    {
        get => _currentClients;
        set => this.RaiseAndSetIfChanged(ref _currentClients, value);
    }

    private string _countClients = string.Empty;
    public string CountClients
    {
        get => _countClients;
        set => this.RaiseAndSetIfChanged(ref _countClients, value);
    }

    public ReactiveCommand<ClientShow, Unit> EditCommand   { get; }
    public ReactiveCommand<ClientShow, Unit> DeleteCommand { get; }

    public ClientsViewModel(Action<ClientShow>? goToEdit = null)
    {
        EditCommand = ReactiveCommand.Create<ClientShow>(
            item => goToEdit?.Invoke(item));

        DeleteCommand = ReactiveCommand.Create<ClientShow>(item =>
        {
            // TODO: вызов сервиса удаления
            Clients.Remove(item);
            CurrentClients.Remove(item);
            CountClients = $"Количество записей на форме: {Clients.Count}";
        });

        var clientsDb = ClientsService.GetClients();
        foreach (var client in clientsDb)
            Clients.Add(new ClientShow(client.Id, client.Name,
                client.PhoneNumber, client.TotalPurchases.ToString(), client.CountOrders));

        CountClients = $"Количество записей на форме: {Clients.Count}";
    }
}