using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;
using DirectoryApi.Auth;
using DirectoryApi.Entities;
using DirectoryApi.Repositories;
using DirectoryApi.Helpers;
using DirectoryApi.Models;
using System;
using System.IO;

namespace DirectoryApi.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IAuthorizationCodeFactory _authCodeFactory;
        private readonly IConfiguration _config;
        private PasswordHasher _hasher { get; }

        public AccountsService(
            IUsersRepository userRepository,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IAuthorizationCodeFactory authCodeFactory,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _authCodeFactory = authCodeFactory;
            _config = config;
            _hasher = new PasswordHasher(options: new OptionsWrapper<HashingOptions>(new HashingOptions()),
                                        passwordOptions: new OptionsWrapper<StrongPasswordOptions>(new StrongPasswordOptions()));
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = _userRepository.AuthenticateUserAsync(model.Username.ToLower(), model.Password).Result;

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            var identity = _jwtFactory.GenerateClaimsIdentity(user.Profile.Email, user.Id, user.Profile.FirstName, user.Profile.LastName, user.Role.Name);
            var userId = identity.Claims.Single(c => c.Type == "id").Value;
            var expiresIn = (int)_jwtOptions.ValidFor.TotalSeconds;
            var accessToken = await Tokens.GenerateJwt(identity, _jwtFactory, user.Profile.Email, user.Profile.FirstName, user.Profile.LastName);
            var refreshToken = Tokens.GenerateRefreshToken(ipAddress);

            await _userRepository.AddRefreshTokensAsync(user, refreshToken);

            return new AuthenticateResponse(userId, accessToken, refreshToken.Token, expiresIn);
        }

        public async Task<AuthenticateResponse> AuthenticateWithCode(AuthenticateCodeRequest model, string ipAddress)
        {
            if (!await _authCodeFactory.VerifyAuthorizationCode(model.Code, model.UserId))
            {
                return null;
            }

            var user = _userRepository.GetUserByIdAsync(new Guid(model.UserId)).Result;

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            var identity = _jwtFactory.GenerateClaimsIdentity(user.Profile.Email, user.Id, user.Profile.FirstName, user.Profile.LastName, user.Role.Name);
            var expiresIn = (int)_jwtOptions.ValidFor.TotalSeconds;
            var accessToken = await Tokens.GenerateJwt(identity, _jwtFactory, user.Profile.Email, user.Profile.FirstName, user.Profile.LastName);
            var refreshToken = Tokens.GenerateRefreshToken(ipAddress);

            await _userRepository.AddRefreshTokensAsync(user, refreshToken);

            return new AuthenticateResponse(model.UserId, accessToken, refreshToken.Token, expiresIn);
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(token);

            // return null if no user found with token
            if (user == null)
            {
                return null;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive)
            {
                return null;
            }

            // generate new jwt
            var identity = _jwtFactory.GenerateClaimsIdentity(user.Profile.Email, user.Id, user.Profile.FirstName, user.Profile.LastName, user.Role.Name);
            var userId = identity.Claims.Single(c => c.Type == "id").Value;
            var expiresIn = (int)_jwtOptions.ValidFor.TotalSeconds;
            var jwtToken = await Tokens.GenerateJwt(identity, _jwtFactory, user.Profile.Email, user.Profile.FirstName, user.Profile.LastName);

            // replace old refresh token with a new one and save
            var newRefreshToken = Tokens.GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            await _userRepository.AddRefreshTokensAsync(user, newRefreshToken);

            return new AuthenticateResponse(userId, jwtToken, newRefreshToken.Token, expiresIn);
        }

        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(token);

            // return false if no user found with token
            if (user == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive)
            {
                return false;
            }

            // revoke token and save
            await _userRepository.RevokeRefreshTokensAsync(user, token, ipAddress);

            return true;
        }

        public async Task RegisterUser(User user)
        {
            await _userRepository.CreateUserAsync(user);
        }

        public async Task UpdateUserPassword(User user, string newPassword)
        {
            // return null if user not found
            if (user == null)
            {
                return;
            }

            await _userRepository.UpdateUserPasswordAsync(user, newPassword);
        }

        public async Task ResetUserPassword(User user, string code, string newPassword)
        {
            // return null if user not found
            if (user == null)
            {
                return;
            }

            if (!await _authCodeFactory.VerifyAuthorizationCode(code, user.Id.ToString()))
            {
                return;
            }

            await _userRepository.UpdateUserPasswordAsync(user, newPassword);
        }

        public string SuggestPassword()
        {
            return _hasher.GeneratePassword();
        }
    }
}