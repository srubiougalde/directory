using System.Threading.Tasks;
using DirectoryApi.Entities;
using DirectoryApi.Models;

namespace DirectoryApi.Services
{
    public interface IAccountsService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> AuthenticateWithCode(AuthenticateCodeRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        Task<bool> RevokeToken(string token, string ipAddress);
        Task RegisterUser(User user);
        string SuggestPassword();
    }
}