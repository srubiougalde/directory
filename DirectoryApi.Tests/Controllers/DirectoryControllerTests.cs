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
    private readonly Mock<ILogger<DirectoryController>> _mockLogger;
    private readonly Mock<IDirectoryService> _mockDirectoryService;
    private readonly DirectoryController _controller;

    public DirectoryControllerTests()
    {
        _mockLogger = new Mock<ILogger<DirectoryController>>();
        _mockDirectoryService = new Mock<IDirectoryService>();

        _mockDirectoryService.Setup(p => p.GetAllMembersAsync()).ReturnsAsync(new List<Member>{
            new Member { Name = "John Doe", WebsiteUrl = "https://www.lipsum.com/" }
        });

        _controller = new DirectoryController(_mockLogger.Object, _mockDirectoryService.Object);
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