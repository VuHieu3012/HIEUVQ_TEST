# React Auth App - Clean Architecture

## 📁 Project Structure

```
src/
├── components/           # React components organized by feature
│   ├── auth/            # Authentication related components
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

## 🏗️ Architecture Principles

### 1. **Separation of Concerns**
- **Components**: Pure UI components with minimal logic
- **Hooks**: Business logic and state management
- **Services**: API communication and data fetching
- **Utils**: Pure functions and utilities
- **Constants**: Configuration and static values

### 2. **Feature-Based Organization**
- Components grouped by feature (auth, layout, pages)
- Router separated into dedicated directory
- Related functionality kept together
- Easy to locate and maintain code

### 3. **Custom Hooks Pattern**
- `useAuth`: Authentication state and operations
- `useForm`: Form handling with validation
- `useAlert`: Alert/notification management
- `useNavigation`: Navigation utilities and helpers
- Reusable logic extracted from components

### 4. **Router Architecture**
- `AppRouter`: Centralized route configuration
- `ProtectedRoute`: Route protection component
- `useNavigation`: Programmatic navigation hook
- Clean separation of routing logic

### 5. **Utility-First Approach**
- `apiClient`: Centralized API communication
- `storage`: Local storage abstraction
- `validation`: Form validation rules and functions

### 6. **Type Safety**
- Comprehensive TypeScript interfaces
- Strict type checking throughout
- Better IDE support and error prevention

## 🔧 Key Features

### **Custom Hooks**
- **useAuth**: Manages authentication state, login, register, logout
- **useForm**: Handles form state, validation, and submission
- **useAlert**: Manages notifications and alerts
- **useNavigation**: Programmatic navigation and route utilities

### **Router System**
- **AppRouter**: Centralized route configuration
- **ProtectedRoute**: Route protection with authentication check
- **useNavigation**: Navigation utilities and helpers
- Clean separation of routing concerns

### **Reusable Components**
- **Button**: Configurable button with loading states
- **Alert**: Dismissible alert component
- **LoadingSpinner**: Loading indicator component
- **AlertContainer**: Manages multiple alerts

### **API Layer**
- Centralized API client with interceptors
- Automatic token management
- Error handling and response transformation

### **Validation System**
- Declarative validation rules
- Real-time form validation
- Custom validation functions

## 🚀 Benefits

1. **Maintainability**: Clear separation of concerns
2. **Reusability**: Common components and hooks
3. **Testability**: Pure functions and isolated logic
4. **Scalability**: Easy to add new features
5. **Type Safety**: Comprehensive TypeScript coverage
6. **Developer Experience**: Better IDE support and debugging
7. **Router Management**: Clean and organized routing system
8. **Navigation**: Programmatic navigation with hooks

## 📝 Usage Examples

### Using Custom Hooks
```typescript
// Authentication
const { isAuthenticated, login, logout } = useAuth();

// Form handling
const { values, errors, handleChange, handleSubmit } = useForm({
  initialValues: { username: '', password: '' },
  validationRules: { username: validationRules.username },
  onSubmit: handleLogin
});

// Alerts
const { alerts, showAlert, removeAlert } = useAlert();

// Navigation
const { goToHome, goToLogin, isCurrentPath } = useNavigation();
```

### Using Router System
```typescript
// AppRouter.tsx - Route configuration
<Routes>
  <Route path="/" element={<HomePage />} />
  <Route path="/login" element={<LoginForm />} />
  <Route path="/data" element={
    <ProtectedRoute>
      <DataPage />
    </ProtectedRoute>
  } />
</Routes>

// Navigation in components
const { goToLogin, goToRegister } = useNavigation();
<Button onClick={goToLogin}>Login</Button>
```

### Using Reusable Components
```typescript
// Button with loading state
<Button isLoading={isSubmitting} loadingText="Saving...">
  Save
</Button>

// Alert container
<AlertContainer alerts={alerts} onRemoveAlert={removeAlert} />

// Loading spinner
<LoadingSpinner size="sm" />
```

## 🛣️ Router Features

### **Route Configuration**
- Centralized route management in `AppRouter.tsx`
- Public routes: Home, Login, Register
- Protected routes: Data (requires authentication)
- Fallback route: Redirect to Home

### **Navigation Hook**
```typescript
const {
  goTo,           // Navigate to any path
  goBack,         // Go back in history
  goForward,      // Go forward in history
  goToHome,       // Navigate to home
  goToLogin,      // Navigate to login
  goToRegister,   // Navigate to register
  goToData,       // Navigate to data page
  isCurrentPath,  // Check if current path matches
  currentPath,    // Get current path
  previousPath    // Get previous path
} = useNavigation();
```

### **Route Protection**
```typescript
// Protect routes that require authentication
<ProtectedRoute>
  <DataPage />
</ProtectedRoute>
```

This architecture follows React best practices and provides a solid foundation for scalable applications with clean routing management.
