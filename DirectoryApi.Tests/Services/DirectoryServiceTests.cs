using Xunit;
using Moq;
using System;
using System.Linq;
using DirectoryApi.Services;
using DirectoryApi.Repositories;
using System.Collections.Generic;
using DirectoryApi.Entities;

namespace DirectoryApi.Tests;

public class DirectoryServiceTests
{
    private readonly Mock<IMemberRepository> _mockMemberRepository;
    private readonly DirectoryService _service;

    public DirectoryServiceTests()
    {
        _mockMemberRepository = new Mock<IMemberRepository>();

        _mockMemberRepository.Setup(p => p.GetAllMembersAsync()).ReturnsAsync(new List<Member>{
            new Member
            {
                Name = "John Doe",
                Website = new Website
                {
                    Url = "https://www.lipsum.com/"
                }
            }
        });

        _service = new DirectoryService(_mockMemberRepository.Object);
    }

    [Fact]
    public async void GetAllMembersAsync_Success_Test()
    {
        var members = await _service.GetAllMembersAsync();

        Assert.Equal(1, members.ToList().Count);
    }

    [Fact(Skip = "Not implemented")]
    public async void GetMemberByIdAsync_Success_Test()
    {
        Assert.Equal(true, true);
    }

    [Fact(Skip = "Not implemented")]
    public async void CreateMemberAsync_Success_Test()
    {
        Assert.Equal(true, true);
    }

    [Fact(Skip = "Not implemented")]
    public async void UpdateMemberAsync_Success_Test()
    {
        Assert.Equal(true, true);
    }

    [Fact(Skip = "Not implemented")]
    public async void DeleteMemberAsync_Success_Test()
    {
        Assert.Equal(true, true);
    }
}