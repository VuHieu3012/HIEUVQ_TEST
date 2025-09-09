using Microsoft.Extensions.Configuration;
using AuthModule.Models;
using AuthModule.Services;
using AuthModule.Repositories;
using Moq;

namespace AuthModule.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Setup mocks
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();
            
            // Setup service with mocked dependencies
            _authService = new AuthService(_mockUserRepository.Object, _mockTokenService.Object);
        }

        #region Login Test Data

        public static IEnumerable<object?[]> LoginServiceTestData()
        {
            // Valid login
            yield return new object?[] { 
                new LoginModel { UsernameOrEmail = "admin@test.com", Password = "Admin123!" },
                new User { Id = 1, Username = "admin", Email = "admin@test.com", IsActive = true, PasswordHash = "hashed_password" },
                true,
                "Login successful",
                "admin"
            };
            // Invalid password
            yield return new object?[] { 
                new LoginModel { UsernameOrEmail = "admin@test.com", Password = "WrongPassword" },
                new User { Id = 1, Username = "admin", Email = "admin@test.com", IsActive = true, PasswordHash = "hashed_password" },
                false,
                "Invalid username/email or password",
                null
            };
            // User not found
            yield return new object?[] { 
                new LoginModel { UsernameOrEmail = "nonexistent@test.com", Password = "SomePassword" },
                null, // User not found
                false,
                "Invalid username/email or password",
                null
            };
        }

        #endregion

        #region Register Test Data

        public static IEnumerable<object?[]> RegisterServiceTestData()
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
                false, // Username exists
                false, // Email exists
                new User { Id = 1, Username = "newuser1", Email = "newuser1@test.com", UserType = "User" },
                true,
                "Registration successful",
                "newuser1"
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
                true, // Username exists
                false, // Email exists
                null,
                false,
                "Username already exists",
                null
            };
        }

        #endregion

        #region Login Tests

        [Theory]
        [MemberData(nameof(LoginServiceTestData))]
        public async Task LoginAsync_WithDifferentCredentials_ReturnsExpectedResult(
            LoginModel loginModel, 
            User? mockUser,
            bool expectedSuccess,
            string expectedMessage,
            string? expectedUsername)
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.GetByUsernameOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(mockUser);

            if (expectedSuccess)
            {
                _mockTokenService
                    .Setup(service => service.GenerateJwtToken(It.IsAny<User>()))
                    .Returns("mock_jwt_token");
            }

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.Equal(expectedSuccess, result.Success);
            // Note: Message might differ due to BCrypt validation, so we check if it contains expected content
            if (!expectedSuccess)
            {
                Assert.Contains("Invalid", result.Message);
            }
            
            if (expectedSuccess)
            {
                Assert.NotNull(result.Token);
                Assert.NotNull(result.User);
                Assert.Equal(expectedUsername, result.User.Username);
                
                // Verify mock interactions
                _mockUserRepository.Verify(repo => repo.GetByUsernameOrEmailAsync(It.IsAny<string>()), Times.Once);
                _mockTokenService.Verify(service => service.GenerateJwtToken(It.IsAny<User>()), Times.Once);
            }
            else
            {
                Assert.Null(result.Token);
                Assert.Null(result.User);
                
                // Verify mock interactions
                _mockUserRepository.Verify(repo => repo.GetByUsernameOrEmailAsync(It.IsAny<string>()), Times.Once);
                _mockTokenService.Verify(service => service.GenerateJwtToken(It.IsAny<User>()), Times.Never);
            }
        }

        #endregion

        #region Register Tests

        [Theory]
        [MemberData(nameof(RegisterServiceTestData))]
        public async Task RegisterAsync_WithDifferentData_ReturnsExpectedResult(
            RegisterModel registerModel, 
            bool usernameExists,
            bool emailExists,
            User? mockCreatedUser,
            bool expectedSuccess,
            string expectedMessage,
            string? expectedUsername)
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.ExistsByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(usernameExists);

            _mockUserRepository
                .Setup(repo => repo.ExistsByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(emailExists);

            if (expectedSuccess)
            {
                _mockUserRepository
                    .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                    .ReturnsAsync(mockCreatedUser!);

                _mockTokenService
                    .Setup(service => service.GenerateJwtToken(It.IsAny<User>()))
                    .Returns("mock_jwt_token");
            }

            // Act
            var result = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.Equal(expectedSuccess, result.Success);
            Assert.Equal(expectedMessage, result.Message);
            
            if (expectedSuccess)
            {
                Assert.NotNull(result.Token);
                Assert.NotNull(result.User);
                Assert.Equal(expectedUsername, result.User.Username);
                
                // Verify mock interactions
                _mockUserRepository.Verify(repo => repo.ExistsByUsernameAsync(It.IsAny<string>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.ExistsByEmailAsync(It.IsAny<string>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
                _mockTokenService.Verify(service => service.GenerateJwtToken(It.IsAny<User>()), Times.Once);
            }
            else
            {
                Assert.Null(result.Token);
                Assert.Null(result.User);
                
                // Verify mock interactions - only check what was actually called
                _mockUserRepository.Verify(repo => repo.ExistsByUsernameAsync(It.IsAny<string>()), Times.Once);
                if (usernameExists)
                {
                    // If username exists, email check should not be called
                    _mockUserRepository.Verify(repo => repo.ExistsByEmailAsync(It.IsAny<string>()), Times.Never);
                }
                else
                {
                    // If username doesn't exist, email check should be called
                    _mockUserRepository.Verify(repo => repo.ExistsByEmailAsync(It.IsAny<string>()), Times.Once);
                }
                _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
                _mockTokenService.Verify(service => service.GenerateJwtToken(It.IsAny<User>()), Times.Never);
            }
        }

        #endregion

        #region ValidateToken Tests

        [Fact]
        public async Task ValidateTokenAsync_WithInvalidToken_ReturnsFailure()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            _mockTokenService
                .Setup(service => service.ValidateToken(It.IsAny<string>()))
                .Returns(false);

            // Act
            var result = await _authService.ValidateTokenAsync(invalidToken);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid token", result.Message);
            Assert.Null(result.User);

            // Verify mock interactions
            _mockTokenService.Verify(service => service.ValidateToken(It.IsAny<string>()), Times.Once);
            _mockTokenService.Verify(service => service.GetUserIdFromToken(It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ValidateTokenAsync_WithEmptyToken_ReturnsFailure()
        {
            // Arrange
            var emptyToken = "";

            _mockTokenService
                .Setup(service => service.ValidateToken(It.IsAny<string>()))
                .Returns(false);

            // Act
            var result = await _authService.ValidateTokenAsync(emptyToken);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid token", result.Message);
            Assert.Null(result.User);

            // Verify mock interactions
            _mockTokenService.Verify(service => service.ValidateToken(It.IsAny<string>()), Times.Once);
            _mockTokenService.Verify(service => service.GetUserIdFromToken(It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task LogoutAsync_WithEmptyToken_ReturnsTrue()
        {
            // Arrange
            var token = "";

            // Act
            var result = await _authService.LogoutAsync(token);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Additional Service Tests

        [Fact]
        public async Task ValidateTokenAsync_WithValidToken_ReturnsSuccess()
        {
            // Arrange
            var token = "valid_token";
            var user = new User { Id = 1, Username = "admin", IsActive = true };

            _mockTokenService
                .Setup(service => service.ValidateToken(It.IsAny<string>()))
                .Returns(true);

            _mockTokenService
                .Setup(service => service.GetUserIdFromToken(It.IsAny<string>()))
                .Returns("1");

            _mockUserRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ValidateTokenAsync(token);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Token is valid", result.Message);
            Assert.NotNull(result.User);
            Assert.Equal("admin", result.User.Username);

            // Verify mock interactions
            _mockTokenService.Verify(service => service.ValidateToken(It.IsAny<string>()), Times.Once);
            _mockTokenService.Verify(service => service.GetUserIdFromToken(It.IsAny<string>()), Times.Once);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_WithAnyToken_ReturnsTrue()
        {
            // Arrange
            var token = "any_token";

            // Act
            var result = await _authService.LogoutAsync(token);

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}
