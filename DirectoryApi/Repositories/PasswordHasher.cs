using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace DirectoryApi.Repositories
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit

        private HashingOptions Options { get; }
        private StrongPasswordOptions StrongPasswordOptions { get; }

        public PasswordHasher(IOptions<HashingOptions> options, IOptions<StrongPasswordOptions> passwordOptions)
        {
            Options = options.Value;
            StrongPasswordOptions = passwordOptions.Value;
        }

        public (bool Verified, bool NeedsUpgrade) Check(string hash, string password)
        {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var needsUpgrade = iterations != Options.Iterations;

            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA256))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);

                var verified = keyToCheck.SequenceEqual(key);

                return (verified, needsUpgrade);
            }
        }

        public string Hash(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
                password,
                SaltSize,
                Options.Iterations,
                HashAlgorithmName.SHA256))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{Options.Iterations}.{salt}.{key}";
            }
        }

        public string GeneratePassword()
        {
            int length = StrongPasswordOptions.RequiredLength;

            bool nonAlphanumeric = StrongPasswordOptions.RequireNonAlphanumeric;
            bool digit = StrongPasswordOptions.RequireDigit;
            bool lowercase = StrongPasswordOptions.RequireLowercase;
            bool uppercase = StrongPasswordOptions.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char n = (char)random.Next(33, 123);
                if ((n > 57 && n < 63) || (n > 90 && n < 97) || (n < 48 && (n != 33 || n != 35 || n != 36)))
                {
                    continue;
                }

                char c = (char)n;
                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)33);
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }
    }

    public sealed class HashingOptions
    {
        public int Iterations { get; set; } = 10000;
    }

    public sealed class StrongPasswordOptions
    {
        public int RequiredLength { get; set; } = 8;
        public bool RequireNonAlphanumeric { get; set; } = true;
        public bool RequireDigit { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireUppercase { get; set; } = true;
    }
}
