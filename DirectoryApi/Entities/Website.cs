using Microsoft.EntityFrameworkCore;

namespace DirectoryApi.Entities;

public class Website
{
    public Website()
    {
        Headings = new HashSet<WebsiteHeading>();
    }

    public int Id { get; set; }
    public Guid MemberId { get; set; }
    public string? Url { get; set; }
    public string? ShortUrl { get; set; }
    public ICollection<WebsiteHeading> Headings { get; set; }
}