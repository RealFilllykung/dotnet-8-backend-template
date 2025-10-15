# Testing Layer - AI Agent Instructions

This guide provides specific instructions for creating and maintaining **Unit Tests** in this .NET 8 Web API project.

## Testing Framework

This project uses:
- **xUnit** - Testing framework
- **Moq** - Mocking library for dependencies
- **Microsoft.AspNetCore.TestHost** - In-memory HTTP testing for controllers

### Required NuGet Packages

```xml
<PackageReference Include="xunit" Version="2.9.2"/>
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2"/>
<PackageReference Include="Moq" Version="4.20.72"/>
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1"/>
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.11"/>
```

## Testing Directory Structure

```
tests/
  controllers/
    {Entity}ControllerTests.cs
  services/
    {Entity}ServiceTests.cs
  repositories/
    {Entity}RepositoryTests.cs
```

## Test Naming Conventions

### Test Class Names
**Format**: `{ClassName}Tests`
**Examples**:
- `IpServiceTests`
- `UserServiceTests`
- `IpControllerTests`
- `ProductRepositoryTests`

### Test Method Names
**Format**: `GivenCondition_WhenAction_ThenExpectedOutcome`

This is the **Given-When-Then** pattern:
- **Given** - The initial state or precondition
- **When** - The action being tested
- **Then** - The expected outcome

**Examples**:
- `GivenValidIpResponse_WhenGetCurrentMachinePublicIp_ThenReturnsCorrectIpAddress`
- `GivenServiceThrowsException_WhenGetCurrentMachinePublicIp_ThenThrowsException`
- `GivenEmptyResponse_WhenGetCurrentPublicIp_ThenThrowsIndexOutOfRangeException`
- `GivenUserDoesNotExist_WhenGetUserById_ThenThrowsKeyNotFoundException`
- `GivenValidRequest_WhenCreateUser_ThenReturnsCreatedUser`

## Writing Readable Tests Without Comments

**CRITICAL RULE**: This project does NOT use comments in any code, including tests.

### How to Write Self-Explanatory Tests

1. **Use descriptive variable names**
2. **Avoid ALL abbreviations**
3. **Use clear method names with Given-When-Then pattern**
4. **Structure code logically with blank lines as separators**
5. **Use regions to organize test categories**

### Abbreviations to AVOID

❌ **DO NOT** use:
- `repo` → Use `repository`
- `svc` → Use `service`
- `req` → Use `request`
- `res` → Use `response`
- `exp` → Use `expected`
- `act` → Use `actual`
- `msg` → Use `message`
- `ex` → Use `exception`
- `cfg` → Use `configuration`

✅ **DO** use full words:
```csharp
string expectedIpAddress = "192.168.1.1";
var mockUserRepository = new Mock<IUserRepository>();
UserResponseModel expectedResponse = new UserResponseModel();
```

## Service Layer Testing

### Purpose
Test business logic by mocking repository dependencies. Services should not make actual HTTP calls or database connections in tests.

### File Location
`tests/services/{Entity}ServiceTests.cs`

### Basic Service Test Template

```csharp
using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.models;
using dotnet_8_backend_template.services;
using Moq;
using Xunit;

namespace dotnet_8_backend_template.tests.services;

public class EntityServiceTests
{
    private readonly Mock<IEntityRepository> _mockEntityRepository;
    private readonly Mock<ILogger<EntityService>> _mockLogger;
    private readonly EntityService _entityService;

    public EntityServiceTests()
    {
        _mockEntityRepository = new Mock<IEntityRepository>();
        _mockLogger = new Mock<ILogger<EntityService>>();
        _entityService = new EntityService(_mockEntityRepository.Object, _mockLogger.Object);
    }

    #region Positive Test Cases

    [Fact]
    public async Task GivenValidInput_WhenGetEntity_ThenReturnsEntity()
    {
        string expectedData = "test data";
        _mockEntityRepository
            .Setup(repository => repository.GetEntityData())
            .ReturnsAsync(expectedData);

        EntityResponseModel result = await _entityService.GetEntity();

        Assert.NotNull(result);
        _mockEntityRepository.Verify(repository => repository.GetEntityData(), Times.Once);
    }

    #endregion

    #region Negative Test Cases

    [Fact]
    public async Task GivenRepositoryThrowsException_WhenGetEntity_ThenThrowsException()
    {
        var expectedException = new HttpRequestException("Network error");
        _mockEntityRepository
            .Setup(repository => repository.GetEntityData())
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            async () => await _entityService.GetEntity()
        );

        Assert.Equal("Network error", exception.Message);
        _mockEntityRepository.Verify(repository => repository.GetEntityData(), Times.Once);
    }

    #endregion
}
```

