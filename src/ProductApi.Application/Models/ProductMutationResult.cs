namespace ProductApi.Application.Models;

public record ProductMutationResult(ProductDto Product, bool IsNew);
