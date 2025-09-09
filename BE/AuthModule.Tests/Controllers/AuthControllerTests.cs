using Microsoft.AspNetCore.Mvc.Testing;
using AuthModule.Models;
using AuthModule.DTOs;
using AuthModule.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AuthModule.Tests.Controllers
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Mock<IAuthService> _mockAuthService;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _mockAuthService = new Mock<IAuthService>();
            
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real AuthService
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthService));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    
                    // Add the mock AuthService
                    services.AddSingleton(_mockAuthService.Object);
                });
            }).CreateClient();
        }

        #region Login Test Data

        public static IEnumerable<object?[]> LoginTestData()
        {
            // Valid login
            yield return new object?[] { 
                new LoginModel { UsernameOrEmail = "admin@test.com", Password = "Admin123!" },
                HttpStatusCode.OK,
                true,
                "Login successful"
            };
            // Invalid credentials
            yield return new object?[] { 
                new LoginModel { UsernameOrEmail = "admin@test.com", Password = "WrongPassword" },
                HttpStatusCode.Unauthorized,
                false,
                "Invalid username/email or password"
            };
            // Empty input
            yield return new object?[] { 
                new LoginModel { UsernameOrEmail = "", Password = "" },
                HttpStatusCode.BadRequest,
                false,
                "Invalid input data"
            };
        }

        #endregion

        #region Register Test Data

        public static IEnumerable<object?[]> RegisterTestData()
        {
            // Valid registration
            yield return new object?[] { 
                new RegisterModel 
                { 
                    Username = "newuser1", 
                    Email = "newuser1@test.com", 
                    Password = "NewUser123!", 
                    ConfirmPassword = "NewUser123!", 
                    UserType = "User", 
                    AcceptTerms = true 
                },
                HttpStatusCode.OK,
                true,
                "Registration successful"
            };
            // Username exists
            yield return new object?[] { 
                new RegisterModel 
                { 
                    Username = "admin", 
                    Email = "newemail@test.com", 
                    Password = "NewUser123!", 
                    ConfirmPassword = "NewUser123!", 
                    UserType = "User", 
                    AcceptTerms = true 
                },
                HttpStatusCode.BadRequest,
                false,
                "Username already exists"
            };
            // Invalid input
            yield return new object?[] { 
                new RegisterModel 
                { 
                    Username = "", 
                    Email = "invalid-email", 
                    Password = "123", 
                    ConfirmPassword = "DifferentPassword", 
                    UserType = "User", 
                    AcceptTerms = false 
                },
                HttpStatusCode.BadRequest,
                false,
                "Invalid input data"
            };
        }

        #endregion


        #region Login Tests

        [Theory]
        [MemberData(nameof(LoginTestData))]
        public async Task Login_WithDifferentCredentials_ReturnsExpectedResult(
            LoginModel loginModel,
            HttpStatusCode expectedStatusCode,
            bool expectedSuccess,
            string expectedMessage)
        {
            // Arrange
            var expectedResponse = new AuthResponse
            {
                Success = expectedSuccess,
                Message = expectedMessage,
                Token = expectedSuccess ? "mock.jwt.token" : null,
                User = expectedSuccess ? new User { Id = 1, Username = "testuser", Email = "test@test.com", UserType = "User" } : null
            };

            // Setup mock based on specific input
            _mockAuthService
                .Setup(x => x.LoginAsync(It.Is<LoginModel>(m => 
                    m.UsernameOrEmail == loginModel.UsernameOrEmail && 
                    m.Password == loginModel.Password)))
                .ReturnsAsync(expectedResponse);

            // Act
            var json = JsonSerializer.Serialize(loginModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            var responseJson = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(authResponse);
            Assert.Equal(expectedSuccess, authResponse.Success);
            Assert.Equal(expectedMessage, authResponse.Message);

            if (expectedSuccess)
            {
                Assert.NotNull(authResponse.Token);
                Assert.NotNull(authResponse.User);
            }

            // Verify service was called with correct parameters
            _mockAuthService.Verify(x => x.LoginAsync(It.Is<LoginModel>(m => 
                m.UsernameOrEmail == loginModel.UsernameOrEmail && 
                m.Password == loginModel.Password)), Times.Once);
        }

        #endregion

        #region Register Tests

        [Theory]
        [MemberData(nameof(RegisterTestData))]
        public async Task Register_WithDifferentData_ReturnsExpectedResult(
            RegisterModel registerModel,
            HttpStatusCode expectedStatusCode,
            bool expectedSuccess,
            string expectedMessage)
        {
            // Arrange
            var expectedResponse = new AuthResponse
            {
                Success = expectedSuccess,
                Message = expectedMessage,
                Token = expectedSuccess ? "mock.jwt.token" : null,
                User = expectedSuccess ? new User { Id = 2, Username = registerModel.Username, Email = registerModel.Email, UserType = registerModel.UserType } : null
            };

            _mockAuthService
                .Setup(x => x.RegisterAsync(It.IsAny<RegisterModel>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var json = JsonSerializer.Serialize(registerModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            var responseJson = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(authResponse);
            Assert.Equal(expectedSuccess, authResponse.Success);
            Assert.Equal(expectedMessage, authResponse.Message);

            if (expectedSuccess)
            {
                Assert.NotNull(authResponse.Token);
                Assert.NotNull(authResponse.User);
            }

            // Verify service was called
            _mockAuthService.Verify(x => x.RegisterAsync(It.IsAny<RegisterModel>()), Times.Once);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_WithValidToken_ReturnsOk()
        {
            // Arrange
            var token = "valid.token.here";
            _mockAuthService
                .Setup(x => x.LogoutAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var json = JsonSerializer.Serialize(token);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/logout", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(responseJson);
            Assert.NotNull(result);

            // Verify service was called
            _mockAuthService.Verify(x => x.LogoutAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Logout_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var invalidToken = "invalid.token.here";
            _mockAuthService
                .Setup(x => x.LogoutAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var json = JsonSerializer.Serialize(invalidToken);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/logout", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Verify service was called
            _mockAuthService.Verify(x => x.LogoutAsync(It.IsAny<string>()), Times.Once);
        }

        #endregion

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}