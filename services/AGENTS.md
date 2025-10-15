# Services Layer - AI Agent Instructions

This guide provides specific instructions for creating and maintaining **Services** in this .NET 8 Web API project.

## Service Responsibilities

Services are the **business logic layer** that:
- Implement business rules and logic
- Transform data between repositories and controllers
- Orchestrate multiple repository calls
- Parse and validate data
- Perform calculations and data processing
- Log business operations
- Handle business-level exceptions

## Service Structure

### Basic Service Template

```csharp
using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.services;

public class EntityService : IEntityService
{
    private readonly IEntityRepository _entityRepository;
    private readonly ILogger<EntityService> _logger;

    public EntityService(IEntityRepository entityRepository, ILogger<EntityService> logger)
    {
        _entityRepository = entityRepository;
        _logger = logger;
    }

    public async Task<EntityResponseModel> GetEntity(int id)
    {
        _logger.LogInformation($"[EntityService] Fetching entity with ID: {id}");
        string rawData = await _entityRepository.GetEntityData(id);
        _logger.LogInformation("[EntityService] Successfully received data from repository");

        EntityResponseModel result = ParseEntityData(rawData);
        return result;
    }

    private EntityResponseModel ParseEntityData(string rawData)
    {
        _logger.LogInformation("[EntityService] Parsing entity data");
        EntityResponseModel model = JsonSerializer.Deserialize<EntityResponseModel>(rawData);
        return model;
    }
}
```

## Naming Conventions

### File Naming
- **File name**: `{Entity}Service.cs`
- **Examples**: `IpService.cs`, `UserService.cs`, `ProductService.cs`

### Class Naming
- **Class name**: `{Entity}Service`
- **Implements**: `I{Entity}Service`
- **Namespace**: `dotnet_8_backend_template.services`

### Method Naming
Use descriptive business-oriented names:
- `Get{Entity}` - Retrieve entity
- `Get{Entity}ById` - Retrieve by identifier
- `Create{Entity}` - Create new entity
- `Update{Entity}` - Update existing entity
- `Delete{Entity}` - Remove entity
- `Process{Entity}` - Perform business operation
- `Calculate{Something}` - Perform calculation
- `Validate{Something}` - Validate data
- `Transform{Data}` - Transform data structure

## Dependency Injection

Services typically inject:
1. **Repository Interfaces** - One or more repository interfaces
2. **ILogger<ServiceName>** - For logging business operations
3. **Other Service Interfaces** - When orchestrating multiple services

```csharp
private readonly IEntityRepository _entityRepository;
private readonly ILogger<EntityService> _logger;

public EntityService(IEntityRepository entityRepository, ILogger<EntityService> logger)
{
    _entityRepository = entityRepository;
    _logger = logger;
}
```

### Multiple Dependencies
```csharp
private readonly IUserRepository _userRepository;
private readonly IEmailService _emailService;
private readonly ILogger<UserService> _logger;

public UserService(
    IUserRepository userRepository,
    IEmailService emailService,
    ILogger<UserService> logger)
{
    _userRepository = userRepository;
    _emailService = emailService;
    _logger = logger;
}
```

## Data Transformation

### Parsing Raw Data

**JSON Parsing**:
```csharp
using System.Text.Json;

private EntityResponseModel ParseEntityData(string rawJson)
{
    _logger.LogInformation("[EntityService] Parsing JSON data");
    EntityResponseModel model = JsonSerializer.Deserialize<EntityResponseModel>(rawJson);
    return model;
}
```

**Custom Parsing**:
```csharp
private IpResponseModel ParseIpData(string responseBody)
{
    _logger.LogInformation("[IpService] Parsing IP data");

    string[] colonSplit = responseBody.Split(':');
    string behindSubstring = colonSplit[1].Substring(1);

    int closingBraceIndex = behindSubstring.IndexOf('}');
    string ip;
    if (closingBraceIndex >= 0)
    {
        ip = behindSubstring.Substring(0, closingBraceIndex).Trim();
    }
    else
    {
        string[] tagSplit = behindSubstring.Split('<');
        ip = tagSplit[0].Trim();
    }

    return new IpResponseModel { ip = ip };
}
```

### Model Transformation

**Request to Repository Format**:
```csharp
private string ConvertToJsonPayload(UserRequestModel request)
{
    var payload = new
    {
        username = request.username,
        email = request.email,
        password = HashPassword(request.password)
    };
    return JsonSerializer.Serialize(payload);
}
```

**Repository to Response Format**:
```csharp
private UserResponseModel TransformToResponseModel(string rawData)
{
    var apiResponse = JsonSerializer.Deserialize<ApiUserDto>(rawData);
    return new UserResponseModel
    {
        id = apiResponse.user_id,
        username = apiResponse.user_name,
        email = apiResponse.email_address,
        createdAt = apiResponse.created_timestamp
    };
}
```

