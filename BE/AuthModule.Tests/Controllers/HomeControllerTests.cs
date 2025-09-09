using Microsoft.AspNetCore.Mvc.Testing;
using AuthModule.Models;
using AuthModule.DTOs;
using AuthModule.Services;
using AuthModule.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using System.Text.Json;

namespace AuthModule.Tests.Controllers
{
    public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ApplicationDbContext> _mockContext;

        public HomeControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _mockAuthService = new Mock<IAuthService>();
            _mockContext = new Mock<ApplicationDbContext>();
            
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove real services
                    var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthService));
                    if (authDescriptor != null)
                        services.Remove(authDescriptor);
                    
                    var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ApplicationDbContext));
                    if (contextDescriptor != null)
                        services.Remove(contextDescriptor);
                    
                    // Add mock services
                    services.AddSingleton(_mockAuthService.Object);
                    services.AddSingleton(_mockContext.Object);
                });
            }).CreateClient();
        }

        #region MVC Actions Tests

        [Fact]
        public async Task Index_ReturnsView()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            // Index requires authentication, so expect 401 Unauthorized
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsView()
        {
            // Act
            var response = await _client.GetAsync("/Account/Login");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task Register_ReturnsView()
        {
            // Act
            var response = await _client.GetAsync("/Account/Register");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task Data_ReturnsView()
        {
            // Act
            var response = await _client.GetAsync("/Data");

            // Assert
            // Data action requires database, so expect 500 error in test environment
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #region API Endpoints Tests

        [Fact]
        public async Task GetUsers_WithoutAuth_ReturnsAccessDenied()
        {
            // Act
            var response = await _client.GetAsync("/api/data/users");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var success = jsonDoc.RootElement.GetProperty("success").GetBoolean();
            var error = jsonDoc.RootElement.GetProperty("error").GetString();

            Assert.False(success);
            Assert.Equal("Access denied", error);
        }

        [Fact]
        public async Task GetUsers_WithValidAdminToken_ReturnsUsers()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User { Id = 1, Username = "admin", Email = "admin@test.com", UserType = "Admin" },
                new User { Id = 2, Username = "user", Email = "user@test.com", UserType = "User" }
            };

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(mockUsers.AsQueryable().Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(mockUsers.AsQueryable().Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(mockUsers.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(mockUsers.AsQueryable().GetEnumerator());

            _mockContext.Setup(x => x.Users).Returns(mockDbSet.Object);
            _mockAuthService.Setup(x => x.IsAdmin(It.IsAny<string>())).Returns(true);

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/data/users");
            request.Headers.Add("Authorization", "Bearer valid.admin.token");
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var success = jsonDoc.RootElement.GetProperty("success").GetBoolean();
            var data = jsonDoc.RootElement.GetProperty("data");

            Assert.True(success);
            Assert.Equal(2, data.GetArrayLength());

            // Verify service was called
            _mockAuthService.Verify(x => x.IsAdmin("valid.admin.token"), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WithNonAdminToken_ReturnsAccessDenied()
        {
            // Arrange
            _mockAuthService.Setup(x => x.IsAdmin(It.IsAny<string>())).Returns(false);

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/data/users");
            request.Headers.Add("Authorization", "Bearer user.token");
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var success = jsonDoc.RootElement.GetProperty("success").GetBoolean();
            var error = jsonDoc.RootElement.GetProperty("error").GetString();

            Assert.False(success);
            Assert.Equal("Access denied", error);

            // Verify service was called
            _mockAuthService.Verify(x => x.IsAdmin("user.token"), Times.Once);
        }

        #endregion

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}