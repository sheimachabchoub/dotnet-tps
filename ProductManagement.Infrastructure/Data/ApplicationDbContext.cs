using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-end gaming laptop", Price = 1500m, Stock = 10 },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 800m, Stock = 25 },
                new Product { Id = 3, Name = "Wireless Headphones", Description = "Noise cancelling headphones", Price = 200m, Stock = 40 });
        });
    }
}