## Business Logic Implementation

### Simple Business Logic

```csharp
public async Task<EntityResponseModel> GetEntityById(int id)
{
    _logger.LogInformation($"[EntityService] Fetching entity with ID: {id}");

    string rawData = await _entityRepository.GetEntityDataById(id);
    _logger.LogInformation("[EntityService] Successfully received data");

    EntityResponseModel result = ParseEntityData(rawData);

    _logger.LogInformation($"[EntityService] Successfully processed entity {id}");
    return result;
}
```

### Complex Business Logic with Validation

```csharp
public async Task<UserResponseModel> CreateUser(UserRequestModel request)
{
    _logger.LogInformation($"[UserService] Creating user: {request.username}");

    await ValidateUserDoesNotExist(request.username);

    string hashedPassword = HashPassword(request.password);

    var payload = new
    {
        username = request.username,
        email = request.email,
        password = hashedPassword
    };

    string jsonPayload = JsonSerializer.Serialize(payload);
    string rawResponse = await _userRepository.CreateUserInApi(jsonPayload);

    _logger.LogInformation($"[UserService] User {request.username} created successfully");

    UserResponseModel result = ParseUserData(rawResponse);

    await _emailService.SendWelcomeEmail(result.email);

    return result;
}

private async Task ValidateUserDoesNotExist(string username)
{
    string existingUserData = await _userRepository.GetUserByUsernameFromApi(username);
    if (!string.IsNullOrEmpty(existingUserData))
    {
        _logger.LogWarning($"[UserService] User {username} already exists");
        throw new InvalidOperationException($"User {username} already exists");
    }
}

private string HashPassword(string password)
{
    _logger.LogInformation("[UserService] Hashing password");
    return BCrypt.Net.BCrypt.HashPassword(password);
}
```

### Orchestrating Multiple Repositories

```csharp
public async Task<OrderResponseModel> CreateOrder(OrderRequestModel request)
{
    _logger.LogInformation($"[OrderService] Creating order for user {request.userId}");

    string userData = await _userRepository.GetUserByIdFromApi(request.userId);
    UserResponseModel user = ParseUserData(userData);

    List<ProductResponseModel> products = new List<ProductResponseModel>();
    foreach (var item in request.items)
    {
        string productData = await _productRepository.GetProductByIdFromApi(item.productId);
        ProductResponseModel product = ParseProductData(productData);
        products.Add(product);
    }

    decimal totalAmount = CalculateTotalAmount(request.items, products);

    var orderPayload = CreateOrderPayload(user, request.items, totalAmount);
    string jsonPayload = JsonSerializer.Serialize(orderPayload);

    string rawResponse = await _orderRepository.CreateOrderInApi(jsonPayload);
    OrderResponseModel result = ParseOrderData(rawResponse);

    _logger.LogInformation($"[OrderService] Order {result.id} created successfully");

    return result;
}

private decimal CalculateTotalAmount(List<OrderItemRequest> items, List<ProductResponseModel> products)
{
    decimal total = 0;
    foreach (var item in items)
    {
        var product = products.First(p => p.id == item.productId);
        total += product.price * item.quantity;
    }
    return total;
}
```

## Error Handling

### Standard Error Handling

```csharp
public async Task<EntityResponseModel> GetEntityById(int id)
{
    try
    {
        _logger.LogInformation($"[EntityService] Fetching entity {id}");
        string rawData = await _entityRepository.GetEntityDataById(id);
        EntityResponseModel result = ParseEntityData(rawData);
        return result;
    }
    catch (HttpRequestException error)
    {
        _logger.LogError(error, $"[EntityService] Failed to fetch entity {id}: {error.Message}");
        throw;
    }
    catch (JsonException error)
    {
        _logger.LogError(error, $"[EntityService] Failed to parse entity data: {error.Message}");
        throw;
    }
}
```

### Business Logic Exceptions

```csharp
public async Task<UserResponseModel> UpdateUser(int id, UserRequestModel request)
{
    _logger.LogInformation($"[UserService] Updating user {id}");

    string existingUserData = await _userRepository.GetUserByIdFromApi(id);
    if (string.IsNullOrEmpty(existingUserData))
    {
        _logger.LogWarning($"[UserService] User {id} not found");
        throw new KeyNotFoundException($"User {id} not found");
    }

    string jsonPayload = ConvertToJsonPayload(request);
    string rawResponse = await _userRepository.UpdateUserInApi(id, jsonPayload);

    UserResponseModel result = ParseUserData(rawResponse);
    _logger.LogInformation($"[UserService] User {id} updated successfully");

    return result;
}
```

## Logging

### Service-Level Logging

