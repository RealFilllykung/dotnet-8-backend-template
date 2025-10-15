# Repositories Layer - AI Agent Instructions

This guide provides specific instructions for creating and maintaining **Repositories** in this .NET 8 Web API project.

## Repository Responsibilities

Repositories are the **data access layer** that:
- Handle external data sources (APIs, databases, file systems)
- Make HTTP requests to external services
- Perform database queries and operations
- Return raw data (strings, primitives) to services
- Handle connection errors and retries
- Log external interactions

## Repository Structure

### Basic Repository Template

```csharp
using dotnet_8_backend_template.interfaces.repositories;

namespace dotnet_8_backend_template.repositories;

public class EntityRepository : IEntityRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EntityRepository> _logger;

    public EntityRepository(HttpClient httpClient, IConfiguration configuration, ILogger<EntityRepository> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetEntityData()
    {
        try
        {
            _logger.LogInformation("[EntityRepository] Fetching entity data from API");
            var response = await _httpClient.GetAsync("/entities");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("[EntityRepository] Successfully received entity data");
            return responseBody;
        }
        catch (Exception error)
        {
            _logger.LogError(error, $"[EntityRepository] Failed to fetch entity data: {error.Message}");
            throw;
        }
    }
}
```

## Naming Conventions

### File Naming
- **File name**: `{Entity}Repository.cs`
- **Examples**: `IpRepository.cs`, `UserRepository.cs`, `ProductRepository.cs`

### Class Naming
- **Class name**: `{Entity}Repository`
- **Implements**: `I{Entity}Repository`
- **Namespace**: `dotnet_8_backend_template.repositories`

### Method Naming
Use descriptive names that indicate the data source:
- `Get{Data}FromApi` - Fetch from external API
- `Get{Data}FromDatabase` - Fetch from database
- `Fetch{Data}` - Generic data retrieval
- `Post{Data}ToApi` - Send data to external API
- `Update{Data}InDatabase` - Update in database
- `Delete{Data}FromApi` - Remove from external API

## Dependency Injection

Repositories typically inject:
1. **HttpClient** - For making HTTP requests (injected via AddHttpClient)
2. **IConfiguration** - For accessing configuration values
3. **ILogger<RepositoryName>** - For logging

```csharp
private readonly HttpClient _httpClient;
private readonly IConfiguration _configuration;
private readonly ILogger<EntityRepository> _logger;

public EntityRepository(HttpClient httpClient, IConfiguration configuration, ILogger<EntityRepository> logger)
{
    _httpClient = _httpClient;
    _configuration = configuration;
    _logger = logger;
}
```

## HTTP Client Usage

### GET Requests

**Simple GET**:
```csharp
public async Task<string> GetEntityData()
{
    try
    {
        _logger.LogInformation("[EntityRepository] Fetching entity data");
        var response = await _httpClient.GetAsync("/entities");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception error)
    {
        _logger.LogError(error, error.Message);
        throw;
    }
}
```

**GET with ID**:
```csharp
public async Task<string> GetEntityById(int id)
{
    try
    {
        _logger.LogInformation($"[EntityRepository] Fetching entity with ID: {id}");
        var response = await _httpClient.GetAsync($"/entities/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception error)
    {
        _logger.LogError(error, $"[EntityRepository] Failed to fetch entity {id}: {error.Message}");
        throw;
    }
}
```

**GET with Query Parameters**:
```csharp
public async Task<string> SearchEntities(string query, int page)
{
    try
    {
        _logger.LogInformation($"[EntityRepository] Searching entities: {query}, page {page}");
        var response = await _httpClient.GetAsync($"/entities?q={query}&page={page}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception error)
    {
        _logger.LogError(error, error.Message);
        throw;
    }
}
```

### POST Requests

**POST with JSON payload**:
```csharp
public async Task<string> CreateEntity(string jsonPayload)
{
    try
    {
        _logger.LogInformation("[EntityRepository] Creating new entity");
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/entities", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception error)
    {
        _logger.LogError(error, $"[EntityRepository] Failed to create entity: {error.Message}");
        throw;
    }
}
```

### PUT Requests

**PUT with JSON payload**:
```csharp
public async Task<string> UpdateEntity(int id, string jsonPayload)
{
    try
    {
        _logger.LogInformation($"[EntityRepository] Updating entity {id}");
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/entities/{id}", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception error)
    {
        _logger.LogError(error, $"[EntityRepository] Failed to update entity {id}: {error.Message}");
        throw;
    }
}
```

### DELETE Requests

