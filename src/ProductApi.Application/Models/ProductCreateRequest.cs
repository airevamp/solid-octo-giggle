using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.Models;

public record ProductCreateRequest(
    [property: Required, StringLength(200)] string Name,
    string? Description,
    [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Price);
