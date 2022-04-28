using DirectoryApi.Entities;

namespace DirectoryApi.Repositories;

public interface IMemberRepository : IBaseRepository<Member>
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
    Task<Member> GetMemberByIdAsync(Guid id);
    Task<Member> CreateMemberAsync(Member member);
    Task<Member> UpdateMemberAsync(Member dbMember, Member inputMember);
    Task<Member> DeleteMemberAsync(Member member);
}