using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;

public class ClientsViewModel : ViewModelBase
{
    private readonly List<ClientShow> _allClients = new();
    public ObservableCollection<ClientShow> Clients { get; } = new();

    private ObservableCollection<ClientShow> _currentClients = new();
    public ObservableCollection<ClientShow> CurrentClients
    {
        get => _currentClients;
        set => this.RaiseAndSetIfChanged(ref _currentClients, value);
    }

    private ClientShow? _selectedClient;
    public ClientShow? SelectedClient
    {
        get => _selectedClient;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedClient, value);

            if (value is null || value.IsRevealed) return;

            value.IsRevealed = true;

            Observable.Timer(TimeSpan.FromSeconds(15))
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Subscribe(_ => value.IsRevealed = false);
        }
    }

    private string _countClients = string.Empty;
    public string CountClients
    {
        get => _countClients;
        set => this.RaiseAndSetIfChanged(ref _countClients, value);
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            ApplySearch(value);
        }
    }

    public ReactiveCommand<ClientShow, Unit> EditCommand   { get; }
    public ReactiveCommand<ClientShow, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetSearchCommand  { get; }

    public ClientsViewModel(Action<ClientShow>? goToEdit = null)
    {
        EditCommand = ReactiveCommand.Create<ClientShow>(
            item => goToEdit?.Invoke(item));

        DeleteCommand = ReactiveCommand.Create<ClientShow>(item =>
        {
            _allClients.Remove(item);
            Clients.Remove(item);
            CurrentClients.Remove(item);
            UpdateCount(Clients.Count);
        });

        ResetSearchCommand = ReactiveCommand.Create(() => { SearchText = string.Empty; });

        var clientsDb = ClientsService.GetClients();
        foreach (var client in clientsDb)
        {
            var show = new ClientShow(
                client.Id,
                client.Name,
                client.PhoneNumber,
                client.TotalPurchases.ToString(),
                client.CountOrders);

            _allClients.Add(show);
            Clients.Add(show);
        }

        UpdateCount(Clients.Count);
    }

    private void ApplySearch(string query)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? _allClients
            : _allClients
                .Where(c => c.Name.ToLower().Contains(query.Trim().ToLower()))
                .ToList();

        Clients.Clear();
        foreach (var c in result) Clients.Add(c);

        UpdateCount(Clients.Count);
    }

    private void UpdateCount(int count) =>
        CountClients = $"Количество записей на форме: {count}";
}