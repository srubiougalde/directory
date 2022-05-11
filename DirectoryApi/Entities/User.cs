using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DirectoryApi.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public int FailedLoginAttempts { get; set; }
        public int SuccessfulLoginAttempts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; }

        //-----------------------------
        //Relationships
        public UserProfile Profile { get; set; }
        public Role Role { get; set; }
    }
}