using Microsoft.EntityFrameworkCore;
using OrdersAPI.Models;

namespace OrdersAPI.Data;

public sealed class OrdersApiContext(DbContextOptions<OrdersApiContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasKey(order => order.OrdersId);
        modelBuilder.Entity<Product>().Property(product => product.Id).ValueGeneratedNever();
    }
}
