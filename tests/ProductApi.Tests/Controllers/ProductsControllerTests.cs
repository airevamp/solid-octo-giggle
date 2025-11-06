using System.Net;
using System.Net.Http.Json;
using ProductApi.Application.Models;
using Xunit;

namespace ProductApi.Tests.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreated()
    {
        var request = new ProductCreateRequest("Laptop", "Gaming laptop", 1500m);

        var response = await _client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(created);
        Assert.Equal(request.Name, created!.Name);
    }
}
