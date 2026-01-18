# Devices

A modular .NET 10 solution exposing a RESTful Devices API. The solution is built with a layered architecture separating concerns across Domain, Infrastructure, Application, and API layers, with comprehensive OpenAPI/Swagger documentation.

## Table of contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Projects](#projects)
- [Prerequisites](#prerequisites)
- [Quick start (CLI)](#quick-start-cli)
- [Quick start (Visual Studio)](#quick-start-visual-studio)
- [API Endpoints](#api-endpoints)
- [API Documentation (Swagger / OpenAPI)](#api-documentation-swagger--openapi)
- [Database](#database)
- [Error Handling](#error-handling)
- [Docker / Containers](#docker--containers)
- [Running Tests](#running-tests)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Overview

This repository implements a complete RESTful API for managing devices with full CRUD operations. The solution demonstrates best practices including:

- **Layered Architecture**: Clean separation of concerns across Domain, Infrastructure, Application, and API layers
- **Dependency Injection**: Built-in ASP.NET Core DI for service registration and resolution
- **Result Pattern**: Type-safe error handling using `ResultWrapper<T>` for operations
- **Entity Framework Core**: SQL Server database persistence with migrations
- **OpenAPI/Swagger**: Auto-generated, interactive API documentation
- **Structured Logging**: Built-in ASP.NET Core logging with controller action tracking
- **ProblemDetails**: Standardized error responses following RFC 7807

## Architecture

```
Devices.Api
  ??? Controllers (HTTP endpoints)
  ??? Mapper (Result to ProblemDetails conversion)
  ??? Program.cs (DI, middleware configuration)

Devices.Application
  ??? Services (IDeviceServices business logic)
  ??? Interfaces (Service contracts)
  ??? Dtos (Data transfer objects)
  ??? Common (ResultWrapper<T>, Error)

Devices.Infrastructure
  ??? DevicesDbContext (EF Core DbContext)
  ??? Repositorys (IDeviceRepository implementation)
  ??? Migrations (Database schema versioning)

Devices.Domain
  ??? DeviceModel (Core entity)
  ??? Interfaces (IDeviceRepository contract)
  ??? Enums (DeviceState enum)

Devices.Tests
  ??? Unit and integration tests
```

**Data Flow**: HTTP Request ? API Controller ? Application Service ? Domain Repository ? Database

## Projects

### Devices.Api (`net10.0`)

ASP.NET Core Web API project serving as the entry point.

**Key Files**:
- `Program.cs` — Host configuration, middleware setup, OpenAPI/Swagger configuration
- `Controllers/DeviceController.cs` — REST endpoints for device operations
- `Mapper/ResultToProblemMapper.cs` — Converts `ResultWrapper<T>` to `ProblemDetails`

**Responsibilities**:
- HTTP request handling and routing
- Input validation and binding
- Response mapping and error handling
- OpenAPI schema generation

### Devices.Application (`net10.0`)

Application layer containing business logic and service contracts.

**Key Files**:
- `Services/DeviceServices.cs` — Core business logic for device management
- `Interfaces/IDeviceServices.cs` — Service contract
- `Dtos/Device/UpdateDevicePatchDto.cs` — Partial update model for PATCH operations
- `Common/ResultWrapper.cs` — Result pattern implementation
- `Common/Error.cs` — Error representation

**Responsibilities**:
- Business rule implementation
- Service orchestration
- Data transfer object (DTO) mapping
- Transaction management (if needed)

### Devices.Infrastructure (`net10.0`)

Data access layer managing database interactions.

**Key Files**:
- `DevicesDbContext.cs` — Entity Framework Core DbContext configuration
- `Repositorys/DeviceRepository.cs` — Generic repository implementation
- `Migrations/` — Database schema history and versioning

**Responsibilities**:
- Database context configuration
- Repository pattern implementation
- LINQ query execution
- Migration management

### Devices.Domain (`net10.0`)

Core domain models and contracts.

**Key Files**:
- `DeviceModel.cs` — Device entity with properties: `Id`, `Name`, `Brand`, `State`, `CreationTime`
- `Interfaces/IDeviceRepository.cs` — Data access contract
- `Enums.cs` — `DeviceState` enumeration

**Responsibilities**:
- Domain entity definitions
- Repository interface contracts
- Domain enumerations

### Devices.Tests (`net10.0`)

Unit and integration test project.

**Responsibilities**:
- Test coverage for services and repositories
- Mocking external dependencies
- Validation of business logic

## Prerequisites

- **.NET SDK 10** (matching `net10.0` target framework)
- **SQL Server** (local or remote database for persistence)
- Optional: Docker and Docker Compose for containerized development
- Optional: Visual Studio 2022/2026 or VS Code with C# Dev Kit

**Verify SDK**:
```bash
dotnet --version
```

## Quick start (CLI)

### 1. Restore dependencies
```bash
dotnet restore
```

### 2. Build the solution
```bash
dotnet build --configuration Release
```

### 3. Apply database migrations
```bash
cd Devices.Api
dotnet ef database update
```

### 4. Run the API
```bash
dotnet run --urls "http://localhost:5000;https://localhost:5001"
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

## Quick start (Visual Studio)

1. Open the solution in Visual Studio 2022/2026
2. Set `Devices.Api` as the startup project
3. Ensure your connection string is configured (see [Database](#database) section)
4. Run with **F5** (with debugger) or **Ctrl+F5** (without debugger)

**Container Support**: If you prefer to run in a container, select the Docker profile from the run/debug dropdown.

## API Endpoints

All endpoints are prefixed with `/device`.

### Read Operations

#### Get All Devices
```http
GET /device
```
**Response**: `200 OK` with list of all devices

#### Get Device by ID
```http
GET /device/GetById/{id}
```
**Parameters**:
- `id` (UUID, required) — Device unique identifier

**Response**: `200 OK` with device details, or `404 Not Found`

#### Get Devices by Brand
```http
GET /device/ByBrand/{brand}
```
**Parameters**:
- `brand` (string, required) — Brand filter

**Response**: `200 OK` with matching devices, or `404 Not Found`

#### Get Devices by State
```http
GET /device/ByState/{state}
```
**Parameters**:
- `state` (integer, required) — Device state (see [Device States](#device-states))

**Response**: `200 OK` with matching devices

### Create Operations

#### Create Device
```http
POST /device
Content-Type: application/json

{
  "name": "iPhone 15",
  "brand": "Apple",
  "state": 0
}
```
**Response**: `201 Created` with device details and `Location` header

### Update Operations

#### Update Device (Partial)
```http
PATCH /device/{id}
Content-Type: application/json

{
  "name": "Updated Name",
  "state": 1
}
```
**Response**: `204 No Content` on success, `404 Not Found`, or `400 Bad Request`

### Delete Operations

#### Delete Device
```http
DELETE /device/{id}
```
**Response**: `200 OK` on success, or `404 Not Found`

### Device States

Device states are represented as integers:
- `0` — Unknown/Default
- `1` — Active
- `2` — Inactive
- `3` — Maintenance (values may vary; check `Devices.Domain/Enums.cs`)

## API Documentation (Swagger / OpenAPI)

The project uses **Swashbuckle** and **Microsoft.AspNetCore.OpenApi** to generate interactive API documentation.

### Access Swagger UI

With the API running, open:
```
https://localhost:5001/swagger/index.html
```
(or `http://localhost:5000/swagger/index.html` for HTTP)

**Features**:
- Interactive endpoint testing
- Request/response schema visualization
- Automatic schema generation from XML documentation
- Try-it-out functionality for all endpoints

### OpenAPI Document

The raw OpenAPI specification (JSON) is available at:
```
https://localhost:5001/openapi/v1.json
```

Use this for client code generation, API testing tools (Postman, Insomnia), or documentation generation.

## Database

### Configuration

Connection string is configured in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DevicesDb;Trusted_Connection=true;"
  }
}
```

### Migrations

Apply migrations to initialize or update the schema:
```bash
dotnet ef database update
```

Create a new migration after model changes:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

Drop and recreate the database:
```bash
dotnet ef database drop --force
dotnet ef database update
```

## Error Handling

### ProblemDetails Format

All errors are returned as `ProblemDetails` (RFC 7807) objects:
```json
{
  "type": "about:blank",
  "title": "Unexpected Error",
  "status": 500,
  "detail": "Exception message",
  "traceId": "00-trace-id-00"
}
```

### Result Pattern

Operations return `ResultWrapper<T>` or `ResultWrapper`:

```csharp
// Success
var result = await _deviceServices.GetByIdAsync(id, ct);
if (result.IsSuccess)
{
    var device = result.Value;
}

// Failure
if (!result.IsSuccess)
{
    var error = result.Error;
    Console.WriteLine(error.Message);
}
```

## Docker / Containers

### Build Docker Image

```bash
docker build -t devices-api:local -f Devices.Api/Dockerfile .
```

### Run Container

```bash
docker run --rm -p 5000:80 -p 5001:443 devices-api:local
```

### Visual Studio Container Support

Visual Studio includes container tooling. To run with container support:
1. In Visual Studio, select the **Docker** profile from the run/debug dropdown
2. Build and run with **F5**

**Note**: Ensure Docker Desktop is running.

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Project
```bash
dotnet test Devices.Tests/Devices.Tests.csproj
```

### Run with Coverage
```bash
dotnet test /p:CollectCoverage=true
```

**Best Practices**:
- Write unit tests for business logic in `Services`
- Use mocking for repository dependencies
- Keep tests fast and deterministic
- Test both success and failure scenarios

## Development Workflow

### Setting Up Local Development

1. Clone the repository
2. Restore dependencies: `dotnet restore`
3. Build solution: `dotnet build`
4. Configure connection string in `appsettings.json` (or use User Secrets)
5. Apply migrations: `dotnet ef database update`
6. Run: `dotnet run` (or F5 in Visual Studio)

### User Secrets (Development)

Store sensitive configuration locally without committing to version control:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

### Adding New Features

1. Define domain model in `Devices.Domain`
2. Create repository interface in `Devices.Domain/Interfaces`
3. Implement repository in `Devices.Infrastructure`
4. Create DTOs in `Devices.Application/Dtos`
5. Implement service in `Devices.Application/Services`
6. Add controller endpoint in `Devices.Api/Controllers`
7. Add unit tests in `Devices.Tests`
8. Update this README if needed

### Code Generation

After model changes, regenerate the database:
```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

## Coding Standards

### C# and .NET

- Target framework: `.NET 10.0`
- C# version: `14.0` (latest language features)
- Nullable reference types: **Enabled** (`<Nullable>enable</Nullable>`)
- Implicit usings: **Enabled** (`<ImplicitUsings>enable</ImplicitUsings>`)

### Code Style

- Follow **PascalCase** for classes, methods, and properties
- Use **camelCase** for private fields and local variables
- Prefix private fields with `_` (e.g., `_logger`)
- Provide XML documentation for public APIs (`///` comments)
- Keep methods focused and under 20 lines when possible

### Documentation

- Document public methods with `<summary>`, `<param>`, and `<returns>` tags
- Use meaningful variable and method names
- Add comments for complex logic or non-obvious decisions

### Async/Await

- Use `async Task` for I/O operations
- Always use `CancellationToken` parameters for cancellable operations
- Avoid `.Result` or `.Wait()`; use `await`

### Dependency Injection

Register services in `Program.cs`:
```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

Inject dependencies through constructor parameters:
```csharp
public class MyController
{
    private readonly IMyService _service;
    
    public MyController(IMyService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
}
```

## Troubleshooting

### Port Already in Use

If ports `5000` or `5001` are already in use:
```bash
dotnet run --urls "http://localhost:5002;https://localhost:5003"
```

### Database Connection Issues

- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Use User Secrets for local development
- Ensure database exists or allow automatic creation

### HTTPS Certificate Issues

Trust the ASP.NET Core development certificate:
```bash
dotnet dev-certs https --trust
```

Remove and recreate if needed:
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Build Errors

Clean and rebuild:
```bash
dotnet clean
dotnet restore
dotnet build
```

### EF Core Migration Issues

Reset migrations (development only):
```bash
dotnet ef database drop --force
dotnet ef database update
```

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes and add tests
4. Commit with clear messages (`git commit -m 'Add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

Please ensure all tests pass and your code follows the coding standards above.

## License

This project is licensed under the MIT License — see the LICENSE file for details.

---

**Repository**: [FilipeMallmann/Devices](https://github.com/FilipeMallmann/Devices)  
**Branch**: `14-api-swagger-documentation`  
**Framework**: .NET 10.0  
**Language**: C# 14.0

