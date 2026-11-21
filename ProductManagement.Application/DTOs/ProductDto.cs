namespace ProductManagement.Application.DTOs;

public sealed class ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
