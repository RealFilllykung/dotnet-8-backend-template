# Prompt Example: Creating a New API Endpoint

This document demonstrates effective prompts for creating new API endpoints with GitHub Copilot or other AI agents in this .NET 8 backend template project.

## Context

This project follows the **Controller - Service - Repository** pattern. When creating new endpoints, AI agents (guided by [AGENTS.md](../AGENTS.md)) should automatically generate all required layers.

---

## Example 1: Simple GET Endpoint

### Prompt
```
Create me a new API endpoint for HTTP GET /agent.
This endpoint will return "hello world" back to the user.
```

### Expected Result

The AI agent should generate the following files:

#### 1. Model (`models/AgentResponseModel.cs`)
```csharp
namespace dotnet_8_backend_template.models;

public class AgentResponseModel
{
    public string message { get; set; }
}
```

#### 2. Repository Interface (`interfaces/repositories/IAgentRepository.cs`)
```csharp
namespace dotnet_8_backend_template.interfaces.repositories;

public interface IAgentRepository
{
    public Task<string> GetAgentMessage();
}
```

#### 3. Repository Implementation (`repositories/AgentRepository.cs`)
```csharp
using dotnet_8_backend_template.interfaces.repositories;

namespace dotnet_8_backend_template.repositories;

public class AgentRepository : IAgentRepository
{
    private readonly ILogger<AgentRepository> _logger;

    public AgentRepository(ILogger<AgentRepository> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetAgentMessage()
    {
        _logger.LogInformation("[AgentRepository] Fetching agent message");
        return await Task.FromResult("hello world");
    }
}
```

#### 4. Service Interface (`interfaces/services/IAgentService.cs`)
```csharp
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.interfaces.services;

public interface IAgentService
{
    public Task<AgentResponseModel> GetAgentMessage();
}
```

#### 5. Service Implementation (`services/AgentService.cs`)
```csharp
using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.services;

public class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly ILogger<AgentService> _logger;

    public AgentService(IAgentRepository agentRepository, ILogger<AgentService> logger)
    {
        _agentRepository = agentRepository;
        _logger = logger;
    }

    public async Task<AgentResponseModel> GetAgentMessage()
    {
        _logger.LogInformation("[AgentService] Processing agent message request");
        string message = await _agentRepository.GetAgentMessage();

        AgentResponseModel responseModel = new AgentResponseModel
        {
            message = message
        };

        return responseModel;
    }
}
```

#### 6. Controller (`controllers/AgentController.cs`)
```csharp
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_8_backend_template.controllers;

[ApiController]
[Route("agent")]
public class AgentController : ControllerBase
{
    private readonly ILogger<AgentController> _logger;
    private readonly IAgentService _agentService;

    public AgentController(ILogger<AgentController> logger, IAgentService agentService)
    {
        _logger = logger;
        _agentService = agentService;
    }

    [HttpGet]
    public async Task<AgentResponseModel> GetAgent()
    {
        return await _agentService.GetAgentMessage();
    }
}
```

#### 7. Dependency Injection Registration (in `Program.cs`)
```csharp
void SetupDependencyInjection()
{
    builder.Services.AddTransient<IIpService, IpService>();
    builder.Services.AddHttpClient<IIpRepository, IpRepository>(client =>
        client.BaseAddress = new Uri(builder.Configuration["IP_API_URL"]!));

    // NEW: Agent endpoint dependencies
    builder.Services.AddTransient<IAgentService, AgentService>();
    builder.Services.AddTransient<IAgentRepository, AgentRepository>();
}
```

### Testing the Endpoint

**Request:**
```http
GET https://localhost:5001/agent
```

**Response:**
```json
{
  "message": "hello world"
}
```

**Swagger UI:**
Navigate to `https://localhost:5001/swagger` and test the `/agent` endpoint.

---

## Example 2: POST Endpoint with Request Body

### Prompt
```
Create a new API endpoint for HTTP POST /user that accepts a username
and email in the request body, and returns a user ID along with the
submitted data.
```

