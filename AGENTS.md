# AGENTS.md - AI Agent Instructions

This document provides essential information for AI agents (GitHub Copilot, Claude, etc.) to effectively generate code for this .NET 8 backend template project.

## Project Overview

This is a **.NET 8 Web API** template following the **Controller-Service-Repository** pattern with dependency injection, designed for clean architecture and maintainability.

**Tech Stack:**
- .NET 8.0
- ASP.NET Core Web API
- Swagger/OpenAPI (Swashbuckle)
- Docker support
- Minimal API configuration with Controllers

## Layer-Specific Guides

**For detailed, layer-specific instructions, refer to these dedicated guides:**

- **[Controllers Guide](controllers/AGENTS.md)** - HTTP layer patterns, routing, request/response handling, complete examples
- **[Services Guide](services/AGENTS.md)** - Business logic, data transformation, orchestration, parsing patterns
- **[Repositories Guide](repositories/AGENTS.md)** - Data access, HTTP clients, external APIs, error handling
- **[Interfaces Guide](interfaces/AGENTS.md)** - Interface contracts for services and repositories, method signatures
- **[Models Guide](models/AGENTS.md)** - Data transfer objects, request/response models, validation
- **[Testing Guide](tests/AGENTS.md)** - Unit tests, integration tests, testing patterns, no-comment philosophy

**💡 Always start with the layer you're working on for detailed examples and best practices.**

## Architecture Pattern: Controller → Service → Repository

This project strictly follows a three-layer architecture:

```
┌─────────────┐
│ Controller  │ ← HTTP layer (routing, request/response handling)
└──────┬──────┘
       │ calls
┌──────▼──────┐
│  Service    │ ← Business logic layer
└──────┬──────┘
       │ calls
┌──────▼──────┐
│ Repository  │ ← Data access layer (HTTP clients, DB access, external APIs)
└─────────────┘
```

### Quick Layer Reference

| Layer | Responsibility | Injects | Returns | Details |
|-------|---------------|---------|---------|---------|
| **Controller** | HTTP routing & requests | `ILogger`, `IService` | Models | [See Controllers Guide](controllers/AGENTS.md) |
| **Service** | Business logic | `ILogger`, `IRepository` | Models | [See Services Guide](services/AGENTS.md) |
| **Repository** | Data access | `HttpClient`, `IConfiguration`, `ILogger` | Raw strings | [See Repositories Guide](repositories/AGENTS.md) |
| **Interface** | Contracts | N/A | N/A | [See Interfaces Guide](interfaces/AGENTS.md) |
| **Model** | Data structures | N/A | N/A | [See Models Guide](models/AGENTS.md) |

## Naming Conventions

### File Structure
```
controllers/
  {Entity}Controller.cs

services/
  {Entity}Service.cs

repositories/
  {Entity}Repository.cs

interfaces/
  services/
    I{Entity}Service.cs
  repositories/
    I{Entity}Repository.cs

models/
  {Entity}ResponseModel.cs
  {Entity}RequestModel.cs

tests/
  controllers/
    {Entity}ControllerTests.cs
  services/
    {Entity}ServiceTests.cs
```

### Naming Rules
- **Classes**: PascalCase (e.g., `IpController`, `IpService`, `IpRepository`)
- **Interfaces**: Prefix with `I` (e.g., `IIpService`, `IIpRepository`)
- **Properties**: camelCase for models (e.g., `public string ip { get; set; }`)
- **Private fields**: Prefix with underscore (e.g., `_logger`, `_ipService`, `_httpClient`)
- **Methods**: PascalCase, descriptive verbs (e.g., `GetCurrentMachinePublicIp`)
- **Route names**: lowercase, hyphen-separated (e.g., `[Route("ip")]`, `[Route("user-profile")]`)

### Namespace Convention
All files use: `namespace dotnet_8_backend_template.{folder};`
- Controllers: `dotnet_8_backend_template.controllers`
- Services: `dotnet_8_backend_template.services`
- Repositories: `dotnet_8_backend_template.repositories`
- Interfaces: `dotnet_8_backend_template.interfaces.services` or `.repositories`
- Models: `dotnet_8_backend_template.models`
- Tests: `dotnet_8_backend_template.tests.{layer}`

## Creating a New Feature - Quick Checklist

When adding a new feature (e.g., "User"), follow these steps:

1. ✅ **Create Model** - `models/UserResponseModel.cs` ([Models Guide](models/AGENTS.md))
2. ✅ **Create Repository Interface** - `interfaces/repositories/IUserRepository.cs` ([Interfaces Guide](interfaces/AGENTS.md))
3. ✅ **Create Repository** - `repositories/UserRepository.cs` ([Repositories Guide](repositories/AGENTS.md))
4. ✅ **Create Service Interface** - `interfaces/services/IUserService.cs` ([Interfaces Guide](interfaces/AGENTS.md))
5. ✅ **Create Service** - `services/UserService.cs` ([Services Guide](services/AGENTS.md))
6. ✅ **Create Controller** - `controllers/UserController.cs` ([Controllers Guide](controllers/AGENTS.md))
7. ✅ **Register in DI** - `Program.cs` → `SetupDependencyInjection()` (see below)
8. ✅ **Add Configuration** - `appsettings.json` if needed (see below)
9. ✅ **Create Tests** - `tests/services/` and `tests/controllers/` ([Testing Guide](tests/AGENTS.md))

**For complete code examples of each step, see the respective layer guide.**

