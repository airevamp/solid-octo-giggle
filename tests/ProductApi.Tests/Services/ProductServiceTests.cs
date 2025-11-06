using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ProductApi.Application.Models;
using ProductApi.Application.Services;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using Xunit;

namespace ProductApi.Tests.Services;

public class ProductServiceTests
{
    private static ProductService CreateService(ProductDbContext context)
    {
        var repository = new ProductRepository(context);
        var logger = NullLogger<ProductService>.Instance;
        return new ProductService(repository, logger);
    }

    private static ProductDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ProductDbContext(options);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidRequest_PersistsProduct()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var request = new ProductCreateRequest("Keyboard", "Mechanical keyboard", 120.5m);

        var result = await service.CreateProductAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Price, result.Price);
        Assert.Single(context.Products);
    }

    [Fact]
    public async Task UpsertProductAsync_WhenProductExists_UpdatesFields()
    {
        await using var context = CreateContext();
        var existing = new ProductApi.Domain.Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = "Mouse",
            Description = "Wireless mouse",
            Price = 50m,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };
        context.Products.Add(existing);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var request = new ProductUpsertRequest("Mouse", "Updated description", 60m);

        var result = await service.UpsertProductAsync(existing.Id, request);

        Assert.False(result.IsNew);
        Assert.Equal(60m, result.Product.Price);
        Assert.Equal("Updated description", result.Product.Description);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenNotFound_ReturnsFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var deleted = await service.DeleteProductAsync(Guid.NewGuid());

        Assert.False(deleted);
    }
}
