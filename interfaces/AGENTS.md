# Interfaces Layer - AI Agent Instructions

This guide provides specific instructions for creating and maintaining **Interfaces** in this .NET 8 Web API project.

## Interface Responsibilities

Interfaces define **contracts** for services and repositories. They:
- Define public methods that implementations must provide
- Enable dependency injection and loose coupling
- Allow for easy testing with mocks
- Document the public API of services and repositories
- Support the Dependency Inversion Principle

## Directory Structure

```
interfaces/
  services/
    I{Entity}Service.cs
  repositories/
    I{Entity}Repository.cs
```

## Service Interfaces

### Location
`interfaces/services/I{Entity}Service.cs`

### Basic Service Interface Template

```csharp
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.interfaces.services;

public interface IEntityService
{
    public Task<EntityResponseModel> GetEntity();
    public Task<EntityResponseModel> GetEntityById(int id);
    public Task<EntityResponseModel> CreateEntity(EntityRequestModel request);
    public Task<EntityResponseModel> UpdateEntity(int id, EntityRequestModel request);
    public Task DeleteEntity(int id);
}
```

### Naming Conventions

**File Name**: `I{Entity}Service.cs`
- Examples: `IIpService.cs`, `IUserService.cs`, `IProductService.cs`

**Interface Name**: `I{Entity}Service`
- Examples: `IIpService`, `IUserService`, `IProductService`

**Namespace**: `dotnet_8_backend_template.interfaces.services`

### Method Naming

Use descriptive, action-oriented names:
- `Get{Entity}` - Retrieve single or all entities
- `Get{Entity}ById` - Retrieve by identifier
- `Get{Entity}By{Criteria}` - Retrieve by specific criteria
- `Create{Entity}` - Create new entity
- `Update{Entity}` - Update existing entity
- `Delete{Entity}` - Remove entity
- `Process{Entity}` - Perform business operation
- `Calculate{Something}` - Perform calculation
- `Validate{Something}` - Perform validation

### Return Types

**For methods that return data**:
```csharp
public Task<EntityResponseModel> GetEntity();
public Task<List<EntityResponseModel>> GetAllEntities();
public Task<EntityResponseModel> CreateEntity(EntityRequestModel request);
```

**For methods that don't return data**:
```csharp
public Task DeleteEntity(int id);
public Task UpdateStatus(int id, string status);
```

**For methods that may not find data**:
```csharp
public Task<EntityResponseModel?> TryGetEntity(int id);
```

### Complete Service Interface Example

```csharp
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.interfaces.services;

public interface IUserService
{
    public Task<List<UserResponseModel>> GetAllUsers();
    public Task<UserResponseModel> GetUserById(int id);
    public Task<UserResponseModel> GetUserByUsername(string username);
    public Task<UserResponseModel> CreateUser(UserRequestModel request);
    public Task<UserResponseModel> UpdateUser(int id, UserRequestModel request);
    public Task DeleteUser(int id);
    public Task<bool> ValidateUserCredentials(string username, string password);
}
```

## Repository Interfaces

### Location
`interfaces/repositories/I{Entity}Repository.cs`

### Basic Repository Interface Template

```csharp
namespace dotnet_8_backend_template.interfaces.repositories;

public interface IEntityRepository
{
    public Task<string> GetEntityData();
    public Task<string> GetEntityDataById(int id);
    public Task<string> CreateEntity(string jsonData);
    public Task<string> UpdateEntity(int id, string jsonData);
    public Task DeleteEntity(int id);
}
```

### Naming Conventions

**File Name**: `I{Entity}Repository.cs`
- Examples: `IIpRepository.cs`, `IUserRepository.cs`, `IProductRepository.cs`

**Interface Name**: `I{Entity}Repository`
- Examples: `IIpRepository`, `IUserRepository`, `IProductRepository`

**Namespace**: `dotnet_8_backend_template.interfaces.repositories`

