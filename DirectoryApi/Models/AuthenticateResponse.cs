using System;
using System.Text.Json.Serialization;
using DirectoryApi.Entities;

namespace DirectoryApi.Models
{
    public class AuthenticateResponse
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(string userId, string jwtToken, string refreshToken, int expiresIn)
        {
            UserId = userId;
            AccessToken = jwtToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
    }
}