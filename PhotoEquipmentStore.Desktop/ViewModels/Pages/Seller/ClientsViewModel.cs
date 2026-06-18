using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Seller;/// <summary>
/// ViewModel списка клиентов с поиском и маскированием данных.
/// </summary>


public class ClientsViewModel : ViewModelBase
{
    private readonly ClientsService _clientsService = new ClientsService();
    private readonly List<ClientShow> _allClients = new();
    /// <summary>
    /// Список клиентов.
    /// </summary>
    public ObservableCollection<ClientShow> Clients { get; } = new();

    private ObservableCollection<ClientShow> _currentClients = new();
    public ObservableCollection<ClientShow> CurrentClients
    {
        get => _currentClients;
        set => this.RaiseAndSetIfChanged(ref _currentClients, value);
    }

    private ClientShow? _selectedClient;
    /// <summary>
    /// Выбранный клиент заказа.
    /// </summary>
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
    /// <summary>
    /// Подпись с количеством клиентов.
    /// </summary>
    public string CountClients
    {
        get => _countClients;
        set => this.RaiseAndSetIfChanged(ref _countClients, value);
    }

    private string _searchText = string.Empty;
    /// <summary>
    /// Строка поиска по названию товара.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            ApplySearch(value);
        }
    }

    /// <summary>

    /// Команда перехода к редактированию записи.

    /// </summary>

    public ReactiveCommand<ClientShow, Unit> EditCommand   { get; }
    /// <summary>
    /// Команда удаления записи.
    /// </summary>
    public ReactiveCommand<ClientShow, Unit> DeleteCommand { get; }
    /// <summary>
    /// Команда сброса строки поиска.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ResetSearchCommand  { get; }

    public ClientsViewModel(Action<ClientShow>? goToEdit = null)
    {
        EditCommand = ReactiveCommand.Create<ClientShow>(
            item =>
            {
                goToEdit?.Invoke(item);
                LoadClients();
            });

        DeleteCommand = ReactiveCommand.CreateFromTask<ClientShow>(async item =>
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Удалить запись?",
                $"Вы уверены, что хотите удалить клиента - {item.Name}? Это действие нельзя будет отменить.");

            if (confirmed)
            {
                var clientsDb = _clientsService.DeleteClient(item.Id);
                if (clientsDb.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync("Успешно", $"Клиент - {item.Name} удалён.");
                    LoadClients();
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось удалить клиента - {item.Name}.");
                }
            }
        });

        ResetSearchCommand = ReactiveCommand.Create(() => { SearchText = string.Empty; });

        LoadClients();
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

    private async void LoadClients()
    {
        _allClients.Clear();
        Clients.Clear();

        var clientsDb = _clientsService.GetClients();
        if (clientsDb.IsSuccess)
        {
            foreach (var client in clientsDb.Clients)
            {
                var show = new ClientShow(
                    client.Id,
                    client.FullName,
                    client.Phone,
                    client.TotalPurchases.ToString(),
                    client.CountOrders);

                _allClients.Add(show);
                Clients.Add(show);
            }
        }
        else
        {
            var show = new ClientShow(
                0,
                "Ошибка загрузки данных",
                "N/A",
                0.ToString(),
                0);

            _allClients.Add(show);
            Clients.Add(show);

            await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось загрузить список клиентов.");
        }

        UpdateCount(Clients.Count);
    }

    private void UpdateCount(int count) =>
        CountClients = $"Количество записей на форме: {count}";
}
