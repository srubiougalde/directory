using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DirectoryApi.Entities;
using DirectoryApi.Services;
using Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static async Task<User> GetUserIdentity(this HttpContext httpContext, IUsersService usersService)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (identity == null)
        {
            return null;
        }
        var userId = identity.FindFirst("id").Value;
        var user = await usersService.GetUserByIdAsync(new Guid(userId));
        return user;
    }
}