### Complete Service Test Example

```csharp
using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.models;
using dotnet_8_backend_template.services;
using Moq;
using Xunit;

namespace dotnet_8_backend_template.tests.services;

public class IpServiceTests
{
    private readonly Mock<IIpRepository> _mockIpRepository;
    private readonly Mock<ILogger<IpService>> _mockLogger;
    private readonly IpService _ipService;

    public IpServiceTests()
    {
        _mockIpRepository = new Mock<IIpRepository>();
        _mockLogger = new Mock<ILogger<IpService>>();
        _ipService = new IpService(_mockIpRepository.Object, _mockLogger.Object);
    }

    #region Positive Test Cases

    [Fact]
    public async Task GivenValidIpResponse_WhenGetCurrentMachinePublicIp_ThenReturnsCorrectIpAddress()
    {
        string expectedIp = "1.2.3.4";
        string mockRepositoryResponse = $"{{\"ip\": {expectedIp}}}";

        _mockIpRepository
            .Setup(repository => repository.GetCurrentPublicIp())
            .ReturnsAsync(mockRepositoryResponse);

        IpResponseModel result = await _ipService.GetCurrentMachinePublicIp();

        Assert.NotNull(result);
        Assert.Equal(expectedIp, result.ip);

        _mockIpRepository.Verify(repository => repository.GetCurrentPublicIp(), Times.Once);
    }

    #endregion

    #region Negative Test Cases

    [Fact]
    public async Task GivenRepositoryThrowsException_WhenGetCurrentMachinePublicIp_ThenThrowsException()
    {
        var expectedException = new HttpRequestException("Unable to connect to the remote server");

        _mockIpRepository
            .Setup(repository => repository.GetCurrentPublicIp())
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            async () => await _ipService.GetCurrentMachinePublicIp()
        );

        Assert.Equal("Unable to connect to the remote server", exception.Message);

        _mockIpRepository.Verify(repository => repository.GetCurrentPublicIp(), Times.Once);
    }

    #endregion
}
```

### Key Service Testing Principles

1. **Mock repository interfaces** using `Mock<IRepository>`
2. **Mock logger** using `Mock<ILogger<ServiceClass>>`
3. **Create service instance** with mocked dependencies in constructor
4. **Use `.Setup()`** to configure mock behavior
5. **Use `.ReturnsAsync()`** for async mock returns
6. **Use `.ThrowsAsync()`** to simulate exceptions
7. **Use `.Verify()`** to ensure methods were called correctly
8. **Use verification times**: `Times.Once`, `Times.Never`, `Times.Exactly(n)`

## Controller Layer Testing (Integration Tests)

### Purpose
Test HTTP endpoints by making actual HTTP requests using `TestServer`. This tests the full HTTP request pipeline without running the actual application.

### File Location
`tests/controllers/{Entity}ControllerTests.cs`

### Required Packages

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http.Json;
```

### Basic Controller Test Template

```csharp
using System.Net;
using System.Net.Http.Json;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace dotnet_8_backend_template.tests.controllers;

public class EntityControllerTests : IDisposable
{
    private TestServer? _testServer;
    private HttpClient? _httpClient;

