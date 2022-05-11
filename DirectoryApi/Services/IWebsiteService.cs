using DirectoryApi.Entities;

namespace DirectoryApi.Services;

public interface IWebsiteService
{
    Task<Website> GetWebsiteByIdAsync(int id);
    Task<Website> GetWebsiteByShortUrlAsync(string shortUrl);
    Task<Website> SyncWebsiteHeadingAsync(Website website);
    Task<List<Website>> GetWebsiteByTopicAsync(string query);
}