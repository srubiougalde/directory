namespace DirectoryApi.Entities;

public class Member
{
    public Member()
    {
        Website = new Website();
        Friends = new HashSet<Member>();
        FriendOf = new HashSet<Member>();
    }

    public Guid Id { get; set; }
    public string? Name { get; set; }
    public ICollection<Member> Friends { get; set; }
    public ICollection<Member> FriendOf { get; set; }

    //-----------------------------
    //Relationships
    public Website Website { get; set; }
}