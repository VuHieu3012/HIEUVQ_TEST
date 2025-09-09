# Backend Solution - AuthModule

This is the backend solution for the AuthModule project, restructured with proper .NET solution architecture.

## Solution Structure

```
BE/
├── BE.sln                          # Solution file
├── AuthModule/                     # Main web application project
│   ├── AuthModule.csproj          # Project file
│   ├── Program.cs                 # Application entry point
│   ├── Controllers/               # API and MVC controllers
│   ├── Services/                  # Business logic services
│   ├── Repositories/              # Data access layer
│   ├── Models/                    # Data models and DTOs
│   ├── Data/                      # Entity Framework context
│   ├── Middleware/                # Custom middleware
│   ├── Views/                     # MVC views
│   ├── wwwroot/                   # Static files
│   └── Migrations/                # Database migrations
└── AuthModule.Tests/              # Unit and integration tests
    ├── AuthModule.Tests.csproj    # Test project file
    ├── Services/                  # Service layer tests
    └── Controllers/               # Controller integration tests
```

## Projects

### AuthModule
The main ASP.NET Core MVC web application containing:
- JWT-based authentication system
- User registration and login functionality
- Role-based access control
- SQLite database with Entity Framework Core
- RESTful API endpoints
- MVC views for web interface

### AuthModule.Tests
Comprehensive test suite including:
- Unit tests for services and business logic
- Integration tests for API endpoints
- In-memory database testing
- Test coverage for authentication flows

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Building the Solution
```bash
# Navigate to the BE directory
cd BE

# Restore packages
dotnet restore

# Build the entire solution
dotnet build

# Build specific project
dotnet build AuthModule
dotnet build AuthModule.Tests
```

### Running the Application
```bash
# Run the main application
dotnet run --project AuthModule

# Run tests
dotnet test
```

### Database Setup
```bash
# Navigate to AuthModule project
cd AuthModule

# Apply migrations
dotnet ef database update
```

## Testing

The solution includes comprehensive testing:

### Unit Tests
- Service layer business logic testing
- Repository pattern testing
- Authentication service testing

### Integration Tests
- API endpoint testing
- End-to-end authentication flow testing
- Database integration testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test AuthModule.Tests
```

## API Endpoints

### Authentication API (`/api/auth/`)
- `POST /login` - User login
- `POST /register` - User registration
- `POST /validate` - Token validation
- `POST /logout` - User logout

### Data Management API (`/api/data/`)
- `GET /users` - Get all users (Admin only)
- `GET /stats` - Get user statistics (Admin only)

### MVC Controllers
- `GET /Account/Login` - Login page
- `GET /Account/Register` - Registration page
- `GET /Home` - Protected home page
- `GET /Data` - Data management page (Admin only)

## Configuration

The application uses `appsettings.json` for configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=authmodule.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "AuthModule",
    "Audience": "AuthModuleUsers",
    "ExpiryMinutes": 60
  }
}
```

## Architecture

The solution follows clean architecture principles:

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Controllers    │───▶│    Services      │───▶│  Repositories   │
│   (API/MVC)     │    │  (Business      │    │  (Data Access)  │
│                 │    │   Logic)        │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│     Views       │    │   Middleware    │    │   Database      │
│   (Frontend)    │    │  (CORS/Auth)    │    │   (SQLite)      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Development

### Adding New Features
1. Create models in `AuthModule/Models/`
2. Add repository methods in `AuthModule/Repositories/`
3. Implement business logic in `AuthModule/Services/`
4. Create controllers in `AuthModule/Controllers/`
5. Add corresponding tests in `AuthModule.Tests/`

### Database Changes
1. Create migration: `dotnet ef migrations add MigrationName`
2. Update database: `dotnet ef database update`
3. Test changes with unit tests

## Security Features

- JWT token-based authentication
- Password hashing with BCrypt
- Input validation (client & server-side)
- CORS support for API endpoints
- Role-based authorization
- SQL injection protection via Entity Framework

## License

This project is part of the KAOPIZ SOFTWARE test assignment.
