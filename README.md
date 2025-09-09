# Authentication System - Full Stack Project

A complete authentication system built with ASP.NET Core backend and React frontend, featuring JWT token-based authentication, user management, and role-based access control.

## 🏗️ Project Architecture

```
HIEUVQ/
├── BE/                          # Backend Solution (.NET 9.0)
│   ├── AuthModule/              # Main Web API & MVC Application
│   ├── AuthModule.Tests/        # Unit & Integration Tests
│   └── BE.sln                   # Solution file
├── react-auth-app/              # Frontend Application (React + TypeScript)
└── README.md                    # This file
```

## 🚀 Quick Start

### Prerequisites
- **.NET 9.0 SDK** (for backend)
- **Node.js 14+** (for frontend)
- **Visual Studio 2022** or **VS Code**

### Backend Setup
```bash
cd BE
dotnet restore
dotnet build
dotnet run --project AuthModule
```
Backend will run on: `http://localhost:5000`

### Frontend Setup
```bash
cd react-auth-app
npm install
npm start
```
Frontend will run on: `http://localhost:3000`

## 📋 Features Overview

### ✅ Authentication System
- **User Registration** with validation
- **User Login** with JWT tokens
- **Token Refresh** mechanism
- **Secure Logout** functionality
- **Remember Me** functionality
- **Password Hashing** with BCrypt

### ✅ User Management
- **Multiple User Types**: EndUser, Admin, Partner
- **Role-based Access Control**
- **User Data Management**
- **Account Status Validation** (IsActive field with basic checks)

### ✅ Security Features
- **JWT Token Authentication**
- **CORS Support** for cross-origin requests
- **Input Validation** (client & server-side)
- **SQL Injection Protection**
- **Secure Token Storage**

### ✅ Data Management
- **User List Display** (Admin only)
- **Real-time Data Refresh**
- **Database Operations** with Entity Framework Core

### ⚠️ Limited Features
- **Account Status Management**: Only validation, no activation/deactivation UI
- **User Management**: Basic CRUD, no advanced admin features

## 🛠️ Technology Stack

### Backend (BE/)
- **Framework**: ASP.NET Core MVC 9.0
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Architecture**: Controller → Service → Repository (with DI)
- **Testing**: xUnit with Moq for mocking
- **Password Hashing**: BCrypt.Net

### Frontend (react-auth-app/)
- **Framework**: React 18 with TypeScript
- **Routing**: React Router v6
- **State Management**: React Context + Custom Hooks
- **UI Framework**: Bootstrap 5
- **HTTP Client**: Axios with interceptors
- **Storage**: localStorage for token management

## 📁 Detailed Project Structure

### Backend Structure
```
BE/
├── AuthModule/                    # Main Application
│   ├── Controllers/              # API & MVC Controllers
│   │   ├── AuthController.cs     # Authentication API endpoints
│   │   └── HomeController.cs     # MVC & Data API endpoints
│   ├── Services/                 # Business Logic Layer
│   │   ├── AuthService.cs        # Core authentication logic
│   │   └── TokenService.cs       # JWT token management
│   ├── Repositories/             # Data Access Layer
│   │   └── UserRepository.cs     # User data operations
│   ├── DTOs/                     # Data Transfer Objects
│   │   └── User.cs               # User DTO
│   ├── Models/                   # Request/Response Models
│   │   ├── LoginModel.cs
│   │   ├── RegisterModel.cs
│   │   └── AuthResponse.cs
│   ├── Data/                     # Database Context
│   │   └── ApplicationDbContext.cs
│   ├── Views/                    # MVC Views
│   │   ├── Account/              # Login & Register pages
│   │   ├── Home/                 # Home & Data pages
│   │   └── Shared/               # Layout templates
│   ├── wwwroot/                  # Static Files
│   │   ├── css/                  # Custom styles
│   │   ├── js/                   # Client-side JavaScript
│   │   └── lib/                  # Third-party libraries
│   └── Migrations/               # Database Migrations
└── AuthModule.Tests/             # Test Project
    ├── Controllers/              # Controller tests with mocking
    └── Services/                 # Service unit tests
```

### Frontend Structure
```
react-auth-app/
├── src/
│   ├── components/               # React Components
│   │   ├── auth/                # Authentication components
│   │   │   ├── LoginForm.tsx    # Login form
│   │   │   └── RegisterForm.tsx # Registration form
│   │   ├── common/              # Reusable components
│   │   │   ├── Alert.tsx        # Alert notifications
│   │   │   ├── Button.tsx       # Custom button
│   │   │   └── LoadingSpinner.tsx
│   │   ├── layout/              # Layout components
│   │   │   ├── Layout.tsx       # Main layout
│   │   │   ├── ProtectedRoute.tsx # Route protection
│   │   │   └── AdminRoute.tsx   # Admin-only routes
│   │   └── pages/               # Page components
│   │       ├── HomePage.tsx     # Home dashboard
│   │       └── DataPage.tsx     # Data management
│   ├── contexts/                # React Context
│   │   └── AuthContext.tsx      # Authentication state
│   ├── hooks/                   # Custom Hooks
│   │   └── useAlert.ts          # Alert management
│   ├── services/                # API Services
│   │   └── authService.ts       # Authentication API calls
│   ├── utils/                   # Utility Functions
│   │   ├── api.ts              # API client configuration
│   │   └── storage.ts          # Local storage utilities
│   ├── constants/              # Application Constants
│   │   ├── api.ts              # API endpoints
│   │   └── app.ts              # App configuration
│   ├── types/                  # TypeScript Types
│   │   └── index.ts            # Type definitions
│   └── router/                 # Routing Configuration
│       └── AppRouter.tsx       # Main router setup
```

## 🔌 API Endpoints

### Authentication API (`/api/auth/`)
- `POST /login` - User authentication
- `POST /register` - User registration
- `POST /validate` - Token validation
- `POST /refresh` - Token refresh
- `POST /logout` - User logout

### Data Management API (`/api/data/`)
- `GET /users` - Get all users (Admin only)

### MVC Routes
- `GET /Account/Login` - Login page
- `GET /Account/Register` - Registration page
- `GET /` - Home page (protected)
- `GET /Data` - Data management page (Admin only)


## 🧪 Testing

### Backend Testing
```bash
cd BE
dotnet test
```
- **Unit Tests**: Service layer business logic
- **Integration Tests**: API endpoints with mocking
- **Repository Tests**: Data access layer

### Frontend Testing
```bash
cd react-auth-app
npm test
```
- **Component Tests**: React component testing
- **Hook Tests**: Custom hook testing
- **Service Tests**: API service testing

## ⚙️ Configuration

### Backend Configuration (`appsettings.json`)
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

### Frontend Configuration (`src/constants/api.ts`)
```typescript
export const API_CONFIG = {
  BASE_URL: 'http://localhost:5000/api',
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/auth/login',
      REGISTER: '/auth/register',
      // ... other endpoints
    }
  }
};
```

## 🚀 Deployment

### Backend Deployment
1. Build the application: `dotnet build --configuration Release`
2. Publish: `dotnet publish --configuration Release --output ./publish`
3. Deploy to your hosting platform

### Frontend Deployment
1. Build: `npm run build`
2. Deploy the `build` folder to your static hosting service



