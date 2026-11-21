using System.ComponentModel.DataAnnotations;

namespace ProductsAPI.Models;

public sealed class Product
{
    public int ProductId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int StockQte { get; set; }

    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }
}
