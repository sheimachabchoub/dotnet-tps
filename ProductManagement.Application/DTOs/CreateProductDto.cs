using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Application.DTOs;

public sealed class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
    public string Name { get; init; } = null!;

    [StringLength(500)]
    public string? Description { get; init; }

    [Range(0.01, 1_000_000, ErrorMessage = "Price must be positive and less than 1000000.")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
    public int Stock { get; init; }
}
