using DirectoryApi.Entities;
using DirectoryApi.Repositories;

namespace DirectoryApi.Services;

public class DirectoryService : IDirectoryService
{
    private readonly IMemberRepository _memberRepository;

    public DirectoryService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return _memberRepository.GetAllMembersAsync();
    }

    public Task<Member> GetMemberByIdAsync(Guid id)
    {
        return _memberRepository.GetMemberByIdAsync(id);
    }

    public Task<Member> CreateMemberAsync(Member member)
    {
        if (member.Id == Guid.Empty)
        {
            member.Id = new Guid();
        }

        return _memberRepository.CreateMemberAsync(member);
    }

    public async Task<Member> UpdateMemberAsync(Guid id, Member inputMember)
    {
        var dbMember = await _memberRepository.GetMemberByIdAsync(id);

        if (dbMember == null)
        {
            return null;
        }

        return await _memberRepository.UpdateMemberAsync(dbMember, inputMember);
    }

    public async Task<Member> DeleteMemberAsync(Guid id)
    {
        var member = await _memberRepository.GetMemberByIdAsync(id);

        if (member == null)
        {
            return null;
        }

        return await _memberRepository.DeleteMemberAsync(member);
    }

    // Friendship
    public async Task<Member> GetMemberFriendsByIdAsync(Guid id)
    {
        return await _memberRepository.GetMemberFriendsByIdAsync(id);
    }

    public async Task<Member> CreateFriendshipAsync(Guid id, Guid friendId)
    {
        return await _memberRepository.CreateFriendshipAsync(id, friendId);
    }

    public async Task<Member> DeleteFriendshipMemberAsync(Guid id, Guid friendId)
    {
        return await _memberRepository.DeleteFriendshipMemberAsync(id, friendId);
    }
}