    public void Dispose()
    {
        _httpClient?.Dispose();
        _testServer?.Dispose();
    }

    #region Positive Test Cases

    [Fact]
    public async Task GivenValidRequest_WhenGetEntity_ThenReturnsOkWithEntity()
    {
        var expectedEntity = new EntityResponseModel { id = 1, name = "Test" };
        var mockEntityService = new Mock<IEntityService>();
        mockEntityService
            .Setup(service => service.GetEntity())
            .ReturnsAsync(expectedEntity);

        _testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddTransient(_ => mockEntityService.Object);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }));

        _httpClient = _testServer.CreateClient();

        var response = await _httpClient.GetAsync("/entity");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<EntityResponseModel>();
        Assert.NotNull(result);
        Assert.Equal(expectedEntity.id, result.id);

        mockEntityService.Verify(service => service.GetEntity(), Times.Once);
    }

    #endregion
}
```

### Complete Controller Test Example

```csharp
using System.Net;
using System.Net.Http.Json;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace dotnet_8_backend_template.tests.controllers;

public class IpControllerTests : IDisposable
{
    private TestServer? _testServer;
    private HttpClient? _httpClient;

    public void Dispose()
    {
        _httpClient?.Dispose();
        _testServer?.Dispose();
    }

    #region Positive Test Cases

    [Fact]
    public async Task GivenValidRequest_WhenGetCurrentMachinePublicIp_ThenReturnsOkWithIpAddress()
    {
        string expectedIp = "203.0.113.42";
        var mockIpService = new Mock<IIpService>();
        mockIpService
            .Setup(service => service.GetCurrentMachinePublicIp())
            .ReturnsAsync(new IpResponseModel { ip = expectedIp });

        _testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddTransient(_ => mockIpService.Object);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }));

        _httpClient = _testServer.CreateClient();

        var response = await _httpClient.GetAsync("/ip");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IpResponseModel>();
        Assert.NotNull(result);
        Assert.Equal(expectedIp, result.ip);

        mockIpService.Verify(service => service.GetCurrentMachinePublicIp(), Times.Once);
    }

    #endregion

    #region Negative Test Cases

    [Fact]
    public async Task GivenServiceThrowsException_WhenGetCurrentMachinePublicIp_ThenThrowsException()
    {
        var mockIpService = new Mock<IIpService>();
        mockIpService
            .Setup(service => service.GetCurrentMachinePublicIp())
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        _testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddTransient(_ => mockIpService.Object);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }));

        _httpClient = _testServer.CreateClient();

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            async () => await _httpClient.GetAsync("/ip")
        );

        Assert.Equal("Service unavailable", exception.Message);

        mockIpService.Verify(service => service.GetCurrentMachinePublicIp(), Times.Once);
    }

    #endregion
}
```

### Key Controller Testing Principles

1. **Use `TestServer`** with `WebHostBuilder` for in-memory HTTP testing
2. **Implement `IDisposable`** to properly clean up resources
3. **Mock service layer only** (not repositories)
4. **Make actual HTTP calls** using `HttpClient.GetAsync()`, `PostAsync()`, etc.
5. **Deserialize responses** using `ReadFromJsonAsync<T>()`
6. **Test HTTP status codes** with `Assert.Equal(HttpStatusCode.OK, response.StatusCode)`
7. **Use `EnsureSuccessStatusCode()`** for 2xx status code validation
8. **Configure minimal middleware** (UseRouting, MapControllers)

## Test Organization with Regions

Use `#region` directives to group tests by category:

```csharp
#region Positive Test Cases

[Fact]
public async Task GivenValidInput_WhenMethodCalled_ThenReturnsExpectedResult()
{
}

#endregion

#region Negative Test Cases

[Fact]
public async Task GivenInvalidInput_WhenMethodCalled_ThenThrowsException()
{
}

#endregion

#region Edge Cases

[Fact]
public async Task GivenEdgeCondition_WhenMethodCalled_ThenHandlesCorrectly()
{
}

#endregion
```

