
namespace DirectoryApi.Models;

public class GetFriendshipResponse
{
    public GetFriendshipResponse()
    {
        Friends = new List<GetFriendshipResponse>();
        Headings = new List<(string Name, string Text)>();
    }

    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? WebsiteShortUrl { get; set; }
    public List<(string Name, string Text)> Headings { get; set; }
    public ICollection<GetFriendshipResponse> Friends { get; set; }
}