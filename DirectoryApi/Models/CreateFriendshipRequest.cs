using System.ComponentModel.DataAnnotations;

namespace DirectoryApi.Models;

public class CreateFriendshipRequest
{
    [Display(Name = "FriendId")]
    [Required(ErrorMessage = "Website Url is required")]
    public Guid FriendId { get; set; }
}