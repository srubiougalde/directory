using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace DirectoryApi.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public ClaimsIdentity GenerateClaimsIdentity(string userName, Guid id, string firstName, string lastName, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, id.ToString()),
            };

            switch (role)
            {
                case Helpers.Constants.Strings.JwtClaims.SuperAdministrator:
                    claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.SuperAdministrator));
                    break;
                case Helpers.Constants.Strings.JwtClaims.Administrator:
                    claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.Administrator));
                    break;
                case Helpers.Constants.Strings.JwtClaims.Client:
                    claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.Client));
                    break;
                default:
                    claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.ApiAccess));
                    break;
            }

            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), claims);
        }

        public async Task<string> GenerateEncodedToken(string userName, string firstName, string lastName, ClaimsIdentity identity)
        {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.GivenName, firstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
                identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)
             };
            claims.AddRange(identity.FindAll(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles));

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}