## Dependency Injection

### Registration Location
ALL dependency injection is configured in `Program.cs` within the `SetupDependencyInjection()` local function.

### Registration Pattern
```csharp
void SetupDependencyInjection()
{
    builder.Services.AddTransient<I{Entity}Service, {Entity}Service>();

    builder.Services.AddHttpClient<I{Entity}Repository, {Entity}Repository>(
        client => client.BaseAddress = new Uri(builder.Configuration["API_BASE_URL"]!)
    );
}
```

**Service Lifetimes:**
- **Services**: Use `AddTransient<IService, ServiceImpl>()`
- **Repositories with HttpClient**: Use `AddHttpClient<IRepository, RepositoryImpl>()`

**For complete examples, see [Repositories Guide](repositories/AGENTS.md) and [Services Guide](services/AGENTS.md).**

## Configuration Management

### appsettings.json Structure
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "IP_API_URL": "https://api.example.com",
  "USER_API_URL": "https://users.example.com"
}
```

### Accessing Configuration
In repositories, inject `IConfiguration`:
```csharp
string apiUrl = _configuration["API_KEY_NAME"];
```

**For complete examples, see [Repositories Guide](repositories/AGENTS.md).**

## Logging Standards

### Log Levels
- `LogInformation`: Normal operation flow
- `LogWarning`: Unexpected but handled situations
- `LogError`: Exceptions and failures

### Logging Pattern
```csharp
_logger.LogInformation($"[{ClassName}] Descriptive message with {variable}");
_logger.LogError(exception, exception.Message);
```

### Where to Log
- **Controllers**: Minimal logging ([Controllers Guide](controllers/AGENTS.md))
- **Services**: Business logic flow ([Services Guide](services/AGENTS.md))
- **Repositories**: External calls, errors ([Repositories Guide](repositories/AGENTS.md))

## Error Handling

### Standard Pattern
```csharp
try
{
    var response = await _httpClient.GetAsync(endpoint);
    return await response.Content.ReadAsStringAsync();
}
catch (Exception error)
{
    _logger.LogError(error, error.Message);
    throw;
}
```

**For layer-specific error handling, see:**
- [Repositories Guide](repositories/AGENTS.md) - External API error handling
- [Services Guide](services/AGENTS.md) - Business logic error handling

## Code Style Guidelines

### General Rules
- Use **file-scoped namespaces** (`namespace Name;` not `namespace Name { }`)
- Enable **nullable reference types** (already enabled)
- Use **implicit usings** (enabled by default in .NET 8)
- Follow **async all the way** pattern
- **DO NOT** use `.Result` or `.Wait()` - always use `await`

### Formatting
- Indentation: 4 spaces
- Braces: Opening brace on same line for methods, new line for classes
- Access modifiers: Always explicit (`public`, `private`, `protected`)

### No Comments Policy
- Use XML comments for public APIs only
- Avoid inline comments - write self-explanatory code
- Use descriptive variable names instead of comments
- **Tests have ZERO comments** - see [Testing Guide](tests/AGENTS.md)

## Development Commands

```bash
dotnet restore          # Restore dependencies
dotnet build            # Build project
dotnet run              # Run application
dotnet watch run        # Run with hot reload
dotnet test             # Run all tests
dotnet publish -c Release   # Publish for production
```

## Swagger/OpenAPI

- Swagger UI available at `/swagger` in Development environment
- Controllers automatically generate OpenAPI specifications
- Use XML comments for detailed API documentation

## Docker Support

```bash
docker build -t dotnet-8-backend-template .
docker run -p 8080:8080 -p 8081:8081 dotnet-8-backend-template
```

## Key Principles for AI Agents

When generating code for this project:

1. **Follow the three-layer pattern** - Controller → Service → Repository
2. **Create ALL layers** - Don't create just a controller; create the complete stack
3. **Register in DI** - Never forget to add services/repositories to `Program.cs`
4. **Use interfaces** - Every service and repository must have an interface
5. **Inject ILogger** - Every class should have logging capability
6. **Use async/await** - All I/O operations must be async
7. **Follow naming conventions** - See naming rules above
8. **Keep controllers thin** - Business logic belongs in services
9. **Handle errors properly** - Try-catch in repositories, log and re-throw
10. **Refer to layer guides** - Always check the specific layer guide for details

## Where to Find Detailed Information

| Topic | Guide |
|-------|-------|
| HTTP routing, endpoints, attributes | [Controllers Guide](controllers/AGENTS.md) |
| Business logic, data transformation | [Services Guide](services/AGENTS.md) |
| HTTP clients, external APIs, data access | [Repositories Guide](repositories/AGENTS.md) |
| Interface contracts, method signatures | [Interfaces Guide](interfaces/AGENTS.md) |
| Request/response models, validation | [Models Guide](models/AGENTS.md) |
| Unit tests, integration tests, mocking | [Testing Guide](tests/AGENTS.md) |

## Questions or Issues?

When encountering ambiguity:
1. **Check the layer-specific guide** for detailed examples
2. Follow the existing `IpController` → `IpService` → `IpRepository` pattern
3. Check `Program.cs` for dependency registration patterns
4. Maintain consistency with existing code structure
5. Prioritize clean architecture and separation of concerns

---

**Last Updated**: 2025-10-15
**Project Version**: .NET 8.0
**Architecture**: Controller-Service-Repository Pattern with Dependency Injection

**📖 For detailed examples and patterns, always refer to the layer-specific guides above.**
