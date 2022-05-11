using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DirectoryApi.Entities;
using DirectoryApi.Models;

namespace DirectoryApi.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByRefreshTokenAsync(string token);
        Task<Guid> CreateUserAsync(User user);
        Task UpdateUserAsync(User dbUser, User user);
        Task DeleteUserAsync(User user);
    }
}