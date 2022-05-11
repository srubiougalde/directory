using System.Threading.Tasks;

namespace DirectoryApi.Auth
{
    public interface IAuthorizationCodeFactory
    {
        Task<string> GenerateAuthorizationCode(string userId);
        Task<bool> VerifyAuthorizationCode(string hash, string userId);
    }
}