using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса управления товарами каталога.
/// </summary>
public interface IProductsService
{
    /// <summary>Возвращает список всех товаров.</summary>
    ProductResultDto GetProducts();

    /// <summary>Создаёт новый товар.</summary>
    /// <param name="product">Данные товара.</param>
    ProductResultDto CreateProduct(Product product);

    /// <summary>Обновляет данные существующего товара.</summary>
    /// <param name="product">Обновлённые данные товара.</param>
    ProductResultDto UpdateProduct(Product product);

    /// <summary>Удаляет товар по идентификатору.</summary>
    /// <param name="productId">Идентификатор товара.</param>
    ProductResultDto DeleteProduct(int productId);
}
