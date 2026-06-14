using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис управления товарами каталога.
/// </summary>
public class ProductsService : IProductsService
{
    private readonly ProductCommands _commands = new();

    /// <summary>Возвращает список всех товаров.</summary>
    public ProductResultDto GetProducts()
    {
        try
        {
            var products = ProductCommands.GetProducts();
            return ProductResultDto.Success(products);
        }
        catch (DatabaseException)
        {
            return ProductResultDto.Failure("Не удалось загрузить список товаров.");
        }
    }

    /// <summary>Создаёт новый товар с предварительным сжатием изображения.</summary>
    /// <param name="product">Данные товара.</param>
    public ProductResultDto CreateProduct(Product product)
    {
        try
        {
            product.Image = ImageCompressor.CompressIfNeeded(product.Image);
            _commands.CreateProduct(product);
            return ProductResultDto.Success(product);
        }
        catch (DatabaseException)
        {
            return ProductResultDto.Failure("Не удалось создать товар.");
        }
    }

    /// <summary>Обновляет данные существующего товара.</summary>
    /// <param name="product">Обновлённые данные товара.</param>
    public ProductResultDto UpdateProduct(Product product)
    {
        try
        {
            product.Image = ImageCompressor.CompressIfNeeded(product.Image);
            _commands.UpdateProduct(product);
            return ProductResultDto.Success(product);
        }
        catch (DatabaseException)
        {
            return ProductResultDto.Failure("Не удалось обновить товар.");
        }
    }

    /// <summary>Удаляет товар по идентификатору.</summary>
    /// <param name="productId">Идентификатор товара.</param>
    public ProductResultDto DeleteProduct(int productId)
    {
        try
        {
            _commands.DeleteProduct(productId);
            return ProductResultDto.Success();
        }
        catch (DatabaseException)
        {
            return ProductResultDto.Failure("Не удалось удалить товар.");
        }
    }
}
