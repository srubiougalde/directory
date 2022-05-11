using DirectoryApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryApi.Repositories;

public class WebsiteRepository : BaseRepository<Website>, IWebsiteRepository
{
    public WebsiteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Website> GetWebsiteByIdAsync(int id)
    {
        return await FindByCondition(x => x.Id == id)
            .SingleOrDefaultAsync();
    }
    public async Task<Website> UpdateWebsiteAsync(Website dbWebsite, Website inputWebsite)
    {
        dbWebsite.Url = inputWebsite.Url;
        dbWebsite.ShortUrl = inputWebsite.ShortUrl;
        dbWebsite.Headings = inputWebsite.Headings;

        Update(dbWebsite);
        await SaveAsync();

        return dbWebsite;
    }

    public async Task<List<Website>> GetWebsiteByTopicAsync(string query)
    {
        return await FindByCondition(x => x.Headings.Any(h => h.InnerText.Equals(query, StringComparison.OrdinalIgnoreCase)))
                            .ToListAsync();
    }
}