**DELETE by ID**:
```csharp
public async Task DeleteEntity(int id)
{
    try
    {
        _logger.LogInformation($"[EntityRepository] Deleting entity {id}");
        var response = await _httpClient.DeleteAsync($"/entities/{id}");
        response.EnsureSuccessStatusCode();
    }
    catch (Exception error)
    {
        _logger.LogError(error, $"[EntityRepository] Failed to delete entity {id}: {error.Message}");
        throw;
    }
}
```

## Configuration Access

### Reading Configuration Values

```csharp
public async Task<string> GetEntityData()
{
    string apiKey = _configuration["ENTITY_API_KEY"]!;
    string apiUrl = _configuration["ENTITY_API_URL"]!;

    _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

    var response = await _httpClient.GetAsync(apiUrl);
    return await response.Content.ReadAsStringAsync();
}
```

### Using Nested Configuration

```csharp
public async Task<string> GetEntityData()
{
    string apiKey = _configuration["ExternalApis:EntityApi:ApiKey"]!;
    string endpoint = _configuration["ExternalApis:EntityApi:Endpoint"]!;

    var response = await _httpClient.GetAsync(endpoint);
    return await response.Content.ReadAsStringAsync();
}
```

## Error Handling

### Standard Error Handling Pattern

```csharp
public async Task<string> GetEntityData()
{
    try
    {
        _logger.LogInformation("[EntityRepository] Starting data fetch");
        var response = await _httpClient.GetAsync("/entities");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (HttpRequestException error)
    {
        _logger.LogError(error, $"[EntityRepository] HTTP error: {error.Message}");
        throw;
    }
    catch (TaskCanceledException error)
    {
        _logger.LogError(error, $"[EntityRepository] Request timeout: {error.Message}");
        throw;
    }
    catch (Exception error)
    {
        _logger.LogError(error, $"[EntityRepository] Unexpected error: {error.Message}");
        throw;
    }
}
```

### Specific HTTP Status Code Handling

```csharp
public async Task<string> GetEntityById(int id)
{
    try
    {
        _logger.LogInformation($"[EntityRepository] Fetching entity {id}");
        var response = await _httpClient.GetAsync($"/entities/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning($"[EntityRepository] Entity {id} not found");
            throw new KeyNotFoundException($"Entity {id} not found");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception error)
    {
        _logger.LogError(error, error.Message);
        throw;
    }
}
```

## Logging

### Log Levels

**Information Logging**:
```csharp
_logger.LogInformation("[EntityRepository] Starting operation");
_logger.LogInformation($"[EntityRepository] Fetching entity with ID: {id}");
_logger.LogInformation("[EntityRepository] Operation completed successfully");
```

**Warning Logging**:
```csharp
_logger.LogWarning($"[EntityRepository] Entity {id} not found in cache, fetching from API");
_logger.LogWarning("[EntityRepository] Rate limit approaching");
```

**Error Logging**:
```csharp
_logger.LogError(exception, $"[EntityRepository] Failed to fetch data: {exception.Message}");
_logger.LogError(exception, exception.Message);
```

### Logging Pattern
Always prefix log messages with `[RepositoryName]` for easy filtering:
```csharp
_logger.LogInformation("[UserRepository] Fetching user data");
_logger.LogError(error, "[UserRepository] Failed to create user");
```

## Complete Repository Examples

### Simple HTTP Repository

```csharp
using dotnet_8_backend_template.interfaces.repositories;

namespace dotnet_8_backend_template.repositories;

public class IpRepository : IIpRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IpRepository> _logger;

    public IpRepository(HttpClient httpClient, IConfiguration configuration, ILogger<IpRepository> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetCurrentPublicIp()
    {
        try
        {
            _logger.LogInformation("[IpRepository] Fetching current public IP");
            var response = await _httpClient.GetAsync("/");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("[IpRepository] Successfully received IP data");
            return responseBody;
        }
        catch (Exception error)
        {
            _logger.LogError(error, $"[IpRepository] Failed to fetch IP: {error.Message}");
            throw;
        }
    }
}
```

### Complex HTTP Repository with CRUD

