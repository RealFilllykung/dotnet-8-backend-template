# Models Layer - AI Agent Instructions

This guide provides specific instructions for creating and maintaining **Models** in this .NET 8 Web API project.

## Model Responsibilities

Models are **data transfer objects (DTOs)** that:
- Define the structure of data passed between layers
- Represent request payloads from clients
- Represent response payloads to clients
- Provide type safety for data structures
- Enable automatic serialization/deserialization
- Support data validation through attributes

## Model Types

### Response Models
Used to return data to clients from API endpoints.

**File Pattern**: `{Entity}ResponseModel.cs`

```csharp
namespace dotnet_8_backend_template.models;

public class UserResponseModel
{
    public int id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public DateTime createdAt { get; set; }
}
```

### Request Models
Used to receive data from clients in API endpoints.

**File Pattern**: `{Entity}RequestModel.cs`

```csharp
namespace dotnet_8_backend_template.models;

public class UserRequestModel
{
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}
```

### General Models
Used for other data structures not tied to requests/responses.

**File Pattern**: `{Entity}Model.cs` or `{DescriptiveName}Model.cs`

```csharp
namespace dotnet_8_backend_template.models;

public class UserSettingsModel
{
    public bool emailNotifications { get; set; }
    public string timezone { get; set; }
    public string language { get; set; }
}
```

## Naming Conventions

### File Naming
- **Response Model**: `{Entity}ResponseModel.cs`
  - Examples: `UserResponseModel.cs`, `ProductResponseModel.cs`, `IpResponseModel.cs`
- **Request Model**: `{Entity}RequestModel.cs`
  - Examples: `UserRequestModel.cs`, `ProductRequestModel.cs`
- **General Model**: `{Entity}Model.cs`
  - Examples: `SettingsModel.cs`, `ConfigurationModel.cs`

### Class Naming
- **Class name**: PascalCase
- **Suffix**: Always end with `Model`
- **Examples**: `UserResponseModel`, `ProductRequestModel`, `OrderModel`

### Property Naming
- **Format**: camelCase (lowercase first letter)
- **Examples**: `id`, `username`, `firstName`, `createdAt`, `isActive`

### Namespace
All models use: `namespace dotnet_8_backend_template.models;`

## Property Definitions

### Basic Properties
```csharp
public class UserResponseModel
{
    public int id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
}
```

### Property Types

**Primitive Types**:
```csharp
public int id { get; set; }
public string name { get; set; }
public bool isActive { get; set; }
public decimal price { get; set; }
public double rating { get; set; }
public DateTime createdAt { get; set; }
```

**Nullable Types**:
```csharp
public int? optionalId { get; set; }
public string? optionalDescription { get; set; }
public DateTime? lastLoginAt { get; set; }
```

**Collections**:
```csharp
public List<string> tags { get; set; }
public string[] categories { get; set; }
public Dictionary<string, string> metadata { get; set; }
```

**Nested Models**:
```csharp
public class OrderResponseModel
{
    public int id { get; set; }
    public UserResponseModel user { get; set; }
    public List<OrderItemModel> items { get; set; }
}
```

## Data Validation Attributes

### Required Fields
```csharp
using System.ComponentModel.DataAnnotations;

namespace dotnet_8_backend_template.models;

public class UserRequestModel
{
    [Required(ErrorMessage = "Username is required")]
    public string username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public string email { get; set; }
}
```

### String Length Validation
```csharp
public class UserRequestModel
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string username { get; set; }

    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string email { get; set; }
}
```

### Range Validation
```csharp
public class ProductRequestModel
{
    [Range(0, 99999.99, ErrorMessage = "Price must be between 0 and 99999.99")]
    public decimal price { get; set; }

    [Range(0, 1000, ErrorMessage = "Quantity must be between 0 and 1000")]
    public int quantity { get; set; }
}
```

### Email Validation
```csharp
public class UserRequestModel
{
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string email { get; set; }
}
```

### Regular Expression Validation
```csharp
public class UserRequestModel
{
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string username { get; set; }

    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format")]
    public string phoneNumber { get; set; }
}
```

### Combined Validation
```csharp
using System.ComponentModel.DataAnnotations;

namespace dotnet_8_backend_template.models;

public class UserRequestModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string password { get; set; }
}
```

## Complete Model Examples

### Simple Response Model
```csharp
namespace dotnet_8_backend_template.models;

public class IpResponseModel
{
    public string ip { get; set; }
}
```

### Detailed Response Model
```csharp
namespace dotnet_8_backend_template.models;

public class UserResponseModel
{
    public int id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public bool isActive { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime? lastLoginAt { get; set; }
}
```

### Request Model with Validation
```csharp
using System.ComponentModel.DataAnnotations;

namespace dotnet_8_backend_template.models;

public class CreateProductRequestModel
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters")]
    public string name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    public decimal price { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public string category { get; set; }

    [Range(0, 99999, ErrorMessage = "Stock quantity must be between 0 and 99999")]
    public int stockQuantity { get; set; }

    public List<string>? tags { get; set; }
}
```

