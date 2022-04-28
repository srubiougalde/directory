using DirectoryApi.Entities;
using DirectoryApi.Models;
using DirectoryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DirectoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DirectoryController : ControllerBase
{
    private readonly ILogger<DirectoryController> _logger;
    private readonly IDirectoryService _directoryService;

    public DirectoryController(ILogger<DirectoryController> logger, IDirectoryService directoryService)
    {
        _logger = logger;
        _directoryService = directoryService;
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetMembers()
    {
        _logger.LogInformation("Getting list of members");
        var members = await _directoryService.GetAllMembersAsync();

        var respose = members.Select(x => new GetMemberResponse
        {
            Id = x.Id,
            Name = x.Name,
            WebsiteUrl = x.WebsiteUrl
        });

        return Ok(respose);
    }

    [HttpGet("members/{id}")]
    public async Task<IActionResult> GetMemberById(Guid id)
    {
        _logger.LogInformation($"Getting member by id {id}");
        var member = await _directoryService.GetMemberByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        var respose = new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            WebsiteUrl = member.WebsiteUrl
        };

        return Ok(respose);
    }

    [HttpPost("members")]
    public async Task<IActionResult> CreateMember(CreateMemberRequest input)
    {
        _logger.LogInformation($"Creating member");
        var member = await _directoryService.CreateMemberAsync(new Member
        {
            Name = input.Name,
            WebsiteUrl = input.WebsiteUrl
        });

        if (member == null)
        {
            return BadRequest();
        }

        var respose = new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            WebsiteUrl = member.WebsiteUrl
        };

        return Ok(respose);
    }

    [HttpPut("members/{id}")]
    public async Task<IActionResult> UpdateMember(Guid id, CreateMemberRequest input)
    {
        _logger.LogInformation($"Updating member with id {id}");
        var memberInput = new Member
        {
            Name = input.Name,
            WebsiteUrl = input.WebsiteUrl
        };

        var member = await _directoryService.UpdateMemberAsync(id, memberInput);

        return (member != null) ? NoContent() : NotFound();
    }

    [HttpDelete("members/{id}")]
    public async Task<IActionResult> DeleteMember(Guid id)
    {
        _logger.LogInformation($"Deleting member with id {id}");
        var member = await _directoryService.DeleteMemberAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        var respose = new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            WebsiteUrl = member.WebsiteUrl
        };

        return Ok(respose);
    }
}
