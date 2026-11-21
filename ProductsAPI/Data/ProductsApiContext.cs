using Microsoft.EntityFrameworkCore;
using ProductsAPI.Models;

namespace ProductsAPI.Data;

public sealed class ProductsApiContext(DbContextOptions<ProductsApiContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(product => product.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "prod1", StockQte = 50, Price = 1000m },
            new Product { ProductId = 2, Name = "prod2", StockQte = 40, Price = 1520m });
    }
}