## Common xUnit Assertions

### Equality Assertions
```csharp
Assert.Equal(expected, actual);
Assert.NotEqual(unexpected, actual);
```

### Boolean Assertions
```csharp
Assert.True(condition);
Assert.False(condition);
```

### Null Assertions
```csharp
Assert.Null(object);
Assert.NotNull(object);
```

### Collection Assertions
```csharp
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Contains(item, collection);
Assert.DoesNotContain(item, collection);
```

### String Assertions
```csharp
Assert.Contains("substring", actualString);
Assert.StartsWith("prefix", actualString);
Assert.EndsWith("suffix", actualString);
```

### Exception Assertions
```csharp
Assert.Throws<ExceptionType>(() => methodCall());
Assert.ThrowsAsync<ExceptionType>(async () => await asyncMethodCall());
```

## Moq Setup and Verification

### Setup Mock Returns
```csharp
mockObject.Setup(method => method.MethodName()).Returns(value);
mockObject.Setup(method => method.MethodName()).ReturnsAsync(value);
```

### Setup Mock Exceptions
```csharp
mockObject.Setup(method => method.MethodName()).Throws<ExceptionType>();
mockObject.Setup(method => method.MethodName()).ThrowsAsync(new ExceptionType("message"));
```

### Verify Method Calls
```csharp
mockObject.Verify(method => method.MethodName(), Times.Once);
mockObject.Verify(method => method.MethodName(), Times.Never);
mockObject.Verify(method => method.MethodName(), Times.Exactly(3));
mockObject.Verify(method => method.MethodName(), Times.AtLeastOnce);
mockObject.Verify(method => method.MethodName(), Times.AtMost(5));
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~IpServiceTests"
dotnet test --filter "FullyQualifiedName~UserControllerTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "FullyQualifiedName~GivenValidIpResponse_WhenGetCurrentMachinePublicIp_ThenReturnsCorrectIpAddress"
```

### Run with Verbose Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Tests in Specific Folder
```bash
dotnet test --filter "FullyQualifiedName~controllers"
dotnet test --filter "FullyQualifiedName~services"
```

## Test Coverage Guidelines

### Service Layer Tests Should Cover
- ✅ All public methods
- ✅ Positive scenarios (valid inputs, successful operations)
- ✅ Negative scenarios (exceptions, invalid inputs)
- ✅ Edge cases (empty strings, null values, boundary conditions)
- ✅ Repository interactions are verified
- ✅ Data transformation logic
- ✅ Business rule validation

### Controller Layer Tests Should Cover
- ✅ Successful HTTP requests (200 OK)
- ✅ Error scenarios (exception propagation)
- ✅ Different HTTP methods (GET, POST, PUT, DELETE)
- ✅ Correct HTTP status codes
- ✅ Response body deserialization
- ✅ Service method calls are verified

## Best Practices for Unit Testing

1. **One assertion concept per test** - Test one behavior per test method
2. **Isolation** - Each test should be independent and not rely on other tests
3. **Fast execution** - Tests should run quickly (no actual HTTP calls, no database)
4. **Deterministic** - Tests should produce same results every time
5. **Descriptive names** - Use Given-When-Then naming pattern
6. **No comments** - Write self-explanatory code with descriptive names
7. **Avoid abbreviations** - Use full words for clarity
8. **Clean up resources** - Implement IDisposable when needed
9. **Mock external dependencies** - Never make real HTTP calls or database queries
10. **Verify mock interactions** - Always verify that mocked methods were called

## Test Checklist for New Features

When adding a new feature, create tests for:

### Service Layer Tests
- ✅ Valid input returns expected output
- ✅ Repository throws exception, service propagates it
- ✅ Empty response from repository
- ✅ Malformed data from repository
- ✅ Null reference scenarios
- ✅ Timeout scenarios (TaskCanceledException)
- ✅ Business logic validation
- ✅ Data transformation correctness

