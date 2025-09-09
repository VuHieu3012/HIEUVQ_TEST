using Microsoft.Extensions.Configuration;
using AuthModule.Models;
using AuthModule.Services;

namespace AuthModule.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;

        public TokenServiceTests()
        {
            // Setup configuration
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey", "YourSuperSecretKeyThatIsAtLeast32CharactersLong!" },
                { "JwtSettings:Issuer", "AuthModule" },
                { "JwtSettings:Audience", "AuthModuleUsers" },
                { "JwtSettings:ExpiryMinutes", "60" }
            });
            _configuration = configBuilder.Build();

            _tokenService = new TokenService(_configuration);
        }

        #region GenerateJwtToken Test Data

        public static IEnumerable<object?[]> GenerateTokenTestData()
        {
            // Regular user
            yield return new object?[] { 
                new User
                {
                    Id = 1,
                    Username = "testuser",
                    Email = "test@test.com",
                    UserType = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                "testuser",
                "User"
            };
        }

        #endregion

        #region GenerateJwtToken Tests

        [Theory]
        [MemberData(nameof(GenerateTokenTestData))]
        public void GenerateJwtToken_WithDifferentUsers_ReturnsValidToken(
            User user, 
            string expectedUsername,
            string expectedUserType)
        {
            // Act
            var token = _tokenService.GenerateJwtToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Contains(".", token); // JWT format check
            
            // Verify token contains user information
            var userId = _tokenService.GetUserIdFromToken(token);
            Assert.Equal(user.Id.ToString(), userId);
        }


        #endregion

        #region ValidateToken Test Data

        public static IEnumerable<object?[]> ValidateTokenTestData()
        {
            // Valid token
            yield return new object?[] { 
                "valid.token.here", // Will be replaced with actual token
                true
            };
            // Invalid token
            yield return new object?[] { 
                "invalid.token.here",
                false
            };
        }

        #endregion

        #region ValidateToken Tests

        [Theory]
        [MemberData(nameof(ValidateTokenTestData))]
        public void ValidateToken_WithDifferentTokens_ReturnsExpectedResult(
            string? token, 
            bool expectedResult)
        {
            // Arrange
            if (token == "valid.token.here")
            {
                var user = new User
                {
                    Id = 1,
                    Username = "testuser",
                    Email = "test@test.com",
                    UserType = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                token = _tokenService.GenerateJwtToken(user);
            }

            // Act
            var isValid = _tokenService.ValidateToken(token!);

            // Assert
            Assert.Equal(expectedResult, isValid);
        }

        #endregion

        #region GetUserIdFromToken Tests

        [Fact]
        public void GetUserIdFromToken_WithValidToken_ReturnsUserId()
        {
            // Arrange
            var user = new User
            {
                Id = 123,
                Username = "testuser",
                Email = "test@test.com",
                UserType = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var token = _tokenService.GenerateJwtToken(user);

            // Act
            var userId = _tokenService.GetUserIdFromToken(token);

            // Assert
            Assert.NotNull(userId);
            Assert.Equal("123", userId);
        }

        [Fact]
        public void GetUserIdFromToken_WithInvalidToken_ReturnsNull()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var userId = _tokenService.GetUserIdFromToken(invalidToken);

            // Assert
            Assert.Null(userId);
        }

        #endregion
    }
}