### Expected Structure

The AI should generate:
- `models/UserRequestModel.cs` - Request DTO
- `models/UserResponseModel.cs` - Response DTO
- `interfaces/repositories/IUserRepository.cs`
- `repositories/UserRepository.cs`
- `interfaces/services/IUserService.cs`
- `services/UserService.cs`
- `controllers/UserController.cs`
- Update `Program.cs` with DI registration

### Sample Controller Code
```csharp
[HttpPost]
public async Task<UserResponseModel> CreateUser([FromBody] UserRequestModel request)
{
    return await _userService.CreateUser(request);
}
```

---

## Example 3: GET Endpoint with Path Parameter

### Prompt
```
Create an API endpoint for HTTP GET /product/{id} that retrieves
product details by ID. Return product name, price, and description.
```

### Expected Controller Code
```csharp
[HttpGet("{id}")]
public async Task<ProductResponseModel> GetProduct(int id)
{
    return await _productService.GetProductById(id);
}
```

---

## Example 4: External API Integration

### Prompt
```
Create an API endpoint for HTTP GET /weather that calls an external
weather API (https://api.weather.com) and returns the current temperature
and conditions.
```

### Expected Result

The AI should:
1. Create repository with `HttpClient` injection
2. Register repository with `AddHttpClient` in `Program.cs`:
```csharp
builder.Services.AddHttpClient<IWeatherRepository, WeatherRepository>(
    client => client.BaseAddress = new Uri(builder.Configuration["WEATHER_API_URL"]!)
);
```
3. Add `"WEATHER_API_URL": "https://api.weather.com"` to `appsettings.json`

---

## Prompt Best Practices

### Good Prompts
- **Be specific about HTTP method and route**: "Create HTTP GET /endpoint"
- **Describe the data flow**: "accepts X, returns Y"
- **Mention external dependencies**: "calls external API at..."
- **Keep it concise**: One clear sentence is often enough

### L Avoid
- Vague requests: "Create a new endpoint" (missing details)
- Skipping layers: "Just create a controller" (breaks pattern)
- Inconsistent naming: "Make an IP getter thing" (unclear entity name)

---

## Advanced Prompts

### With Query Parameters
```
Create HTTP GET /search?keyword={keyword}&limit={limit} that searches
products and returns matching results.
```

### With Authentication (Future)
```
Create HTTP POST /auth/login that accepts username and password,
validates credentials, and returns a JWT token.
```

### With Database Integration (Future)
```
Create HTTP GET /users that retrieves all users from the database
using Entity Framework Core.
```

---

## Verification Checklist

After AI generates code, verify:
- All 6 files created (Model, Interface�2, Implementation�2, Controller)
- Dependencies registered in `Program.cs`
- Naming follows conventions (`{Entity}Controller`, `I{Entity}Service`, etc.)
- Proper namespace usage
- Logging included in Service and Repository
- Async/await pattern used correctly
- Controller inherits from `ControllerBase` with proper attributes

---

## Tips for One-Shot Success

1. **Reference AGENTS.md**: Ensure AI has read the AGENTS.md file for context
2. **Use consistent entity names**: Pick clear, singular nouns (User, Product, Agent)
3. **Specify return format**: "returns JSON with..." or "returns a model containing..."
4. **Mention data source**: "from external API", "from database", or "generated locally"
5. **Keep prompts focused**: One endpoint per prompt for best results

---

## Troubleshooting

### AI only created Controller
**Fix**: Remind AI to follow the three-layer pattern:
```
Follow the Controller � Service � Repository pattern from AGENTS.md
```

### Missing DI Registration
**Fix**: Explicitly request:
```
Also update Program.cs with dependency injection registration
```

### Inconsistent naming
**Fix**: Specify the entity name:
```
Use "Agent" as the entity name for all files
```

---

**Last Updated**: 2025-10-15
**Related Documentation**: [AGENTS.md](../AGENTS.md)
