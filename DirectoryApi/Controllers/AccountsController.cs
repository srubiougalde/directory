using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DirectoryApi.Services;
using DirectoryApi.Models;
using Microsoft.AspNetCore.Http;

namespace DirectoryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private IAccountsService _accountService;
        private IUsersService _userService;

        public AccountsController(IAccountsService accountService, IUsersService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var response = _accountService.Authenticate(model, IpAddress()).Result;

            if (string.IsNullOrEmpty(response?.AccessToken))
            {
                return BadRequest("Username or password is incorrect");
            }

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("authcode")]
        public IActionResult AuthenticateWithCode([FromBody] AuthenticateCodeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var response = _accountService.AuthenticateWithCode(model, IpAddress()).Result;

            if (string.IsNullOrEmpty(response?.AccessToken))
            {
                return BadRequest("Username or password is incorrect");
            }

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _accountService.RefreshToken(refreshToken, IpAddress());

            if (response == null)
            {
                return Unauthorized("Invalid token");
            }

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            var response = await _accountService.RevokeToken(token, IpAddress());

            if (!response)
            {
                return NotFound("Token not found");
            }

            return Ok("Token revoked");
        }

        [HttpGet("{id}/refreshTokens")]
        public async Task<IActionResult> GetRefreshTokens(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Tokens not found");
            }

            return Ok(user.RefreshTokens);
        }

        [HttpGet("me", Name = "UserIdentity")]
        public async Task<IActionResult> GetUserIdentity()
        {
            var user = await HttpContext.GetUserIdentity(_userService);
            if (user == null)
            {
                return BadRequest("Invalid claims identity");
            }

            return Ok(new
            {
                user.Id,
                user.Profile?.FirstName,
                user.Profile?.LastName,
                user.Profile?.Email
            });
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}