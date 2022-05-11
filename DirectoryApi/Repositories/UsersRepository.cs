using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using DirectoryApi.Entities;
using DirectoryApi.Models;

namespace DirectoryApi.Repositories
{
    public class UsersRepository : BaseRepository<User>, IUsersRepository
    {
        private PasswordHasher _hasher { get; }

        public UsersRepository(ApplicationDbContext context)
            : base(context)
        {
            _hasher = new PasswordHasher(options: new OptionsWrapper<HashingOptions>(new HashingOptions { Iterations = 1000 }),
                                        passwordOptions: new OptionsWrapper<StrongPasswordOptions>(new StrongPasswordOptions()));
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await FindAll()
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.Role).AsNoTracking()
                        .ToListAsync();
        }
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await FindByCondition(user => user.Id.Equals(userId))
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.Role).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await FindByCondition(user => user.Profile.Email.ToLower().Equals(email.ToLower()))
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.Role).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByRefreshTokenAsync(string token)
        {
            return await FindByCondition(user => user.RefreshTokens.Any(t => t.Token == token))
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.Role).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task<Guid> CreateUserAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.IsActive = true;
            user.CreatedAt = user.ModifiedAt = DateTime.UtcNow;

            user.Profile.Id = Guid.NewGuid();
            user.Profile.Password = !string.IsNullOrEmpty(user.Profile.Password) ? _hasher.Hash(user.Profile.Password) : string.Empty;

            DetachLocal(user, p => p.Id.Equals(user.Id));
            Create(user);
            await SaveAsync();

            return user.Id;
        }

        public async Task UpdateUserAsync(User dbUser, User user)
        {
            dbUser.IsActive = user.IsActive;
            dbUser.ModifiedAt = DateTime.UtcNow;

            dbUser.Profile.FirstName = user.Profile.FirstName;
            dbUser.Profile.LastName = user.Profile.LastName;
            dbUser.Profile.Password = !string.IsNullOrEmpty(user.Profile.Password) ? _hasher.Hash(user.Profile.Password) : dbUser.Profile.Password;
            dbUser.RoleId = user.RoleId;

            DetachLocal(dbUser, p => p.Id.Equals(dbUser.Id));
            Update(dbUser);
            await SaveAsync();
        }

        public async Task UpdateUserPasswordAsync(User user, string newPassword)
        {
            user.Profile.Password = !string.IsNullOrEmpty(newPassword) ? _hasher.Hash(newPassword) : string.Empty;
            user.ModifiedAt = DateTime.UtcNow;

            DetachLocal(user, p => p.Id.Equals(user.Id));
            Update(user);

            await SaveAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            DetachLocal(user, p => p.Id.Equals(user.Id));
            Delete(user);
            await SaveAsync();
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var userdb = await FindByCondition(user => user.IsActive && user.Profile.Email.Equals(email))
                        .Include(p => p.Profile).AsNoTracking()
                        .SingleOrDefaultAsync();

            if (userdb == null || userdb.Profile == null)
            {
                return await Task.FromResult<User>(null);
            }

            var result = _hasher.Check(userdb.Profile.Password, password);

            if (result.Verified)
            {
                userdb.FailedLoginAttempts = 0;
                userdb.SuccessfulLoginAttempts++;
                userdb.LastLoggedIn = DateTime.UtcNow;

                DetachLocal(userdb, p => p.Id.Equals(userdb.Id));
                Update(userdb);
                await SaveAsync();

                userdb = await FindByCondition(user => user.IsActive && user.Profile.Email.Equals(email))
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.Role).AsNoTracking()
                        .SingleOrDefaultAsync();

                return await Task.FromResult<User>(userdb);
            }

            userdb.FailedLoginAttempts++;

            DetachLocal(userdb, p => p.Id.Equals(userdb.Id));
            Update(userdb);
            await SaveAsync();

            return await Task.FromResult<User>(null);
        }

        public async Task AddRefreshTokensAsync(User user, RefreshToken refreshToken)
        {
            var userdb = await FindByCondition(x => x.Id.Equals(user.Id)).AsNoTracking().SingleOrDefaultAsync();

            userdb.RefreshTokens.Add(refreshToken);

            DetachLocal(user, p => p.Id.Equals(user.Id));
            Update(userdb);
            await SaveAsync();
        }

        public async Task RevokeRefreshTokensAsync(User user, string token, string ipAddress)
        {
            var userdb = await FindByCondition(x => x.Id.Equals(user.Id)).AsNoTracking().SingleOrDefaultAsync();

            var refreshToken = userdb.RefreshTokens.Single(x => x.Token == token);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            DetachLocal(user, p => p.Id.Equals(user.Id));
            Update(userdb);
            await SaveAsync();
        }
    }
}