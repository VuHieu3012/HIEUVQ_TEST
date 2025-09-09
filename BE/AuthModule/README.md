# Auth Module - ASP.NET Core Backend

## Overview
This is a complete authentication module built with ASP.NET Core MVC, featuring JWT token-based authentication, user registration, secure login functionality, and data management with role-based access control.

## Tech Stack
- **Backend:** ASP.NET Core MVC 9.0
- **Database:** SQLite with Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **Frontend:** JavaScript + Bootstrap 5 (Views)
- **Password Hashing:** BCrypt.Net
- **Architecture:** Controller → Service → Repository (with DI)
- **CORS:** Custom middleware for cross-origin requests

## Features Implemented

### ✅ Core Authentication
- User registration with validation
- User login with JWT token generation
- Password hashing with BCrypt
- Token validation and refresh
- Secure logout functionality

### ✅ User Management
- Multiple user types (EndUser, Admin, Partner)
- Account status tracking
- Unique username/email validation
- User data management

### ✅ Security Features
- JWT token-based authentication
- Password strength requirements
- Input validation (client & server-side)
- CORS support for API endpoints
- Secure token storage in localStorage
- Custom CORS middleware

### ✅ Data Management
- User list management
- Data export capabilities
- Role-based data access

### ✅ Database
- SQLite database with Entity Framework Core
- User table with proper constraints
- Unique indexes on username and email
- Migration support

## Project Structure
```
AuthModule/
├── Controllers/
│   ├── AuthController.cs      # API endpoints for auth
│   ├── AccountController.cs   # MVC controllers for views
│   ├── DataController.cs      # Data management endpoints
│   └── HomeController.cs      # Protected home page
├── Models/
│   ├── User.cs               # User entity
│   ├── LoginModel.cs         # Login request model
│   ├── RegisterModel.cs      # Registration request model
│   ├── AuthResponse.cs       # API response model
│   └── ErrorViewModel.cs     # Error handling model
├── Services/
│   ├── IAuthService.cs       # Authentication service interface
│   ├── AuthService.cs        # Authentication business logic
│   ├── ITokenService.cs      # JWT token service interface
│   └── TokenService.cs       # JWT token generation/validation
├── Repositories/
│   ├── IUserRepository.cs    # User repository interface
│   └── UserRepository.cs     # User data access
├── Data/
│   └── ApplicationDbContext.cs # Entity Framework context
├── Middleware/
│   └── CorsMiddleware.cs     # Custom CORS handling
├── Migrations/
│   ├── InitialCreate.cs     # Initial database schema
│   ├── FixSeedData.cs       # Seed data migration
│   └── RemoveSeedData.cs    # Cleanup migration
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml      # Login page
│   │   └── Register.cshtml   # Registration page
│   ├── Data/
│   │   └── Index.cshtml      # Data management page
│   ├── Home/
│   │   ├── Index.cshtml      # Protected home page
│   └── Shared/
│       ├── _Layout.cshtml    # Main layout template
│       ├── _ViewImports.cshtml # View imports
│       ├── _ViewStart.cshtml # View start configuration
│       └── Error.cshtml      # Error page
└── wwwroot/
    ├── css/
    │   └── site.css          # Custom styles
    ├── js/
    │   ├── site.js           # Site JavaScript
    │   └── auth.js           # Authentication utilities
    └── lib/                  # Third-party libraries
        ├── bootstrap/        # Bootstrap 5
        ├── jquery/           # jQuery
        └── jquery-validation/ # Form validation
```

## Architecture

The application follows a clean architecture pattern with clear separation of concerns:

```
Controllers → Services → Repositories → Database
     ↓           ↓           ↓
   Views    Middleware   Entity Framework
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
- `GET /Home` - Protected home page (requires authentication)
- `GET /Data` - Data management page (Admin only)

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Navigate to the AuthModule directory
3. Restore packages: `dotnet restore`
4. Update database: `dotnet ef database update`
5. Run the application: `dotnet run`

### Testing
1. Open `https://localhost:5000` in your browser
2. Navigate to `/Account/Register` to create a new account
3. Navigate to `/Account/Login` to login
4. Access `/Home` to see the protected area
5. Access `/Data` to see user management (Admin only)
6. Use `test-api.html` for API testing

## Default Configuration
- Database: SQLite (`authmodule.db`)
- JWT Secret: `YourSuperSecretKeyThatIsAtLeast32CharactersLong!`
- Token Expiry: 60 minutes
- Password Requirements: Minimum 6 characters

## Security Notes
- Passwords are hashed using BCrypt
- JWT tokens are stateless and secure
- Input validation on both client and server
- CORS enabled for API access with custom middleware
- SQL injection protection via Entity Framework
- Role-based access control for data endpoints


## License
This project is part of the KAOPIZ SOFTWARE test assignment.
