using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IProductsService
{
    ProductResultDto GetProducts();
    ProductResultDto CreateProduct(Product product);
    ProductResultDto UpdateProduct(Product product);
    ProductResultDto DeleteProduct(int productId);
}