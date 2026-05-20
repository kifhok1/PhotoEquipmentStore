using System.Collections.ObjectModel;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;

namespace PhotoEquipmentStore.Application.Services;

public class ProductsService
{
    public static ObservableCollection<Product> GetProducts()
    {
        return ProductCommands.GetProducts();
    }
}