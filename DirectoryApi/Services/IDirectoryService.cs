using DirectoryApi.Entities;

namespace DirectoryApi.Services;

public interface IDirectoryService
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
    Task<Member> GetMemberByIdAsync(Guid id);
    Task<Member> CreateMemberAsync(Member member);
    Task<Member> UpdateMemberAsync(Guid id, Member inputMember);
    Task<Member> DeleteMemberAsync(Guid id);

    // Friendship
    Task<Member> GetMemberFriendsByIdAsync(Guid id);
    Task<Member> CreateFriendshipAsync(Guid id, Guid friendId);
    Task<Member> DeleteFriendshipMemberAsync(Guid id, Guid friendId);
}