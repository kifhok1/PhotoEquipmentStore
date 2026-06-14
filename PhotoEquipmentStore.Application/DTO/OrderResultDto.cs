using System.Collections.ObjectModel;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.DTO;

public class OrderResultDto
{
    public bool    IsSuccess       { get; init; }
    public string? ErrorMessage    { get; init; }

    public List<Order>     Orders     { get; init; } = [];
    public List<OrderItem> OrderItems { get; init; } = [];

    public static OrderResultDto Success(ObservableCollection<Order> orders) =>
        new() { IsSuccess = true, Orders = orders.ToList() };

    public static OrderResultDto Success(ObservableCollection<OrderItem> items) =>
        new() { IsSuccess = true, OrderItems = items.ToList() };

    public static OrderResultDto Success() =>
        new() { IsSuccess = true };

    public static OrderResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
