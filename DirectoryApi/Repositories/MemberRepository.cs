using DirectoryApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryApi.Repositories;

public class MemberRepository : BaseRepository<Member>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return await FindAll().Include(p => p.Website).ToListAsync();
    }

    public async Task<Member> GetMemberByIdAsync(Guid id)
    {
        return await FindByCondition(x => x.Id == id).Include(p => p.Website).SingleOrDefaultAsync();
    }

    public async Task<Member> CreateMemberAsync(Member member)
    {
        member.Id = Guid.NewGuid();

        Create(member);
        await SaveAsync();

        return member;
    }

    public async Task<Member> UpdateMemberAsync(Member dbMember, Member inputMember)
    {
        dbMember.Name = inputMember.Name;

        DetachLocal(dbMember, p => p.Id.Equals(dbMember.Id));
        Update(dbMember);
        await SaveAsync();

        return dbMember;
    }

    public async Task<Member> DeleteMemberAsync(Member member)
    {
        DetachLocal(member, p => p.Id.Equals(member.Id));
        Delete(member);
        await SaveAsync();

        return member;
    }

    // Friendship
    public async Task<Member> GetMemberFriendsByIdAsync(Guid id)
    {
        return await FindByCondition(x => x.Id == id)
                    .Include(p => p.Website)
                    .Include(p => p.Friends).ThenInclude(p => p.Website)
                    .SingleOrDefaultAsync();
    }

    public async Task<Member> CreateFriendshipAsync(Guid id, Guid friendId)
    {
        var member = await FindByCondition(x => x.Id == id).SingleOrDefaultAsync();
        var friend = await FindByCondition(x => x.Id == friendId).SingleOrDefaultAsync();

        member.Friends.Add(friend);
        friend.Friends.Add(member);

        await SaveAsync();

        return member;
    }

    public async Task<Member> DeleteFriendshipMemberAsync(Guid id, Guid friendId)
    {
        var member = await FindByCondition(x => x.Id == id).SingleOrDefaultAsync();
        var friend = await FindByCondition(x => x.Id == friendId).SingleOrDefaultAsync();

        member.Friends.Remove(friend);
        friend.Friends.Remove(member);

        await SaveAsync();

        return member;
    }
}