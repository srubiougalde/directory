namespace DirectoryApi.Entities;

public class Member
{
    public Member()
    {
        Website = new Website();
    }

    public Guid Id { get; set; }
    public string? Name { get; set; }

    //-----------------------------
    //Relationships
    public Website Website { get; set; }
}