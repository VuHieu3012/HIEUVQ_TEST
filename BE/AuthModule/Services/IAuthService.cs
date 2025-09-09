using AuthModule.Models;
using AuthModule.DTOs;

namespace AuthModule.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginModel loginModel);
        Task<AuthResponse> RegisterAsync(RegisterModel registerModel);
        Task<AuthResponse> ValidateTokenAsync(string token);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string token);
        bool IsAdmin(string token);
    }
}
