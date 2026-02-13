# Blueprint.API - Backend REST API

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet" alt=".NET 8.0">
  <img src="https://img.shields.io/badge/API-REST-green" alt="REST API">
  <img src="https://img.shields.io/badge/Auth-JWT-orange" alt="JWT Auth">
  <img src="https://img.shields.io/badge/Pattern-CQRS-blue" alt="CQRS">
</p>

---

## Overview

The **Blueprint.API** solution provides a secure, well-architected REST API backend built with .NET 8. It implements clean architecture principles, CQRS pattern for data access, and JWT Bearer authentication.

> This API is designed to work with the [Blazor.Web](../Blazor.Web/README.md) frontend, but can be used independently with any client.

---

## Project Structure

```
Blueprint.API/
??? Blueprint.API/                 # API Host (Controllers, Configuration)
?   ??? Controllers/
?   ?   ??? AuthController.cs      # Authentication endpoints
?   ??? Configuration/             # Service registration extensions
?   ?   ??? AuthServiceExtensions.cs
?   ?   ??? JwtServiceExtensions.cs
?   ?   ??? SwaggerServiceExtensions.cs
?   ??? Program.cs
??? Blueprint.API.Logic/           # Business Logic Layer
?   ??? AuthLogic/
?   ?   ??? AuthLogic.cs           # Authentication business rules
?   ?   ??? IAuthLogic.cs
?   ??? Helpers/
?       ??? TokenProvider.cs       # JWT token generation
?       ??? PasswordVerifier.cs    # Password hashing
??? Blueprint.API.Repository/      # Data Access Layer (CQRS)
?   ??? AuthRepository/
?       ??? Commands/              # Write operations
?       ?   ??? AuthCommandRepository.cs
?       ?   ??? IAuthCommandRepository.cs
?       ??? Queries/               # Read operations
?           ??? AuthQueryRepository.cs
?           ??? IAuthQueryRepository.cs
??? Blueprint.API.Models/          # API-specific models
??? Blueprint.API.Test/            # Unit tests
```

---

## Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or full instance)

### 1. Configure Secrets

The API requires configuration for database and JWT. These are stored securely using User Secrets.

```bash
cd Blueprint.API

# Set the database connection string
dotnet user-secrets set "ConnectionStrings:DatabaseConnection" "Server=(localdb)\mssqllocaldb;Database=BlueprintAPI;Trusted_Connection=True;MultipleActiveResultSets=true"

# Set the JWT signing key (minimum 32 characters)
dotnet user-secrets set "Jwt:Key" "your-secure-secret-key-at-least-32-characters-long"
```

Or use Visual Studio:
1. Right-click **Blueprint.API** project
2. Select **Manage User Secrets**
3. Add your configuration to `secrets.json`

### 2. Run the API

```bash
cd Blueprint.API
dotnet run
```

The API will be available at: **https://localhost:7115**

### 3. Access Swagger

Navigate to https://localhost:7115/swagger to explore the API documentation.

---

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Issuer": "AppAPI",
    "Audience": "AppAPIUsers",
    "ExpiresInMinutes": 60
  }
}
```

### Required User Secrets

| Key | Description | Example |
|-----|-------------|---------|
| `ConnectionStrings:DatabaseConnection` | SQL Server connection string | `Server=(localdb)\mssqllocaldb;...` |
| `Jwt:Key` | JWT signing key (min 32 chars) | `your-secure-secret-key-here...` |

---

## Architecture

### CQRS Pattern

The repository layer implements a CQRS-lite pattern, separating read and write operations:

```
AuthRepository/
??? Commands/                      # Write operations (INSERT, UPDATE, DELETE)
?   ??? IAuthCommandRepository.cs
?   ??? AuthCommandRepository.cs
?       ??? RegisterUser()
??? Queries/                       # Read operations (SELECT)
    ??? IAuthQueryRepository.cs
    ??? AuthQueryRepository.cs
        ??? GetUserByUsername()
        ??? GetPasswordHashByUsername()
```

**Benefits:**
- Clear separation of read/write concerns
- Easier to optimize queries independently
- Better testability with focused interfaces

### Service Registration

Services are organized into extension methods for clean `Program.cs`:

```csharp
// Program.cs
builder.Services.AddDatabaseServices(builder.Configuration);  // Database settings
builder.Services.AddJwtAuthentication(builder.Configuration); // JWT config
builder.Services.AddSwaggerWithJwtAuth();                     // Swagger + JWT

// Repositories (CQRS)
builder.Services.AddScoped<IAuthQueryRepository, AuthQueryRepository>();
builder.Services.AddScoped<IAuthCommandRepository, AuthCommandRepository>();

// Business Logic
builder.Services.AddScoped<IAuthLogic, AuthLogic>();

// Helpers
builder.Services.AddSingleton<IPasswordVerifier, PasswordVerifier>();
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();
```

### Logging

Structured logging with Serilog is implemented throughout:

| Layer | Log Level | Examples |
|-------|-----------|----------|
| Controllers | Information/Warning | Request start, auth failures |
| Logic | Debug/Warning | Validation, business rule checks |
| Repository | Debug/Warning | Database operations, SP execution |
| JWT Config | Information/Error | Startup configuration, key validation |

---

## API Endpoints

### Authentication

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/api/Auth/register` | Register new user | `RegisterUserDto` |
| `POST` | `/api/Auth/login` | Authenticate user | `LoginUserDto` |

### Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check |

### Request/Response Examples

**Register User**
```http
POST /api/Auth/register
Content-Type: application/json

{
  "username": "newuser",
  "userPassword": "SecurePassword123"
}
```

**Login**
```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "newuser",
  "userPassword": "SecurePassword123"
}
```

**Success Response**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "newuser",
  "role": "User",
  "expiresAt": "2024-01-15T14:30:00Z"
}
```

---

## Testing

### Running Tests

```bash
cd Blueprint.API.Test
dotnet test
```

### Test Coverage

| Area | Tests |
|------|-------|
| AuthLogic | Login, registration, validation |
| TokenProvider | Token generation, expiration, claims |
| AuthMappings | DTO/Entity conversions |

---

## Database

### Stored Procedures

The API uses stored procedures for data access:

| Procedure | Purpose |
|-----------|---------|
| `dbo.usp_GetUserSummaryByUsername` | Retrieve user details |
| `dbo.usp_GetPasswordHashByUsername` | Retrieve password hash for verification |
| `dbo.usp_RegisterUser` | Create new user |

---

## Security Considerations

- ? JWT keys stored in User Secrets (not appsettings.json)
- ? Passwords hashed using ASP.NET Identity PasswordHasher
- ? Minimum 32-character JWT signing key enforced at startup
- ? Token expiration configured
- ? Sensitive data not logged (passwords, keys)

---

## Extending the API

### Adding a New Entity

1. **Create DTOs** in `Template.Models/Dtos/`
2. **Create Query Repository** in `Repository/EntityRepository/Queries/`
3. **Create Command Repository** in `Repository/EntityRepository/Commands/`
4. **Create Logic class** in `Logic/EntityLogic/`
5. **Create Controller** in `Controllers/`
6. **Register services** in `Program.cs`

---

## Screenshots

> Add screenshots to `docs/images/` folder at the repository root.

![Swagger UI](../docs/images/api-swagger-ui.png)

---

## Related Documentation

- [Root README](../README.md) - Full project overview
- [Blazor.Web README](../Blazor.Web/README.md) - Frontend documentation
- [Swagger UI](https://localhost:7115/swagger) - Interactive API docs

---

## License

This project is part of the [Blazor WASM + API Starter Template](../README.md) and is licensed under the MIT License.
