using Microsoft.EntityFrameworkCore;
using TP6_WEBAPI.Models;

namespace TP6_WEBAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<Compte> Comptes => Set<Compte>();

    public DbSet<Operation> Operations => Set<Operation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Compte>()
            .HasIndex(c => c.NumeroCompte)
            .IsUnique();

        modelBuilder.Entity<Compte>()
            .HasOne(c => c.Client)
            .WithMany(c => c.Comptes)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Operation>()
            .HasOne(o => o.Compte)
            .WithMany(c => c.Operations)
            .HasForeignKey(o => o.CompteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
