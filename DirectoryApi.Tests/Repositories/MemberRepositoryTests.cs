using System.Threading.Tasks;
using DirectoryApi.Entities;
using DirectoryApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Linq;

namespace DirectoryApi.Tests;

public class MemberRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public MemberRepositoryTests()
    {
        var dbName = $"DirectoryDb_{DateTime.Now.ToFileTimeUtc()}";
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(dbName).Options;
    }

    [Fact]
    public async Task GetAllMembersAsync_Success_Test()
    {
        var repository = await CreateRepositoryAsync();

        var members = await repository.GetAllMembersAsync();
        Assert.Equal(3, members.ToList().Count);
    }

    [Fact]
    public async Task GetMemberByIdAsync_Success_Test()
    {
        var repository = await CreateRepositoryAsync();

        var member = await repository.GetMemberByIdAsync(new Guid("8b0eecca-69c2-45bc-88f4-576a0b8a40bd"));
        Assert.NotNull(member);
    }

    [Fact]
    public async Task CreateMemberAsync_Success_Test()
    {
        var repository = await CreateRepositoryAsync();

        await repository.CreateMemberAsync(new Member
        {
            Id = new Guid("ebaed374-fb88-4768-a37f-3f46d4de304e"),
            Name = "John Doe",
            Website = new Website
            {
                Url = "https://www.lipsum.com/"
            }
        });

        var members = await repository.GetAllMembersAsync();
        Assert.Equal(4, members.ToList().Count);
    }

    [Fact]
    public async Task UpdateMemberAsync_Success_Test()
    {
        var repository = await CreateRepositoryAsync();

        var dbMember = await repository.GetMemberByIdAsync(new Guid("8b0eecca-69c2-45bc-88f4-576a0b8a40bd"));

        var memberInput = new Member
        {
            Name = "John Doe",
            Website = new Website
            {
                Url = "https://www.lipsum.com/"
            }
        };

        var member = await repository.UpdateMemberAsync(dbMember, memberInput);
        Assert.Equal("John Doe", member.Name);
    }

    [Fact]
    public async Task DeleteMemberAsync_Success_Test()
    {
        var repository = await CreateRepositoryAsync();

        var dbMember = await repository.GetMemberByIdAsync(new Guid("8b0eecca-69c2-45bc-88f4-576a0b8a40bd"));

        await repository.DeleteMemberAsync(dbMember);

        var members = await repository.GetAllMembersAsync();
        Assert.Equal(2, members.ToList().Count);
    }

    private async Task<MemberRepository> CreateRepositoryAsync()
    {
        var context = new ApplicationDbContext(_dbContextOptions);
        await PopulateDataAsync(context);
        return new MemberRepository(context);
    }

    private async Task PopulateDataAsync(ApplicationDbContext context)
    {
        await context.Members.AddAsync(new Member
        {
            Id = new Guid("8b0eecca-69c2-45bc-88f4-576a0b8a40bd"),
            Name = "Phillip Anthropy",
            Website = new Website
            {
                Url = "https://www.lipsum.com/"
            }
        });

        await context.Members.AddAsync(new Member
        {
            Id = new Guid("3564d532-e066-4513-8db7-5696326a3dea"),
            Name = "Parsley Montana",
            Website = new Website
            {
                Url = "https://www.lipsum.com/"
            }
        });

        await context.Members.AddAsync(new Member
        {
            Id = new Guid("aa5a3b7e-237f-4a9f-bdc6-45bf58af5200"),
            Name = "Jason Response",
            Website = new Website
            {
                Url = "https://www.lipsum.com/"
            }
        });

        await context.SaveChangesAsync();
    }
}