```csharp
using System.Text;
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

    public async Task<string> GetAllUsersFromApi()
    {
        try
        {
            _logger.LogInformation("[UserRepository] Fetching all users");
            var response = await _httpClient.GetAsync("/users");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception error)
        {
            _logger.LogError(error, $"[UserRepository] Failed to fetch users: {error.Message}");
            throw;
        }
    }

    public async Task<string> GetUserByIdFromApi(int id)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] Fetching user {id}");
            var response = await _httpClient.GetAsync($"/users/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception error)
        {
            _logger.LogError(error, $"[UserRepository] Failed to fetch user {id}: {error.Message}");
            throw;
        }
    }

    public async Task<string> CreateUserInApi(string jsonPayload)
    {
        try
        {
            _logger.LogInformation("[UserRepository] Creating new user");
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/users", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception error)
        {
            _logger.LogError(error, $"[UserRepository] Failed to create user: {error.Message}");
            throw;
        }
    }

    public async Task<string> UpdateUserInApi(int id, string jsonPayload)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] Updating user {id}");
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/users/{id}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception error)
        {
            _logger.LogError(error, $"[UserRepository] Failed to update user {id}: {error.Message}");
            throw;
        }
    }

    public async Task DeleteUserInApi(int id)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] Deleting user {id}");
            var response = await _httpClient.DeleteAsync($"/users/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception error)
        {
            _logger.LogError(error, $"[UserRepository] Failed to delete user {id}: {error.Message}");
            throw;
        }
    }
}
```

## Dependency Registration

Repositories must be registered in `Program.cs` using `AddHttpClient`:

```csharp
void SetupDependencyInjection()
{
    builder.Services.AddHttpClient<IEntityRepository, EntityRepository>(
        client => client.BaseAddress = new Uri(builder.Configuration["ENTITY_API_URL"]!)
    );
}
```

## Best Practices

1. **Return raw data** - Return strings or primitives, let services parse
2. **Log all external calls** - Log before and after HTTP requests
3. **Use try-catch** - Catch exceptions, log, and re-throw
4. **Use HttpClient correctly** - Inject via AddHttpClient, don't create instances
5. **Set BaseAddress in DI** - Configure base URL during registration
6. **Use relative paths** - HttpClient already has BaseAddress set
7. **Handle timeouts** - Catch TaskCanceledException for timeout scenarios
8. **Check status codes** - Use EnsureSuccessStatusCode() or check manually
9. **Use async/await** - All I/O operations should be async
10. **Prefix logs with repository name** - Makes debugging easier

## Common Mistakes to Avoid

❌ **DO NOT** create HttpClient instances manually
```csharp
public async Task<string> GetData()
{
    using var client = new HttpClient();  // Wrong! Memory leak risk
    return await client.GetAsync("url");
}
```

✅ **DO** inject HttpClient
```csharp
private readonly HttpClient _httpClient;

public EntityRepository(HttpClient httpClient, ...)
{
    _httpClient = httpClient;
}
```

❌ **DO NOT** parse data in repositories
```csharp
public async Task<UserResponseModel> GetUser(int id)
{
    var response = await _httpClient.GetAsync($"/users/{id}");
    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<UserResponseModel>(json);  // Wrong! Parsing is service responsibility
}
```

✅ **DO** return raw data
```csharp
public async Task<string> GetUserDataFromApi(int id)
{
    var response = await _httpClient.GetAsync($"/users/{id}");
    return await response.Content.ReadAsStringAsync();  // Correct! Return raw string
}
```

❌ **DO NOT** swallow exceptions
```csharp
public async Task<string> GetData()
{
    try
    {
        return await _httpClient.GetAsync("/data");
    }
    catch (Exception error)
    {
        _logger.LogError(error, error.Message);
        return "";  // Wrong! Hides error from caller
    }
}
```

✅ **DO** re-throw exceptions
```csharp
public async Task<string> GetData()
{
    try
    {
        return await _httpClient.GetAsync("/data");
    }
    catch (Exception error)
    {
        _logger.LogError(error, error.Message);
        throw;  // Correct! Let caller handle
    }
}
```

## Checklist for Creating Repositories

- ✅ File named `{Entity}Repository.cs`
- ✅ Class named `{Entity}Repository`
- ✅ Implements `I{Entity}Repository` interface
- ✅ Namespace is `dotnet_8_backend_template.repositories`
- ✅ Injects HttpClient, IConfiguration, ILogger
- ✅ All methods are async (return Task or Task<T>)
- ✅ Methods return raw data (strings, primitives)
- ✅ Try-catch blocks around all external calls
- ✅ Logging before and after operations
- ✅ Exceptions are logged and re-thrown
- ✅ Uses EnsureSuccessStatusCode() or manual status checks
- ✅ Registered in Program.cs with AddHttpClient

## Related Documentation

- See [../services/AGENTS.md](../services/AGENTS.md) for how services use repositories
- See [../interfaces/AGENTS.md](../interfaces/AGENTS.md) for repository interfaces
- See [../AGENTS.md](../AGENTS.md) for dependency injection patterns
- See [../tests/AGENTS.md](../tests/AGENTS.md) for repository testing patterns

---

**Last Updated**: 2025-10-15
**Layer**: Repositories (Data Access Layer)
**Pattern**: Controller → Service → Repository
