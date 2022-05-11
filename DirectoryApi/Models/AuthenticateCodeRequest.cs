using System.ComponentModel.DataAnnotations;

namespace DirectoryApi.Models
{
    public class AuthenticateCodeRequest
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}