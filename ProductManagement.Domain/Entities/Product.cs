using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public void ValidateBusinessRules()
    {
        if (Price <= 0)
        {
            throw new InvalidOperationException("Product price must be positive.");
        }

        if (Stock < 0)
        {
            throw new InvalidOperationException("Product stock cannot be negative.");
        }
    }
}
