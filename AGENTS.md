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

### Layer Responsibilities

**Controllers** (`controllers/` folder):
- Inherit from `ControllerBase`
- Use `[ApiController]` and `[Route("route-name")]` attributes
- Handle HTTP methods (`[HttpGet]`, `[HttpPost]`, etc.)
- Accept interface dependencies via constructor injection
- Return model objects directly (framework handles serialization)
- Inject `ILogger<ControllerName>` and service interfaces only
- Keep logic minimal - delegate to services

**Services** (`services/` folder):
- Implement interfaces from `interfaces/services/`
- Contain business logic and orchestration
- Inject repository interfaces and `ILogger<ServiceName>`
- Perform data transformation and validation
- Handle error logging and business rules

**Repositories** (`repositories/` folder):
- Implement interfaces from `interfaces/repositories/`
- Handle data access (HTTP clients, databases, external APIs)
- Inject `HttpClient`, `IConfiguration`, `ILogger<RepositoryName>`
- Return raw data (strings, DTOs) for services to process
- Handle exceptions and log errors

**Interfaces**:
- Services: `interfaces/services/I{Name}Service.cs`
- Repositories: `interfaces/repositories/I{Name}Repository.cs`
- All interfaces are public
- Methods are `public` and use `async Task<T>` for asynchronous operations

**Models** (`models/` folder):
- Plain C# classes for data transfer
- Use PascalCase for class names, camelCase for properties
- Include `namespace dotnet_8_backend_template.models;`

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

## Code Generation Templates

### 1. Creating a New Feature (Complete Flow)

When creating a new feature (e.g., "User"), generate ALL layers:

#### Step 1: Create the Model
```csharp
// models/UserResponseModel.cs
namespace dotnet_8_backend_template.models;

public class UserResponseModel
{
    public int id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
}
```

#### Step 2: Create Repository Interface
```csharp
// interfaces/repositories/IUserRepository.cs
namespace dotnet_8_backend_template.interfaces.repositories;

public interface IUserRepository
{
    public Task<string> GetUserById(int userId);
}
```

#### Step 3: Create Repository Implementation
```csharp
// repositories/UserRepository.cs
using dotnet_8_backend_template.interfaces.repositories;

namespace dotnet_8_backend_template.repositories;

public class UserRepository : IUserRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(HttpClient httpClient, IConfiguration configuration, ILogger<UserRepository> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetUserById(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/users/{userId}");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception error)
        {
            _logger.LogError(error, error.Message);
            throw;
        }
    }
}
```

#### Step 4: Create Service Interface
```csharp
// interfaces/services/IUserService.cs
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.interfaces.services;

public interface IUserService
{
    public Task<UserResponseModel> GetUserById(int userId);
}
```

#### Step 5: Create Service Implementation
```csharp
// services/UserService.cs
using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponseModel> GetUserById(int userId)
    {
        _logger.LogInformation($"[UserService] Fetching user with ID: {userId}");
        string responseBody = await _userRepository.GetUserById(userId);
        _logger.LogInformation($"[UserService] Successfully received user data");

        // Parse and transform data here
        UserResponseModel responseModel = ParseUserData(responseBody);
        return responseModel;
    }

    private UserResponseModel ParseUserData(string responseBody)
    {
        // Implement parsing logic
        throw new NotImplementedException();
    }
}
```

#### Step 6: Create Controller
```csharp
// controllers/UserController.cs
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_8_backend_template.controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<UserResponseModel> GetUser(int id)
    {
        return await _userService.GetUserById(id);
    }
}
```

#### Step 7: Register Dependencies in Program.cs
**CRITICAL**: Always add dependency injection registration to `SetupDependencyInjection()` method:

```csharp
// In Program.cs SetupDependencyInjection() method
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddHttpClient<IUserRepository, UserRepository>(
    client => client.BaseAddress = new Uri(builder.Configuration["USER_API_URL"]!)
);
```

## Dependency Injection Rules

### Registration Location
ALL dependency injection is configured in `Program.cs` within the `SetupDependencyInjection()` local function.

### Service Lifetimes
- **Services**: Use `AddTransient<IService, ServiceImpl>()`
- **Repositories with HttpClient**: Use `AddHttpClient<IRepository, RepositoryImpl>()`
- **Configuration**: Access via `builder.Configuration["KEY_NAME"]`

### Registration Pattern
```csharp
void SetupDependencyInjection()
{
    // Services (business logic) - Transient lifetime
    builder.Services.AddTransient<I{Entity}Service, {Entity}Service>();

    // Repositories (data access) - HttpClient factory pattern
    builder.Services.AddHttpClient<I{Entity}Repository, {Entity}Repository>(
        client => client.BaseAddress = new Uri(builder.Configuration["API_BASE_URL"]!)
    );
}
```

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
In repositories, inject `IConfiguration` and access values:
```csharp
string apiUrl = _configuration["API_KEY_NAME"];
```

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
- **Controllers**: Minimal logging, mainly for request tracking
- **Services**: Log business logic flow, key decisions, data transformations
- **Repositories**: Log external calls, errors, response status

## HTTP Client Usage

