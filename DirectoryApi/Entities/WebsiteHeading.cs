using Microsoft.EntityFrameworkCore;

namespace DirectoryApi.Entities;

public class WebsiteHeading
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? InnerText { get; set; }
}