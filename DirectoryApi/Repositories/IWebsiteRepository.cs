using DirectoryApi.Entities;

namespace DirectoryApi.Repositories;

public interface IWebsiteRepository : IBaseRepository<Website>
{
    Task<Website> GetWebsiteByIdAsync(int id);
    Task<Website> UpdateWebsiteAsync(Website dbWebsite, Website inputWebsite);
    Task<List<Website>> GetWebsiteByTopicAsync(string query);
}