### Method Naming

Repository methods typically:
- `Get{Data}` - Fetch data from external source
- `Fetch{Data}` - Alternative to Get
- `Create{Entity}` - Send create request
- `Update{Entity}` - Send update request
- `Delete{Entity}` - Send delete request
- `Post{Data}` - HTTP POST operation
- `Put{Data}` - HTTP PUT operation

### Return Types

Repositories typically return:
- **Raw data** as `string` (JSON, XML, etc.)
- **HTTP responses** as `string`
- **Simple types** for status checks (`bool`, `int`)

```csharp
public Task<string> GetCurrentPublicIp();
public Task<string> FetchUserDataById(int id);
public Task<bool> CheckEntityExists(int id);
```

### Complete Repository Interface Example

```csharp
namespace dotnet_8_backend_template.interfaces.repositories;

public interface IUserRepository
{
    public Task<string> GetAllUsersFromApi();
    public Task<string> GetUserByIdFromApi(int id);
    public Task<string> GetUserByUsernameFromApi(string username);
    public Task<string> CreateUserInApi(string jsonPayload);
    public Task<string> UpdateUserInApi(int id, string jsonPayload);
    public Task DeleteUserInApi(int id);
    public Task<bool> CheckUserExistsInApi(int id);
}
```

## Method Signatures

### All Methods Are Public
```csharp
public Task<ReturnType> MethodName(parameters);
```

### Async Methods
All interface methods should be asynchronous:
```csharp
public Task<EntityResponseModel> GetEntity();  // Returns data
public Task UpdateEntity(int id);  // No return data
```

### Method Parameters

**Single parameter**:
```csharp
public Task<UserResponseModel> GetUserById(int id);
```

**Multiple parameters**:
```csharp
public Task<UserResponseModel> UpdateUser(int id, UserRequestModel request);
public Task<List<ProductResponseModel>> SearchProducts(string query, int page, int pageSize);
```

**Optional parameters** (avoid in interfaces, use overloads):
```csharp
public Task<List<EntityResponseModel>> GetEntities();
public Task<List<EntityResponseModel>> GetEntities(int pageSize);
public Task<List<EntityResponseModel>> GetEntities(int page, int pageSize);
```

## Interface Documentation

### XML Documentation Comments

Add XML documentation for clarity:

```csharp
namespace dotnet_8_backend_template.interfaces.services;

/// <summary>
/// Service for managing user operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves all users from the system
    /// </summary>
    /// <returns>List of user response models</returns>
    public Task<List<UserResponseModel>> GetAllUsers();

    /// <summary>
    /// Retrieves a specific user by their unique identifier
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <returns>User response model</returns>
    public Task<UserResponseModel> GetUserById(int id);

    /// <summary>
    /// Creates a new user in the system
    /// </summary>
    /// <param name="request">User creation request containing user details</param>
    /// <returns>Created user response model</returns>
    public Task<UserResponseModel> CreateUser(UserRequestModel request);
}
```

## Service vs Repository Interfaces

### Service Interfaces
- Work with **business models** (RequestModel, ResponseModel)
- Define **business operations** (CreateUser, ValidateCredentials)
- Return **typed models**
- Used by **controllers**

### Repository Interfaces
- Work with **raw data** (strings, primitives)
- Define **data access operations** (GetUserDataFromApi, PostToExternalApi)
- Return **raw strings or primitives**
- Used by **services**

## Complete Examples

### Service Interface Example

```csharp
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.interfaces.services;

public interface IProductService
{
    public Task<List<ProductResponseModel>> GetAllProducts();
    public Task<List<ProductResponseModel>> GetProductsByCategory(string category);
    public Task<ProductResponseModel> GetProductById(int id);
    public Task<ProductResponseModel> CreateProduct(ProductRequestModel request);
    public Task<ProductResponseModel> UpdateProduct(int id, ProductRequestModel request);
    public Task DeleteProduct(int id);
    public Task<decimal> CalculateProductPrice(int productId, int quantity);
    public Task<bool> IsProductAvailable(int productId);
}
```