**Information Logs**:
```csharp
_logger.LogInformation($"[EntityService] Starting operation for entity {id}");
_logger.LogInformation("[EntityService] Data fetched successfully from repository");
_logger.LogInformation("[EntityService] Data transformation completed");
_logger.LogInformation($"[EntityService] Operation completed successfully");
```

**Warning Logs**:
```csharp
_logger.LogWarning($"[UserService] User {username} not found");
_logger.LogWarning("[ProductService] Product stock is low");
```

**Error Logs**:
```csharp
_logger.LogError(exception, $"[EntityService] Failed to process entity: {exception.Message}");
```

### Logging Pattern
Always prefix with `[ServiceName]` for easy log filtering and tracing:
```csharp
_logger.LogInformation("[UserService] Creating new user");
_logger.LogInformation("[UserService] Validating user credentials");
_logger.LogInformation("[UserService] User created successfully");
```

## Complete Service Examples

### Simple Service

```csharp
using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.services;

public class IpService : IIpService
{
    private readonly IIpRepository _ipRepository;
    private readonly ILogger<IpService> _logger;

    public IpService(IIpRepository ipRepository, ILogger<IpService> logger)
    {
        _ipRepository = ipRepository;
        _logger = logger;
    }

    public async Task<IpResponseModel> GetCurrentMachinePublicIp()
    {
        IpResponseModel responseModel = new IpResponseModel();
        _logger.LogInformation("[IpService] Calling IP repository to get IP string");
        string ipResponseBody = await _ipRepository.GetCurrentPublicIp();
        _logger.LogInformation("[IpService] Successfully receive a response of: " + ipResponseBody);
        responseModel.ip = GetIpFromResponseBody(ipResponseBody);
        return responseModel;
    }

    private string GetIpFromResponseBody(string ipResponseBody)
    {
        string[] colonSplit = ipResponseBody.Split(':');
        string behindSubstring = colonSplit[1].Substring(1);

        int closingBraceIndex = behindSubstring.IndexOf('}');
        string ip;
        if (closingBraceIndex >= 0)
        {
            ip = behindSubstring.Substring(0, closingBraceIndex).Trim();
        }
        else
        {
            string[] tagSplit = behindSubstring.Split('<');
            ip = tagSplit[0].Trim();
        }

        return ip;
    }
}
```

### Complex Service with CRUD Operations

```csharp
using System.Text.Json;
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

    public async Task<List<UserResponseModel>> GetAllUsers()
    {
        _logger.LogInformation("[UserService] Fetching all users");
        string rawData = await _userRepository.GetAllUsersFromApi();
        _logger.LogInformation("[UserService] Successfully received all users data");

        List<UserResponseModel> users = JsonSerializer.Deserialize<List<UserResponseModel>>(rawData);
        return users;
    }

    public async Task<UserResponseModel> GetUserById(int id)
    {
        _logger.LogInformation($"[UserService] Fetching user with ID: {id}");
        string rawData = await _userRepository.GetUserByIdFromApi(id);
        _logger.LogInformation($"[UserService] Successfully received user {id} data");

        UserResponseModel user = JsonSerializer.Deserialize<UserResponseModel>(rawData);
        return user;
    }

    public async Task<UserResponseModel> CreateUser(UserRequestModel request)
    {
        _logger.LogInformation($"[UserService] Creating user: {request.username}");

        string hashedPassword = HashPassword(request.password);
        var payload = new
        {
            username = request.username,
            email = request.email,
            password = hashedPassword
        };

        string jsonPayload = JsonSerializer.Serialize(payload);
        string rawResponse = await _userRepository.CreateUserInApi(jsonPayload);

        _logger.LogInformation($"[UserService] User {request.username} created successfully");

        UserResponseModel user = JsonSerializer.Deserialize<UserResponseModel>(rawResponse);
        return user;
    }

    public async Task<UserResponseModel> UpdateUser(int id, UserRequestModel request)
    {
        _logger.LogInformation($"[UserService] Updating user {id}");

        string jsonPayload = JsonSerializer.Serialize(request);
        string rawResponse = await _userRepository.UpdateUserInApi(id, jsonPayload);

        _logger.LogInformation($"[UserService] User {id} updated successfully");

        UserResponseModel user = JsonSerializer.Deserialize<UserResponseModel>(rawResponse);
        return user;
    }

    public async Task DeleteUser(int id)
    {
        _logger.LogInformation($"[UserService] Deleting user {id}");
        await _userRepository.DeleteUserInApi(id);
        _logger.LogInformation($"[UserService] User {id} deleted successfully");
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
```

## Dependency Registration

Services must be registered in `Program.cs`:

```csharp
void SetupDependencyInjection()
{
    builder.Services.AddTransient<IEntityService, EntityService>();
}
```

## Best Practices