### Controller Layer Tests
- ✅ Valid HTTP GET request returns 200 OK with correct data
- ✅ Service throws exception, controller propagates it
- ✅ HTTP POST request with valid body returns 201 Created
- ✅ HTTP PUT request updates resource correctly
- ✅ HTTP DELETE request returns 204 No Content
- ✅ HTTP status codes are correct
- ✅ Response serialization works correctly

## Common Mistakes to Avoid

❌ **DO NOT** use comments
```csharp
[Fact]
public async Task Test()
{
    // Arrange
    var mock = new Mock<IService>();  // Wrong!

    // Act
    var result = await method();  // Wrong!

    // Assert
    Assert.NotNull(result);  // Wrong!
}
```

✅ **DO** use descriptive names and blank lines
```csharp
[Fact]
public async Task GivenValidInput_WhenMethodCalled_ThenReturnsResult()
{
    var mockService = new Mock<IService>();
    mockService.Setup(service => service.GetData()).ReturnsAsync("data");

    var result = await method();

    Assert.NotNull(result);
}
```

❌ **DO NOT** use abbreviations
```csharp
var exp = "expected";  // Wrong!
var act = await GetData();  // Wrong!
var repo = new Mock<IRepository>();  // Wrong!
```

✅ **DO** use full words
```csharp
string expected = "expected";
string actual = await GetData();
var mockRepository = new Mock<IRepository>();
```

❌ **DO NOT** test multiple concerns in one test
```csharp
[Fact]
public async Task TestEverything()
{
    // Testing create, update, and delete in one test - Wrong!
}
```

✅ **DO** separate into focused tests
```csharp
[Fact]
public async Task GivenValidRequest_WhenCreateUser_ThenReturnsCreatedUser() { }

[Fact]
public async Task GivenValidRequest_WhenUpdateUser_ThenReturnsUpdatedUser() { }

[Fact]
public async Task GivenValidUserId_WhenDeleteUser_ThenUserIsDeleted() { }
```

## Checklist for Creating Tests

### Service Test Checklist
- ✅ File named `{Entity}ServiceTests.cs`
- ✅ Class named `{Entity}ServiceTests`
- ✅ Namespace is `dotnet_8_backend_template.tests.services`
- ✅ Mock repository interfaces in constructor
- ✅ Mock logger in constructor
- ✅ Test method names use Given-When-Then pattern
- ✅ No comments in test code
- ✅ No abbreviations in variable names
- ✅ Tests organized with regions
- ✅ All public methods tested
- ✅ Positive and negative scenarios covered
- ✅ Mock interactions verified with Times

### Controller Test Checklist
- ✅ File named `{Entity}ControllerTests.cs`
- ✅ Class named `{Entity}ControllerTests`
- ✅ Namespace is `dotnet_8_backend_template.tests.controllers`
- ✅ Implements IDisposable
- ✅ Uses TestServer and WebHostBuilder
- ✅ Mocks service layer only
- ✅ Makes actual HTTP calls
- ✅ Tests HTTP status codes
- ✅ Tests response deserialization
- ✅ No comments in test code
- ✅ No abbreviations in variable names

## Related Documentation

- See [../services/AGENTS.md](../services/AGENTS.md) for service implementation patterns
- See [../controllers/AGENTS.md](../controllers/AGENTS.md) for controller implementation patterns
- See [../repositories/AGENTS.md](../repositories/AGENTS.md) for repository implementation patterns
- See [../models/AGENTS.md](../models/AGENTS.md) for model definitions
- See [../AGENTS.md](../AGENTS.md) for overall architecture

---

**Last Updated**: 2025-10-15
**Testing Framework**: xUnit with Moq
**Pattern**: Unit Tests for Services, Integration Tests for Controllers
