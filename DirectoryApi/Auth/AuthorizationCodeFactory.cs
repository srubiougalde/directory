using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DirectoryApi.Auth
{
    public class AuthorizationCodeFactory : IAuthorizationCodeFactory
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit

        public Task<string> GenerateAuthorizationCode(string userId)
        {
            var id = userId.Replace("-", string.Empty);
            using (var algorithm = new Rfc2898DeriveBytes(id, SaltSize))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return Task.FromResult($"{salt}.{key}");
            }
        }

        public Task<bool> VerifyAuthorizationCode(string hash, string userId)
        {
            var parts = hash.Split('.', 2);

            if (parts.Length != 2)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{salt}.{hash}`");
            }

            var salt = Convert.FromBase64String(parts[0].Replace(" ", "+"));
            var key = Convert.FromBase64String(parts[1].Replace(" ", "+"));

            var id = userId.Replace("-", string.Empty);
            using (var algorithm = new Rfc2898DeriveBytes(
              id,
              salt))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);

                var verified = keyToCheck.SequenceEqual(key);

                return Task.FromResult(verified);
            }
        }
    }
}