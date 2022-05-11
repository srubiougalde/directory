using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DirectoryApi.Entities;
using DirectoryApi.Models;

namespace DirectoryApi.Repositories
{
    public interface IUsersRepository : IBaseRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByRefreshTokenAsync(string token);
        Task<Guid> CreateUserAsync(User user);
        Task UpdateUserAsync(User dbUser, User user);
        Task UpdateUserPasswordAsync(User user, string password);
        Task DeleteUserAsync(User user);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task AddRefreshTokensAsync(User user, RefreshToken refreshToken);
        Task RevokeRefreshTokensAsync(User user, string token, string ipAddress);
    }
}