### Repository Interface Example

```csharp
namespace dotnet_8_backend_template.interfaces.repositories;

public interface IProductRepository
{
    public Task<string> GetAllProductsFromApi();
    public Task<string> GetProductsByCategoryFromApi(string category);
    public Task<string> GetProductByIdFromApi(int id);
    public Task<string> CreateProductInApi(string jsonPayload);
    public Task<string> UpdateProductInApi(int id, string jsonPayload);
    public Task DeleteProductInApi(int id);
    public Task<bool> CheckProductExistsInApi(int id);
    public Task<string> GetProductPriceFromApi(int id);
}
```

## Best Practices

1. **One interface per service/repository** - Don't combine multiple concerns
2. **All methods are public** - Interfaces define public contracts
3. **All methods are async** - Use `Task` or `Task<T>`
4. **Descriptive method names** - Clear action verbs and specific entities
5. **Use models in services** - Services work with typed models
6. **Use primitives in repositories** - Repositories work with raw data
7. **No implementation details** - Interfaces only define contracts
8. **Keep interfaces focused** - Single Responsibility Principle
9. **Document with XML comments** - Especially for public APIs
10. **Version carefully** - Changing interfaces affects all implementations

## Common Mistakes to Avoid

❌ **DO NOT** include implementation details
```csharp
public interface IUserService
{
    public Task<UserResponseModel> GetUserById(int id)
    {
        // Wrong! No implementation in interfaces
        return await _repository.GetUser(id);
    }
}
```

✅ **DO** define method signatures only
```csharp
public interface IUserService
{
    public Task<UserResponseModel> GetUserById(int id);
}
```

❌ **DO NOT** use concrete types when models exist
```csharp
public interface IUserRepository
{
    public Task<UserResponseModel> GetUser(int id); // Wrong! Repository should return raw data
}
```

✅ **DO** return raw data from repositories
```csharp
public interface IUserRepository
{
    public Task<string> GetUserDataFromApi(int id); // Correct!
}
```

❌ **DO NOT** use synchronous methods
```csharp
public interface IUserService
{
    public UserResponseModel GetUser(int id); // Missing async!
}
```

✅ **DO** use async methods
```csharp
public interface IUserService
{
    public Task<UserResponseModel> GetUser(int id);
}
```

## Checklist for Creating Interfaces

When creating a new interface:

### Service Interface Checklist
- ✅ File named `I{Entity}Service.cs`
- ✅ Interface named `I{Entity}Service`
- ✅ Namespace is `dotnet_8_backend_template.interfaces.services`
- ✅ All methods are `public`
- ✅ All methods return `Task` or `Task<T>`
- ✅ Methods accept and return business models
- ✅ Method names are descriptive and action-oriented
- ✅ XML documentation added for public methods

### Repository Interface Checklist
- ✅ File named `I{Entity}Repository.cs`
- ✅ Interface named `I{Entity}Repository`
- ✅ Namespace is `dotnet_8_backend_template.interfaces.repositories`
- ✅ All methods are `public`
- ✅ All methods return `Task` or `Task<T>`
- ✅ Methods return raw data types (string, bool, int)
- ✅ Method names clearly indicate data source
- ✅ XML documentation added for clarity

## Related Documentation

- See [../services/AGENTS.md](../services/AGENTS.md) for service implementations
- See [../repositories/AGENTS.md](../repositories/AGENTS.md) for repository implementations
- See [../models/AGENTS.md](../models/AGENTS.md) for model definitions
- See [../AGENTS.md](../AGENTS.md) for overall architecture

---

**Last Updated**: 2025-10-15
**Layer**: Interfaces (Contracts)
**Pattern**: Dependency Inversion Principle