### Nested Model Example
```csharp
namespace dotnet_8_backend_template.models;

public class OrderResponseModel
{
    public int id { get; set; }
    public string orderNumber { get; set; }
    public UserResponseModel customer { get; set; }
    public List<OrderItemModel> items { get; set; }
    public decimal totalAmount { get; set; }
    public string status { get; set; }
    public DateTime createdAt { get; set; }
}

public class OrderItemModel
{
    public int productId { get; set; }
    public string productName { get; set; }
    public int quantity { get; set; }
    public decimal unitPrice { get; set; }
    public decimal totalPrice { get; set; }
}
```

## JSON Serialization

### Default Behavior
By default, models are serialized to JSON using System.Text.Json with camelCase properties matching the property names.

```csharp
public class UserResponseModel
{
    public int id { get; set; }  // Serializes as "id"
    public string username { get; set; }  // Serializes as "username"
}
```

### Custom JSON Property Names
```csharp
using System.Text.Json.Serialization;

namespace dotnet_8_backend_template.models;

public class UserResponseModel
{
    [JsonPropertyName("userId")]
    public int id { get; set; }

    [JsonPropertyName("userName")]
    public string username { get; set; }
}
```

### Ignoring Properties
```csharp
using System.Text.Json.Serialization;

namespace dotnet_8_backend_template.models;

public class UserResponseModel
{
    public int id { get; set; }
    public string username { get; set; }

    [JsonIgnore]
    public string passwordHash { get; set; }  // Not serialized
}
```

## Model Organization

### Single Entity Models
Keep related models in the same file when they're small and tightly coupled:

```csharp
namespace dotnet_8_backend_template.models;

public class UserResponseModel
{
    public int id { get; set; }
    public string username { get; set; }
}

public class UserRequestModel
{
    public string username { get; set; }
    public string password { get; set; }
}
```

### Complex Models
Separate into individual files when models are large or complex:

- `UserResponseModel.cs`
- `UserRequestModel.cs`
- `UserSettingsModel.cs`

## Best Practices

1. **Use camelCase for properties** - Matches JSON conventions
2. **Suffix with Model** - Clear indication of purpose (ResponseModel, RequestModel)
3. **Add validation attributes** - Validate at API boundary
4. **Use nullable types appropriately** - `string?`, `int?` for optional fields
5. **Keep models simple** - Pure data structures, no logic
6. **Use meaningful names** - Descriptive property names
7. **Document complex models** - XML comments for clarity
8. **Group related models** - Keep request/response pairs together
9. **Use nested models** - For complex hierarchical data
10. **Avoid circular references** - Can cause serialization issues

## Common Mistakes to Avoid

❌ **DO NOT** use PascalCase for properties
```csharp
public class UserResponseModel
{
    public int Id { get; set; }  // Wrong! Should be camelCase
    public string Username { get; set; }  // Wrong!
}
```

✅ **DO** use camelCase for properties
```csharp
public class UserResponseModel
{
    public int id { get; set; }  // Correct!
    public string username { get; set; }  // Correct!
}
```

❌ **DO NOT** include business logic
```csharp
public class UserResponseModel
{
    public string username { get; set; }
    public string email { get; set; }

    public bool IsValid()  // Wrong! No logic in models
    {
        return !string.IsNullOrEmpty(username);
    }
}
```

✅ **DO** keep models as pure data structures
```csharp
public class UserResponseModel
{
    public string username { get; set; }
    public string email { get; set; }
}
```

❌ **DO NOT** forget validation on request models
```csharp
public class UserRequestModel
{
    public string username { get; set; }  // Missing validation
    public string email { get; set; }  // Missing validation
}
```

✅ **DO** add validation attributes
```csharp
public class UserRequestModel
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string username { get; set; }

    [Required]
    [EmailAddress]
    public string email { get; set; }
}
```

## Checklist for Creating Models

### Response Model Checklist
- ✅ File named `{Entity}ResponseModel.cs`
- ✅ Class named `{Entity}ResponseModel`
- ✅ Namespace is `dotnet_8_backend_template.models`
- ✅ Properties use camelCase
- ✅ Properties have appropriate types (nullable if optional)
- ✅ No business logic in model
- ✅ XML documentation for complex models

### Request Model Checklist
- ✅ File named `{Entity}RequestModel.cs`
- ✅ Class named `{Entity}RequestModel`
- ✅ Namespace is `dotnet_8_backend_template.models`
- ✅ Properties use camelCase
- ✅ Validation attributes added where needed
- ✅ Required fields marked with `[Required]`
- ✅ String lengths specified with `[StringLength]`
- ✅ Email fields use `[EmailAddress]`
- ✅ Numeric ranges use `[Range]`

## Related Documentation

- See [../controllers/AGENTS.md](../controllers/AGENTS.md) for how controllers use models
- See [../services/AGENTS.md](../services/AGENTS.md) for how services transform models
- See [../interfaces/AGENTS.md](../interfaces/AGENTS.md) for interface method signatures
- See [../AGENTS.md](../AGENTS.md) for overall architecture

---

**Last Updated**: 2025-10-15
**Layer**: Models (Data Transfer Objects)
**Pattern**: DTOs for API Communication
