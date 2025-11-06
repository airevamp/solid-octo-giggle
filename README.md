# solid-octo-giggle

This repository contains a layered sample REST API for managing products, built with .NET 8 and Entity Framework Core.

## Getting started

```bash
# Restore dependencies and run tests
 dotnet test ProductApi.sln

# Run the API locally
 dotnet run --project src/ProductApi.Api/ProductApi.Api.csproj
```

The API listens on the default ASP.NET Core ports and exposes Swagger UI in development for easy exploration.

## Project layout

- `ProductApi.Domain` – Domain entities.
- `ProductApi.Application` – DTOs, service contracts, and application services.
- `ProductApi.Infrastructure` – EF Core DbContext and repository implementation.
- `ProductApi.Api` – ASP.NET Core API project with controllers and DI configuration.
- `ProductApi.Tests` – xUnit test suite covering services and controller endpoints with `WebApplicationFactory`.
