using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class ProductsService : IProductsService
{
    private readonly ProductCommands _commands = new();

    // ── Read ──────────────────────────────────────────────────────────────────

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

    // ── Create ────────────────────────────────────────────────────────────────

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

    // ── Update ────────────────────────────────────────────────────────────────

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

    // ── Delete ────────────────────────────────────────────────────────────────

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