using DirectoryApi.Entities;
using DirectoryApi.Models;
using DirectoryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DirectoryApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DirectoryController : ControllerBase
{
    private readonly IStringLocalizer<DirectoryController> _localizer;
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
    private readonly ILogger<DirectoryController> _logger;
    private readonly IDirectoryService _directoryService;
    private readonly IWebsiteService _websiteService;
    private readonly IUsersService _userService;

    public DirectoryController(IStringLocalizer<DirectoryController> localizer, IStringLocalizer<SharedResource> sharedLocalizer, ILogger<DirectoryController> logger, IDirectoryService directoryService, IWebsiteService websiteService, IUsersService userService)
    {
        _localizer = localizer;
        _sharedLocalizer = sharedLocalizer;
        _logger = logger;
        _directoryService = directoryService;
        _websiteService = websiteService;
        _userService = userService;
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetMembers()
    {
        _logger.LogInformation(_localizer["Getting list of members"]);
        var members = await _directoryService.GetAllMembersAsync();

        var respose = members.Select(x => new GetMemberResponse
        {
            Id = x.Id,
            Name = x.Name,
            WebsiteUrl = x.Website.Url,
            WebsiteShortUrl = $"https://evr.ly/{x.Website.ShortUrl}",
            FriendsCount = x.Friends.Count
        });

        return Ok(respose);
    }

    [HttpGet("members/{id}")]
    public async Task<IActionResult> GetMemberById(Guid id)
    {
        _logger.LogInformation($"{_localizer["Getting member by id"]} {id}");
        var member = await _directoryService.GetMemberByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        var respose = new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            WebsiteUrl = member.Website.Url,
            WebsiteShortUrl = $"https://evr.ly/{member.Website.ShortUrl}",
            FriendsCount = member.Friends.Count
        };

        return Ok(respose);
    }

    [HttpPost("members")]
    public async Task<IActionResult> CreateMember(CreateMemberRequest input)
    {
        _logger.LogInformation(_localizer["Creating member"]);
        var member = await _directoryService.CreateMemberAsync(new Member
        {
            Name = input.Name,
            Website = new Website
            {
                Url = input.WebsiteUrl
            }
        });

        if (member == null)
        {
            return BadRequest();
        }

        var website = await _websiteService.SyncWebsiteHeadingAsync(member.Website);

        var respose = new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            WebsiteUrl = member.Website.Url,
            WebsiteShortUrl = $"https://evr.ly/{member.Website.ShortUrl}"
        };

        return Ok(respose);
    }

    [HttpPut("members/{id}")]
    public async Task<IActionResult> UpdateMember(Guid id, CreateMemberRequest input)
    {
        _logger.LogInformation($"{_localizer["Updating member with id"]} {id}");
        var memberInput = new Member
        {
            Name = input.Name,
            Website = new Website
            {
                Url = input.WebsiteUrl
            }
        };

        var member = await _directoryService.UpdateMemberAsync(id, memberInput);

        if (member == null)
        {
            return NotFound();
        }

        var website = await _websiteService.SyncWebsiteHeadingAsync(member.Website);

        return NoContent();
    }

    [HttpDelete("members/{id}")]
    public async Task<IActionResult> DeleteMember(Guid id)
    {
        _logger.LogInformation($"{_localizer["Deleting member with id"]} {id}");
        var member = await _directoryService.DeleteMemberAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        var respose = new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            WebsiteUrl = member.Website.Url
        };

        return Ok(respose);
    }

    [HttpGet("members/{id}/friends")]
    public async Task<IActionResult> GetMemberFriends(Guid id)
    {
        _logger.LogInformation($"{_localizer["Getting friends of id"]} {id}");

        var member = await _directoryService.GetMemberFriendsByIdAsync(id);

        var result = new GetFriendshipResponse
        {
            Id = member.Id,
            Name = member.Name,
            WebsiteUrl = member.Website.Url,
            WebsiteShortUrl = $"https://evr.ly/{member.Website.ShortUrl}",
            Friends = member.Friends.Select(x => new GetFriendshipResponse
            {
                Id = x.Id,
                Name = x.Name,
                WebsiteUrl = x.Website.Url,
                WebsiteShortUrl = $"https://evr.ly/{x.Website.ShortUrl}",
                Headings = x.Website.Headings.Select(h => (Name: h.Name, Text: h.InnerText)).ToList()
            }).ToList()
        };

        return Ok(result);
    }

    [HttpPost("members/{id}/friends")]
    public async Task<IActionResult> CreateFriendship(Guid id, CreateFriendshipRequest input)
    {
        _logger.LogInformation($"{_localizer["Creating friends of id"]} {id}");

        var member = await _directoryService.CreateFriendshipAsync(id, input.FriendId);

        var result = new GetFriendshipResponse
        {
            Id = member.Id,
            Name = member.Name,
            Friends = member.Friends.Where(x => x.Id.Equals(input.FriendId)).Select(x => new GetFriendshipResponse
            {
                Id = x.Id,
                Name = x.Name
            }).ToList()
        };

        return Ok(result);
    }

    [HttpDelete("members/{id}/friends/{friendId}")]
    public async Task<IActionResult> DeleteFriendship(Guid id, Guid friendId)
    {
        _logger.LogInformation($"{_localizer["Deliting friends of id"]} {id}");

        await _directoryService.DeleteFriendshipMemberAsync(id, friendId);

        return Ok();
    }

    [HttpGet("topics/search/{query}")]
    public async Task<IActionResult> SearchTopicExperts(string query)
    {
        // Get the user in session based on the token
        var userIdentity = await HttpContext.GetUserIdentity(_userService);
        if (userIdentity == null)
        {
            return BadRequest("Invalid claims identity");
        }

        _logger.LogInformation(_localizer["Searching experts for topic"]);

        // Get websites with certain topic 
        var websites = await _websiteService.GetWebsiteByTopicAsync(query);

        // Get friends for the current user
        var friends = (await _directoryService.GetMemberFriendsByIdAsync(userIdentity.Id)).Friends;

        // Filter friend's websites 
        var suggestion = websites.Where(x => !friends.Any(f => f.Id == x.MemberId));

        var experts = new List<GetMemberResponse>();
        foreach (Website s in suggestion)
        {
            var expert = await _directoryService.GetMemberByIdAsync(s.MemberId);

            experts.Add(new GetMemberResponse
            {
                Id = expert.Id,
                Name = expert.Name,
                WebsiteUrl = expert.Website.Url,
                WebsiteShortUrl = $"https://evr.ly/{expert.Website.ShortUrl}",
                FriendsCount = expert.Friends.Count
            });
        }
        return Ok(experts);
    }
}