1. **Business logic only** - Services contain business rules, not data access
2. **Transform data** - Convert between repository strings and business models
3. **Log business operations** - Log key business decisions and flows
4. **Validate input** - Check business rules before calling repositories
5. **Use private methods** - Break complex logic into smaller helper methods
6. **Handle exceptions** - Catch, log, and throw or transform exceptions
7. **No HTTP calls** - Services call repositories, not HttpClient directly
8. **Return models** - Always return typed business models
9. **Orchestrate** - Coordinate multiple repository calls when needed
10. **Single Responsibility** - Each service handles one business domain

## Common Mistakes to Avoid

❌ **DO NOT** make HTTP calls directly
```csharp
public async Task<UserResponseModel> GetUser(int id)
{
    using var client = new HttpClient();  // Wrong! Use repository
    var response = await client.GetAsync($"http://api.com/users/{id}");
    return JsonSerializer.Deserialize<UserResponseModel>(await response.Content.ReadAsStringAsync());
}
```

✅ **DO** use repositories
```csharp
public async Task<UserResponseModel> GetUser(int id)
{
    string rawData = await _userRepository.GetUserByIdFromApi(id);
    return JsonSerializer.Deserialize<UserResponseModel>(rawData);
}
```

❌ **DO NOT** return raw strings
```csharp
public async Task<string> GetUser(int id)  // Wrong! Return model
{
    return await _userRepository.GetUserByIdFromApi(id);
}
```

✅ **DO** return typed models
```csharp
public async Task<UserResponseModel> GetUser(int id)
{
    string rawData = await _userRepository.GetUserByIdFromApi(id);
    return JsonSerializer.Deserialize<UserResponseModel>(rawData);
}
```

❌ **DO NOT** put all logic in one method
```csharp
public async Task<OrderResponseModel> CreateOrder(OrderRequestModel request)
{
    string userData = await _userRepository.GetUserByIdFromApi(request.userId);
    var user = JsonSerializer.Deserialize<UserResponseModel>(userData);
    var products = new List<ProductResponseModel>();
    foreach (var item in request.items) { /* complex logic */ }
    decimal total = 0;
    foreach (var item in request.items) { /* complex logic */ }
    var orderPayload = new { /* complex logic */ };
    string jsonPayload = JsonSerializer.Serialize(orderPayload);
    string rawResponse = await _orderRepository.CreateOrderInApi(jsonPayload);
    return JsonSerializer.Deserialize<OrderResponseModel>(rawResponse);
}
```

✅ **DO** break into helper methods
```csharp
public async Task<OrderResponseModel> CreateOrder(OrderRequestModel request)
{
    UserResponseModel user = await GetAndValidateUser(request.userId);
    List<ProductResponseModel> products = await FetchProducts(request.items);
    decimal total = CalculateTotalAmount(request.items, products);
    string jsonPayload = CreateOrderPayload(user, request.items, total);
    string rawResponse = await _orderRepository.CreateOrderInApi(jsonPayload);
    return ParseOrderData(rawResponse);
}

private async Task<UserResponseModel> GetAndValidateUser(int userId) { /* logic */ }
private async Task<List<ProductResponseModel>> FetchProducts(List<OrderItemRequest> items) { /* logic */ }
private decimal CalculateTotalAmount(List<OrderItemRequest> items, List<ProductResponseModel> products) { /* logic */ }
private string CreateOrderPayload(UserResponseModel user, List<OrderItemRequest> items, decimal total) { /* logic */ }
private OrderResponseModel ParseOrderData(string rawData) { /* logic */ }
```

## Checklist for Creating Services

- ✅ File named `{Entity}Service.cs`
- ✅ Class named `{Entity}Service`
- ✅ Implements `I{Entity}Service` interface
- ✅ Namespace is `dotnet_8_backend_template.services`
- ✅ Injects repository interfaces and ILogger
- ✅ All public methods are async
- ✅ Methods accept and return business models
- ✅ Raw data from repositories is parsed/transformed
- ✅ Business logic is implemented
- ✅ Logging at key business operation points
- ✅ Complex logic broken into private helper methods
- ✅ Registered in Program.cs with AddTransient

## Related Documentation

- See [../controllers/AGENTS.md](../controllers/AGENTS.md) for how controllers use services
- See [../repositories/AGENTS.md](../repositories/AGENTS.md) for how services use repositories
- See [../interfaces/AGENTS.md](../interfaces/AGENTS.md) for service interfaces
- See [../models/AGENTS.md](../models/AGENTS.md) for model definitions
- See [../tests/AGENTS.md](../tests/AGENTS.md) for service testing patterns
- See [../AGENTS.md](../AGENTS.md) for overall architecture

---

**Last Updated**: 2025-10-15
**Layer**: Services (Business Logic Layer)
**Pattern**: Controller → Service → Repository
