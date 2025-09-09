# React Authentication App

A React TypeScript application with clean architecture that replicates the functionality of the AuthModule Views with modern React patterns, custom hooks, and organized routing.

## ✨ Features

- **Authentication System**
  - User login with username/email and password
  - User registration with validation
  - Token-based authentication
  - Remember me functionality
  - Logout functionality

- **Clean Architecture**
  - Feature-based component organization
  - Custom hooks for business logic
  - Centralized router management
  - Utility-first approach
  - TypeScript throughout

- **User Interface**
  - Responsive design using Bootstrap 5
  - Reusable component library
  - Form validation and error handling
  - Loading states and user feedback
  - Protected routes with navigation hooks

- **Data Management**
  - Database data viewer
  - User statistics dashboard
  - Real-time data refresh
  - API service layer

## 📁 Project Structure

```
src/
├── components/           # React components organized by feature
│   ├── auth/            # Authentication components
│   │   ├── LoginForm.tsx
│   │   └── RegisterForm.tsx
│   ├── common/          # Reusable common components
│   │   ├── Alert.tsx
│   │   ├── AlertContainer.tsx
│   │   ├── Button.tsx
│   │   └── LoadingSpinner.tsx
│   ├── layout/          # Layout components
│   │   ├── Layout.tsx
│   │   └── ProtectedRoute.tsx
│   └── pages/           # Page components
│       ├── HomePage.tsx
│       ├── DataPage.tsx
│       └── PrivacyPage.tsx
├── constants/           # Application constants
│   ├── api.ts          # API configuration
│   └── app.ts          # App configuration
├── hooks/              # Custom React hooks
│   ├── useAuth.ts      # Authentication hook
│   ├── useForm.ts      # Form handling hook
│   ├── useAlert.ts     # Alert management hook
│   └── useNavigation.ts # Navigation hook
├── router/             # Router configuration
│   ├── AppRouter.tsx   # Main router component
│   └── index.ts        # Router exports
├── services/           # API services
│   └── authService.ts  # Authentication service
├── types/              # TypeScript type definitions
│   └── index.ts        # All type definitions
├── utils/              # Utility functions
│   ├── api.ts          # API client utility
│   ├── storage.ts      # Local storage utilities
│   └── validation.ts   # Form validation utilities
├── App.tsx             # Main app component
└── index.tsx           # App entry point
```

## Getting Started

### Prerequisites

- Node.js (version 14 or higher)
- npm or yarn
- Backend API running on http://localhost:5000

### Installation

1. Navigate to the project directory:
   ```bash
   cd react-auth-app
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

4. Open your browser and navigate to `http://localhost:3000`

### Configuration

Update the API base URL in `src/services/authService.ts` to match your backend API:

```typescript
const API_BASE_URL = 'http://localhost:5000/api'; // Change this to your backend URL
```

## Available Scripts

- `npm start` - Runs the app in development mode
- `npm build` - Builds the app for production
- `npm test` - Launches the test runner
- `npm eject` - Ejects from Create React App (one-way operation)

## API Endpoints

The app expects the following API endpoints:

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/logout` - User logout
- `POST /api/auth/validate` - Token validation
- `GET /api/auth/profile` - Get user profile
- `GET /api/data/users` - Get all users
- `GET /api/data/stats` - Get user statistics

## 🏗️ Architecture Highlights

### **Clean Code Principles**
- **Separation of Concerns**: Components, hooks, services, and utilities are clearly separated
- **Feature-Based Organization**: Components grouped by functionality (auth, layout, pages)
- **Custom Hooks Pattern**: Business logic extracted into reusable hooks
- **Router Architecture**: Centralized routing with navigation utilities
- **Utility-First Approach**: Reusable functions and constants

### **Custom Hooks**
- `useAuth` - Authentication state and operations
- `useForm` - Form handling with validation
- `useAlert` - Alert/notification management
- `useNavigation` - Programmatic navigation utilities

### **Router System**
- `AppRouter` - Centralized route configuration
- `ProtectedRoute` - Route protection component
- `useNavigation` - Navigation hook with utilities

## 📊 Features Comparison

This React app replicates the functionality of the original ASP.NET Core Views:

| Original View | React Component | Features |
|---------------|-----------------|----------|
| Login.cshtml | LoginForm.tsx | Form validation, API integration, error handling |
| Register.cshtml | RegisterForm.tsx | Registration form, terms modal, validation |
| Home/Index.cshtml | HomePage.tsx | Authentication state, token validation |
| Data/Index.cshtml | DataPage.tsx | User statistics, data table, refresh functionality |
| Shared/_Layout.cshtml | Layout.tsx | Navigation, responsive design, authentication state |

## 🛠️ Technologies Used

- **React 18** - UI library with hooks
- **TypeScript** - Type safety throughout
- **React Router** - Client-side routing
- **Bootstrap 5** - CSS framework
- **Axios** - HTTP client with interceptors
- **Custom Hooks** - State management and business logic

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test your changes
5. Submit a pull request

## License

This project is licensed under the MIT License.
