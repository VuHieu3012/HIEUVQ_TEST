# AuthModule Test Suite

Bộ test xUnit đơn giản và tập trung cho AuthModule APIs sử dụng **Data-Driven Testing Pattern**.

## Cấu trúc Test

### Controller Tests
- **`AuthControllerTests.cs`** - Test các API authentication (login, register, validate, logout)
- **`DataControllerTests.cs`** - Test các API admin (GetUsers)
- **`AccountControllerTests.cs`** - Test các view actions
- **`HomeControllerTests.cs`** - Test các home views (Index, Error)

### Service Tests
- **`AuthServiceTests.cs`** - Test business logic của AuthService
- **`TokenServiceTests.cs`** - Test JWT token generation và validation

## Test Coverage

### AuthController APIs
- ✅ **Login** - Valid/invalid credentials, validation errors
- ✅ **Register** - Valid registration, duplicates, validation errors
- ✅ **Validate Token** - Valid/invalid tokens
- ✅ **Logout** - Token logout

### DataController APIs
- ✅ **GetUsers** - Admin access, access denied

### AccountController Views
- ✅ **Login/Register/AccessDenied** - View rendering

### HomeController Views
- ✅ **Index** - Home page view rendering
- ✅ **Error** - Error page view rendering
- ✅ **HTTP Methods** - GET/POST method testing
- ✅ **Route Testing** - Different case routes

### Service Layer
- ✅ **AuthService** - Login, register, validate, logout logic
- ✅ **TokenService** - JWT generation, validation, user extraction

## Chạy Tests

```bash
# Chạy tất cả tests
dotnet test AuthModule.Tests

# Chạy với output chi tiết
dotnet test AuthModule.Tests --verbosity normal

# Chạy test class cụ thể
dotnet test AuthModule.Tests --filter "AuthControllerTests"
```

## Test Data

- **Admin User**: `admin@test.com` / `Admin123!`
- **Regular User**: `user1@test.com` / `User123!`

## Đặc điểm

- **Data-Driven Testing**: Sử dụng `[Theory]` + `[MemberData]` pattern
- **Comprehensive Coverage**: Test tất cả controllers và services
- **Tách biệt**: Mỗi test sử dụng database riêng
- **Nhanh**: Sử dụng in-memory database
- **Dễ hiểu**: Code test rõ ràng, dễ maintain

## Test Statistics

| Test Class | Test Methods | Data Cases | Total Tests |
|------------|--------------|------------|-------------|
| AuthControllerTests | 4 | 6+7+4+2 | 19 |
| DataControllerTests | 1 | 4 | 4 |
| AccountControllerTests | 3 | 3 | 3 |
| HomeControllerTests | 6 | 5+5+6+3 | 19 |
| AuthServiceTests | 3 | 5+3+2 | 10 |
| TokenServiceTests | 3 | 3+1+5 | 9 |
| **TOTAL** | **20** | **64** | **64** |

Bộ test này đảm bảo tất cả APIs và services hoạt động đúng và an toàn cho production.