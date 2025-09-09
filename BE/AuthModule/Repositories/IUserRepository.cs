using AuthModule.DTOs;

namespace AuthModule.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}
