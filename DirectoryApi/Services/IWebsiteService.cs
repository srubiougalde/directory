using DirectoryApi.Entities;

namespace DirectoryApi.Services;

public interface IWebsiteService
{
    Task<Website> GetWebsiteByIdAsync(int id);
    Task<Website> SyncWebsiteHeadingAsync(Website website);
}