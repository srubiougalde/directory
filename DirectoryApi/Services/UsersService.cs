using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DirectoryApi.Auth;
using DirectoryApi.Entities;
using DirectoryApi.Models;
using DirectoryApi.Repositories;
using Microsoft.Extensions.Configuration;

namespace DirectoryApi.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IRolesRepository _roleRepository;
        private readonly IAuthorizationCodeFactory _authCodeFactory;
        private readonly IConfiguration _config;

        public UsersService(
            IUsersRepository userRepository,
            IRolesRepository roleRepository,
            IAuthorizationCodeFactory authCodeFactory,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _authCodeFactory = authCodeFactory;
            _config = config;
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return _userRepository.GetAllUsersAsync();
        }

        public Task<User> GetUserByIdAsync(Guid userId)
        {
            return _userRepository.GetUserByIdAsync(userId);
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            return _userRepository.GetUserByEmailAsync(email);
        }

        public Task<User> GetUserByRefreshTokenAsync(string token)
        {
            return _userRepository.GetUserByRefreshTokenAsync(token);
        }

        public Task<Guid> CreateUserAsync(User user)
        {
            return _userRepository.CreateUserAsync(user);
        }

        public Task UpdateUserAsync(User dbUser, User user)
        {
            return _userRepository.UpdateUserAsync(dbUser, user);
        }

        public Task DeleteUserAsync(User user)
        {
            return _userRepository.DeleteUserAsync(user);
        }
    }
}