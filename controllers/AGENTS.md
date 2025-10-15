# Controllers Layer - AI Agent Instructions

This guide provides specific instructions for creating and maintaining **Controllers** in this .NET 8 Web API project.

## Controller Responsibilities

Controllers are the **HTTP layer** of the application. They:
- Handle incoming HTTP requests
- Route requests to appropriate service methods
- Return HTTP responses with proper status codes
- Perform minimal logic (delegate to services)
- Handle request validation at the HTTP level

## Controller Structure

### Basic Controller Template

```csharp
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_8_backend_template.controllers;

[ApiController]
[Route("entity-name")]
public class EntityController : ControllerBase
{
    private readonly ILogger<EntityController> _logger;
    private readonly IEntityService _entityService;

    public EntityController(ILogger<EntityController> logger, IEntityService entityService)
    {
        _logger = logger;
        _entityService = entityService;
    }

    [HttpGet]
    public async Task<EntityResponseModel> GetEntity()
    {
        return await _entityService.GetEntity();
    }

    [HttpGet("{id}")]
    public async Task<EntityResponseModel> GetEntityById(int id)
    {
        return await _entityService.GetEntityById(id);
    }

    [HttpPost]
    public async Task<EntityResponseModel> CreateEntity([FromBody] EntityRequestModel request)
    {
        return await _entityService.CreateEntity(request);
    }

    [HttpPut("{id}")]
    public async Task<EntityResponseModel> UpdateEntity(int id, [FromBody] EntityRequestModel request)
    {
        return await _entityService.UpdateEntity(id, request);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntity(int id)
    {
        await _entityService.DeleteEntity(id);
        return NoContent();
    }
}
```

## Naming Conventions

### File Naming
- **File name**: `{Entity}Controller.cs`
- **Examples**: `IpController.cs`, `UserController.cs`, `ProductController.cs`

### Class Naming
- **Class name**: `{Entity}Controller`
- **Inherits from**: `ControllerBase` (not `Controller`)
- **Namespace**: `dotnet_8_backend_template.controllers`

### Route Naming
- **Format**: lowercase with hyphens for multi-word entities
- **Examples**:
  - `[Route("ip")]` for IpController
  - `[Route("user")]` for UserController
  - `[Route("product-category")]` for ProductCategoryController

### Method Naming
- **Format**: PascalCase with descriptive verb
- **Examples**:
  - `GetEntity()` - Get all or single entity
  - `GetEntityById(int id)` - Get entity by ID
  - `CreateEntity(EntityRequestModel request)` - Create new entity
  - `UpdateEntity(int id, EntityRequestModel request)` - Update existing entity
  - `DeleteEntity(int id)` - Delete entity

## Required Attributes

### Class Level
```csharp
[ApiController]  // Required - enables automatic model validation and binding
[Route("route-name")]  // Required - defines base route
```

### Method Level
```csharp
[HttpGet]  // For retrieving data
[HttpGet("{id}")]  // For retrieving by ID
[HttpPost]  // For creating new resources
[HttpPut("{id}")]  // For updating existing resources
[HttpDelete("{id}")]  // For deleting resources
[HttpPatch("{id}")]  // For partial updates
```

## Dependency Injection

Controllers should inject:
1. **ILogger<ControllerName>** - Always inject for logging
2. **Service Interfaces** - One or more service interfaces (IEntityService)

**DO NOT inject**:
- Repository interfaces (use services instead)
- HttpClient directly
- Database contexts directly

### Injection Pattern
```csharp
private readonly ILogger<EntityController> _logger;
private readonly IEntityService _entityService;

public EntityController(ILogger<EntityController> logger, IEntityService entityService)
{
    _logger = logger;
    _entityService = entityService;
}
```

## HTTP Method Patterns

### GET - Retrieve Data
```csharp
[HttpGet]
public async Task<EntityResponseModel> GetAll()
{
    return await _entityService.GetAll();
}

[HttpGet("{id}")]
public async Task<EntityResponseModel> GetById(int id)
{
    return await _entityService.GetById(id);
}
```

### POST - Create Resource
```csharp
[HttpPost]
public async Task<EntityResponseModel> Create([FromBody] EntityRequestModel request)
{
    return await _entityService.Create(request);
}
```

### PUT - Update Resource
```csharp
[HttpPut("{id}")]
public async Task<EntityResponseModel> Update(int id, [FromBody] EntityRequestModel request)
{
    return await _entityService.Update(id, request);
}
```

### DELETE - Remove Resource
```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    await _entityService.Delete(id);
    return NoContent();
}
```

## Parameter Binding

### Route Parameters
```csharp
[HttpGet("{id}")]
public async Task<EntityResponseModel> GetById(int id)
{
    // id comes from route: /entity/123
}

[HttpGet("{category}/{id}")]
public async Task<EntityResponseModel> GetByCategoryAndId(string category, int id)
{
    // Matches: /entity/electronics/123
}
```

### Query Parameters
```csharp
[HttpGet]
public async Task<List<EntityResponseModel>> Search(string query, int page = 1, int pageSize = 10)
{
    // Matches: /entity?query=test&page=1&pageSize=10
    return await _entityService.Search(query, page, pageSize);
}
```

