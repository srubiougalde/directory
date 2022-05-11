using Xunit;
using DirectoryApi.Controllers;
using DirectoryApi.Services;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using DirectoryApi.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryApi.Tests;

public class DirectoryControllerTests
{
    private readonly Mock<IStringLocalizer<DirectoryController>> _mockLocalizer;
    private readonly Mock<IStringLocalizer<SharedResource>> _mockSharedLocalizer;
    private readonly Mock<ILogger<DirectoryController>> _mockLogger;
    private readonly Mock<IDirectoryService> _mockDirectoryService;
    private readonly Mock<IWebsiteService> _mockWebsiteService;
    private readonly Mock<IUsersService> _mockUserService;
    private readonly DirectoryController _controller;

    public DirectoryControllerTests()
    {
        _mockLocalizer = new Mock<IStringLocalizer<DirectoryController>>();
        _mockSharedLocalizer = new Mock<IStringLocalizer<SharedResource>>();
        _mockLogger = new Mock<ILogger<DirectoryController>>();
        _mockDirectoryService = new Mock<IDirectoryService>();
        _mockWebsiteService = new Mock<IWebsiteService>();
        _mockUserService = new Mock<IUsersService>();

        _mockDirectoryService.Setup(p => p.GetAllMembersAsync()).ReturnsAsync(new List<Member>{
            new Member { Name = "John Doe", Website = new Website { Url = "https://www.lipsum.com/" }}
        });

        _controller = new DirectoryController(_mockLocalizer.Object, _mockSharedLocalizer.Object, _mockLogger.Object, _mockDirectoryService.Object, _mockWebsiteService.Object, _mockUserService.Object);
    }

    [Fact]
    public async void GetMembers_Success_Test()
    {
        var result = await _controller.GetMembers();
        var okResult = result as OkObjectResult;

        Assert.NotNull(okResult);
        Assert.Equal(200, okResult?.StatusCode);
    }

    [Fact(Skip = "Not implemented")]
    public async void GetMemberById_Success_Test()
    {
        Assert.Equal(true, true);
    }

    [Fact(Skip = "Not implemented")]
    public async void CreateMember_Success_Test()
    {
        Assert.Equal(true, true);
    }

    [Fact(Skip = "Not implemented")]
    public async void UpdateMember_Success_Test()
    {
        Assert.Equal(true, true);
    }

    [Fact(Skip = "Not implemented")]
    public async void DeleteMember_Success_Test()
    {
        Assert.Equal(true, true);
    }
}