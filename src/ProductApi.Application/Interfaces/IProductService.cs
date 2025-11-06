using ProductApi.Application.Models;

namespace ProductApi.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateProductAsync(ProductCreateRequest request, CancellationToken cancellationToken = default);
    Task<ProductMutationResult> UpsertProductAsync(Guid id, ProductUpsertRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}
