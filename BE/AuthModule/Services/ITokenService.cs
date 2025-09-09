using AuthModule.DTOs;

namespace AuthModule.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        string? GetUserIdFromToken(string token);
        string? GetUserTypeFromToken(string token);
    }
}