### Request Body
```csharp
[HttpPost]
public async Task<EntityResponseModel> Create([FromBody] EntityRequestModel request)
{
    // request comes from HTTP body as JSON
    return await _entityService.Create(request);
}
```

## Return Types

### Return Model Directly
```csharp
[HttpGet]
public async Task<EntityResponseModel> Get()
{
    return await _entityService.Get();
}
```

### Return IActionResult for Status Codes
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var entity = await _entityService.GetById(id);
    if (entity == null)
    {
        return NotFound();
    }
    return Ok(entity);
}

[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    await _entityService.Delete(id);
    return NoContent();
}
```

### Common Action Results
- `Ok(data)` - 200 OK with data
- `Created(uri, data)` - 201 Created
- `NoContent()` - 204 No Content
- `BadRequest()` or `BadRequest(error)` - 400 Bad Request
- `NotFound()` - 404 Not Found
- `Unauthorized()` - 401 Unauthorized
- `Forbid()` - 403 Forbidden

## Error Handling

### Let Exceptions Propagate
Controllers should generally let exceptions bubble up to be handled by middleware:

```csharp
[HttpGet("{id}")]
public async Task<EntityResponseModel> GetById(int id)
{
    return await _entityService.GetById(id);
}
```

### Handle Specific Business Logic
For specific business logic validation:

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] EntityRequestModel request)
{
    if (request.Value < 0)
    {
        return BadRequest("Value cannot be negative");
    }

    var result = await _entityService.Create(request);
    return CreatedAtAction(nameof(GetById), new { id = result.id }, result);
}
```

## Logging in Controllers

### Minimal Logging
Controllers should have minimal logging:

```csharp
[HttpGet("{id}")]
public async Task<EntityResponseModel> GetById(int id)
{
    _logger.LogInformation($"[EntityController] Fetching entity with ID: {id}");
    return await _entityService.GetById(id);
}
```

Most logging should happen in the service layer.

## Complete Example

```csharp
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

    [HttpGet]
    public async Task<List<UserResponseModel>> GetAllUsers()
    {
        _logger.LogInformation("[UserController] Fetching all users");
        return await _userService.GetAllUsers();
    }

    [HttpGet("{id}")]
    public async Task<UserResponseModel> GetUserById(int id)
    {
        _logger.LogInformation($"[UserController] Fetching user with ID: {id}");
        return await _userService.GetUserById(id);
    }

    [HttpPost]
    public async Task<UserResponseModel> CreateUser([FromBody] UserRequestModel request)
    {
        _logger.LogInformation($"[UserController] Creating user: {request.username}");
        return await _userService.CreateUser(request);
    }

    [HttpPut("{id}")]
    public async Task<UserResponseModel> UpdateUser(int id, [FromBody] UserRequestModel request)
    {
        _logger.LogInformation($"[UserController] Updating user with ID: {id}");
        return await _userService.UpdateUser(id, request);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        _logger.LogInformation($"[UserController] Deleting user with ID: {id}");
        await _userService.DeleteUser(id);
        return NoContent();
    }
}
```

## Best Practices

1. **Keep controllers thin** - Delegate all business logic to services
2. **One service per controller** - Typically inject only one main service interface
3. **Async all the way** - Always use async/await for I/O operations
4. **Use meaningful HTTP verbs** - GET for reads, POST for creates, PUT for updates, DELETE for deletes
5. **Return appropriate status codes** - Use IActionResult when you need control over status codes
6. **Validate at API boundary** - Use data annotations on request models
7. **Log minimally** - Log request entry points, let services handle detailed logging
8. **No business logic** - Controllers orchestrate, services execute
9. **Use route parameters for IDs** - `[HttpGet("{id}")]` not query strings
10. **Use [FromBody] explicitly** - Makes intent clear for POST/PUT requests

## Common Mistakes to Avoid

❌ **DO NOT** inject repositories directly
```csharp
public UserController(IUserRepository userRepository) // Wrong!
```

✅ **DO** inject service interfaces
```csharp
public UserController(IUserService userService) // Correct!
```

❌ **DO NOT** perform business logic in controllers
```csharp
[HttpPost]
public async Task<UserResponseModel> Create([FromBody] UserRequestModel request)
{
    // Wrong! Business logic in controller
    var hashedPassword = HashPassword(request.password);
    var user = new User { Password = hashedPassword };
    return await _userService.CreateUser(user);
}
```

✅ **DO** delegate to services
```csharp
[HttpPost]
public async Task<UserResponseModel> Create([FromBody] UserRequestModel request)
{
    return await _userService.CreateUser(request);
}
```

❌ **DO NOT** use synchronous methods
```csharp
[HttpGet]
public UserResponseModel Get()
{
    return _userService.GetUser().Result; // Blocks thread!
}
```

✅ **DO** use async/await
```csharp
[HttpGet]
public async Task<UserResponseModel> Get()
{
    return await _userService.GetUser();
}
```

## Related Documentation

- See [../services/AGENTS.md](../services/AGENTS.md) for service layer patterns
- See [../models/AGENTS.md](../models/AGENTS.md) for request/response models
- See [../tests/AGENTS.md](../tests/AGENTS.md) for controller testing patterns
- See [../AGENTS.md](../AGENTS.md) for overall architecture

---

**Last Updated**: 2025-10-15
**Layer**: Controllers (HTTP Layer)
**Pattern**: Controller → Service → Repository
