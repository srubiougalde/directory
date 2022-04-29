using DirectoryApi.Entities;
using DirectoryApi.Helpers;
using DirectoryApi.Repositories;

namespace DirectoryApi.Services;

public class WebsiteService : IWebsiteService
{
    private readonly IWebsiteRepository _websiteRepository;

    public WebsiteService(IWebsiteRepository websiteRepository)
    {
        _websiteRepository = websiteRepository;
    }

    public Task<Website> GetWebsiteByIdAsync(int id)
    {
        return _websiteRepository.GetWebsiteByIdAsync(id);
    }

    public async Task<Website> SyncWebsiteHeadingAsync(Website website)
    {
        website.Headings = FeedHelper.ReadHeadings(website.Url).Select(x => new WebsiteHeading
        {
            Name = x.OriginalName,
            InnerText = x.InnerText
        }).ToList();

        return await _websiteRepository.UpdateWebsiteAsync(website, website);
    }
}