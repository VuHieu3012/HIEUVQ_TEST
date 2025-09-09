using AuthModule.Models;
using AuthModule.DTOs;
using AuthModule.Repositories;
using BCrypt.Net;

namespace AuthModule.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginModel loginModel)
        {
            try
            {
                // Find user by username or email
                var user = await _userRepository.GetByUsernameOrEmailAsync(loginModel.UsernameOrEmail);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid username/email or password"
                    };
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Account is deactivated"
                    };
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid username/email or password"
                    };
                }

                // Generate JWT token
                var token = _tokenService.GenerateJwtToken(user);

                // Generate refresh token only if Remember Me is checked
                string? refreshToken = null;
                DateTime? refreshTokenExpiry = null;
                
                if (loginModel.RememberMe)
                {
                    refreshToken = _tokenService.GenerateRefreshToken();
                    refreshTokenExpiry = DateTime.UtcNow.AddDays(30); // 30 days for remember me
                    
                    // Update user with refresh token
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiry = refreshTokenExpiry;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                }

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    RefreshToken = refreshToken, // null if not Remember Me
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // 1 hour
                    User = user
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                // Check if username already exists
                if (await _userRepository.ExistsByUsernameAsync(registerModel.Username))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                // Check if email already exists
                if (await _userRepository.ExistsByEmailAsync(registerModel.Email))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                }

                // Create new user
                var user = new User
                {
                    Username = registerModel.Username,
                    Email = registerModel.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
                    UserType = registerModel.UserType,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateAsync(user);

                // Generate JWT token
                var token = _tokenService.GenerateJwtToken(createdUser);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // 1 hour
                    User = createdUser
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> ValidateTokenAsync(string token)
        {
            try
            {
                if (!_tokenService.ValidateToken(token))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                var userId = _tokenService.GetUserIdFromToken(token);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || !user.IsActive)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "User not found or inactive"
                    };
                }

                return new AuthResponse
                {
                    Success = true,
                    Message = "Token is valid",
                    User = user
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Token validation failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Find user by refresh token
                var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    };
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Account is deactivated"
                    };
                }

                // Check if refresh token is expired
                if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Refresh token has expired"
                    };
                }

                // Generate new JWT token
                var newToken = _tokenService.GenerateJwtToken(user);

                // Generate new refresh token
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                // Update user with new refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 days
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // 1 hour
                    User = user
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Token refresh failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> LogoutAsync(string token)
        {
            try
            {
                // In a real application, you might want to blacklist the token
                // For now, we'll just return true as JWT tokens are stateless
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        public bool IsAdmin(string token)
        {
            try
            {
                if (!_tokenService.ValidateToken(token)) return false;
                var userType = _tokenService.GetUserTypeFromToken(token);
                return userType == "Admin";
            }
            catch
            {
                return false;
            }
        }

    }
}
