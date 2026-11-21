using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto> AddProductAsync(CreateProductDto productDto, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(UpdateProductDto productDto, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);
}
