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
        return await FindAll().ToListAsync();
    }

    public async Task<Member> GetMemberByIdAsync(Guid id)
    {
        return await FindByCondition(x => x.Id == id).SingleOrDefaultAsync();
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
        dbMember.WebsiteUrl = inputMember.WebsiteUrl;

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
}