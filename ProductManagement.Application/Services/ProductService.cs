using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces.Repositories;
using ProductManagement.Application.Interfaces.Services;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> AddProductAsync(CreateProductDto productDto, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Stock = productDto.Stock
        };
        product.ValidateBusinessRules();
        var createdProduct = await repository.AddAsync(product, cancellationToken);
        return MapToDto(createdProduct);
    }

    public async Task UpdateProductAsync(UpdateProductDto productDto, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(productDto.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");
        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.Price = productDto.Price;
        product.Stock = productDto.Stock;
        product.ValidateBusinessRules();
        await repository.UpdateAsync(product, cancellationToken);
    }

    public Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        => repository.DeleteAsync(id, cancellationToken);

    private static ProductDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock
    };
}
