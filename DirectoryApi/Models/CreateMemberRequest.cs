using System.ComponentModel.DataAnnotations;

namespace DirectoryApi.Models;

public class CreateMemberRequest
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "{0} is required")]
    public string? Name { get; set; }

    [Display(Name = "WebsiteUrl")]
    [Required(ErrorMessage = "Website Url is required")]
    [Url]
    public string? WebsiteUrl { get; set; }
}