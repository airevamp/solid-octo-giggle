using Microsoft.Extensions.Logging;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Models;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.Services;

public class ProductService : IProductService
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        var skip = (pageNumber - 1) * pageSize;

        var products = await _repository.GetPagedAsync(skip, pageSize, cancellationToken);
        var totalCount = await _repository.CountAsync(cancellationToken);

        var items = products.Select(MapToDto).ToList();

        return new PagedResult<ProductDto>(items, pageNumber, pageSize, totalCount);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request.Name, request.Price);

        var now = DateTimeOffset.UtcNow;
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created product {ProductId}", product.Id);

        return MapToDto(product);
    }

    public async Task<ProductMutationResult> UpsertProductAsync(Guid id, ProductUpsertRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Product id must be provided.");
        }

        ValidateRequest(request.Name, request.Price);

        var product = await _repository.GetByIdAsync(id, cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var created = false;

        if (product is null)
        {
            product = new Product
            {
                Id = id,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Price = request.Price,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _repository.AddAsync(product, cancellationToken);
            created = true;
            _logger.LogInformation("Created product {ProductId} via upsert", id);
        }
        else
        {
            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim();
            product.Price = request.Price;
            product.UpdatedAt = now;

            await _repository.UpdateAsync(product, cancellationToken);
            _logger.LogInformation("Updated product {ProductId}", id);
        }

        await _repository.SaveChangesAsync(cancellationToken);

        var dto = MapToDto(product);
        return new ProductMutationResult(dto, created);
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        await _repository.DeleteAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted product {ProductId}", id);

        return true;
    }

    private static void ValidateRequest(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Name is required.");
        }

        if (price <= 0)
        {
            throw new ValidationException("Price must be greater than zero.");
        }
    }

    private static ProductDto MapToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.CreatedAt,
        product.UpdatedAt);
}
