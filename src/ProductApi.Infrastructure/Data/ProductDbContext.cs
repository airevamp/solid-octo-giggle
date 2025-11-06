using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var product = modelBuilder.Entity<Product>();
        product.HasKey(p => p.Id);
        product.Property(p => p.Name).IsRequired().HasMaxLength(200);
        product.Property(p => p.Price).HasColumnType("decimal(18,2)");
        product.Property(p => p.CreatedAt).IsRequired();
        product.Property(p => p.UpdatedAt).IsRequired();
    }
}