### Repository Pattern with HttpClient
```csharp
public class ExampleRepository : IExampleRepository
{
    private readonly HttpClient _httpClient; // Injected via AddHttpClient
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExampleRepository> _logger;

    public ExampleRepository(HttpClient httpClient, IConfiguration configuration, ILogger<ExampleRepository> logger)
    {
        _httpClient = httpClient; // BaseAddress already set in DI
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> FetchData()
    {
        try
        {
            // BaseAddress is already set, use relative path
            var response = await _httpClient.GetAsync("/endpoint");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception error)
        {
            _logger.LogError(error, error.Message);
            throw;
        }
    }
}
```

## Error Handling

### Repository Layer
```csharp
try
{
    // Data access code
    var response = await _httpClient.GetAsync(endpoint);
    return await response.Content.ReadAsStringAsync();
}
catch (Exception error)
{
    _logger.LogError(error, error.Message);
    throw; // Re-throw to let upper layers handle
}
```

### Service Layer
```csharp
public async Task<ResultModel> ProcessData()
{
    try
    {
        _logger.LogInformation("[ServiceName] Starting operation");
        var data = await _repository.GetData();
        // Process data
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"[ServiceName] Failed to process: {ex.Message}");
        throw; // Or return error model
    }
}
```

## Async/Await Patterns

### Standard Async Method
```csharp
public async Task<ReturnType> MethodName()
{
    return await _dependency.AsyncMethod();
}
```

### DO NOT use `.Result` or `.Wait()`
**AVOID**:
```csharp
var result = _httpClient.GetAsync(url).Result; // ❌ BLOCKS THREAD
```

**PREFER**:
```csharp
var result = await _httpClient.GetAsync(url); // ✅ NON-BLOCKING
```

**EXCEPTION**: Only use `.Result` if absolutely necessary (e.g., in existing codebase pattern, but flag for refactoring).

## Swagger/OpenAPI

### Automatic API Documentation
- Swagger UI available at `/swagger` in Development environment
- Controllers automatically generate OpenAPI specifications
- Use XML comments for detailed documentation:

```csharp
/// <summary>
/// Retrieves user information by ID
/// </summary>
/// <param name="id">The user's unique identifier</param>
/// <returns>User details</returns>
[HttpGet("{id}")]
public async Task<UserResponseModel> GetUser(int id)
{
    return await _userService.GetUserById(id);
}
```

## Docker Support

### Dockerfile Pattern
- Multi-stage build (base, build, publish, final)
- Uses .NET 8.0 runtime and SDK
- Exposes ports 8080 (HTTP) and 8081 (HTTPS)
- Entry point: `dotnet dotnet-8-backend-template.dll`

### Building and Running
```bash
# Build image
docker build -t dotnet-8-backend-template .

# Run container
docker run -p 8080:8080 -p 8081:8081 dotnet-8-backend-template
```

## Development Workflow

### Adding a New Feature Checklist
1. ✅ Create model(s) in `models/`
2. ✅ Create repository interface in `interfaces/repositories/`
3. ✅ Implement repository in `repositories/`
4. ✅ Create service interface in `interfaces/services/`
5. ✅ Implement service in `services/`
6. ✅ Create controller in `controllers/`
7. ✅ Register dependencies in `Program.cs` → `SetupDependencyInjection()`
8. ✅ Add configuration keys to `appsettings.json` if needed
9. ✅ Test endpoint via Swagger UI

### Common Commands
```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run

# Run with watch (hot reload)
dotnet watch run

# Publish for production
dotnet publish -c Release
```

## Code Style Guidelines

### General Rules
- Use **file-scoped namespaces** (`namespace Name;` not `namespace Name { }`)
- Enable **nullable reference types** (already enabled in project)
- Use **implicit usings** (enabled by default in .NET 8)
- Keep methods focused and small
- Follow **async all the way** pattern

### Formatting
- Indentation: 4 spaces
- Braces: Opening brace on same line for methods, new line for classes
- Access modifiers: Always explicit (`public`, `private`, `protected`)

### Comments
- Use XML comments for public APIs
- Inline comments for complex business logic only
- Avoid obvious comments

## Testing Endpoints

### Using Swagger UI
1. Run application: `dotnet run`
2. Navigate to: `https://localhost:{port}/swagger`
3. Test endpoints interactively

### Using .http file
The project includes `dotnet-8-backend-template.http` for HTTP request testing (VS Code REST Client extension).

## Key Principles for AI Agents

When generating code for this project:

1. **Always follow the three-layer pattern**: Controller → Service → Repository
2. **Create ALL layers**: Don't create just a controller; create the complete stack
3. **Register in DI**: Never forget to add services/repositories to `Program.cs`
4. **Use interfaces**: Every service and repository must have an interface
5. **Inject ILogger**: Every class should have logging capability
6. **Async patterns**: Use `async`/`await` consistently
7. **Naming consistency**: Follow the exact naming conventions shown
8. **Minimal controllers**: Keep controllers thin, logic in services
9. **Error handling**: Try-catch in repositories, log and re-throw
10. **Configuration**: External URLs and settings go in `appsettings.json`

## Example: Full Feature Implementation

See the "Creating a New Feature" section above for a complete, step-by-step example of implementing a new API endpoint following all project conventions.

## Questions or Issues?

When encountering ambiguity:
1. Follow the existing `IpController` → `IpService` → `IpRepository` pattern
2. Check `Program.cs` for dependency registration patterns
3. Maintain consistency with existing code structure
4. Prioritize clean architecture and separation of concerns

---

**Last Updated**: 2025-10-15
**Project Version**: .NET 8.0
**Architecture**: Controller-Service-Repository Pattern with Dependency Injection
