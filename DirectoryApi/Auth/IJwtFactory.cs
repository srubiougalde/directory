using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DirectoryApi.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, string firstName, string lastName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string userName, Guid id, string firstName, string lastName, string role);
    }
}