
namespace DirectoryApi.Models;

public class GetMemberResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? WebsiteShortUrl { get; set; }
    public int FriendsCount { get; set; }
}