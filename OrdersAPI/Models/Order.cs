using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.Models;

public sealed class Order
{
    public int OrdersId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    public int ProductId { get; set; }
}
