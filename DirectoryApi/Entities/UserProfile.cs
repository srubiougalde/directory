using System;
using System.Text.Json.Serialization;

namespace DirectoryApi.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}