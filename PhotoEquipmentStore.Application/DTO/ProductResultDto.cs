using System.Collections.Generic;
using System.Linq;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.DTO;

public class ProductResultDto
{
    public bool IsSuccess        { get; init; }
    public string? ErrorMessage  { get; init; }
    public Product? Product      { get; init; }
    public List<Product> Products { get; init; } = [];

    public static ProductResultDto Success(IEnumerable<Product> products) =>
        new() { IsSuccess = true, Products = products.ToList() };

    public static ProductResultDto Success(Product product) =>
        new() { IsSuccess = true, Product = product };

    public static ProductResultDto Success() =>
        new() { IsSuccess = true };

    public static ProductResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
