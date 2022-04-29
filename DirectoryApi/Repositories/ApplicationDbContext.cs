using DirectoryApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryApi.Repositories;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<Friendship> Friendships => Set<Friendship>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Member>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasOne(p => p.Website).WithOne();
            e.HasMany(p => p.Friends)
            .WithMany(p => p.FriendOf)
            .UsingEntity<Friendship>(
                p => p.HasOne<Member>().WithMany().HasForeignKey(x => x.MemberId),
                p => p.HasOne<Member>().WithMany().HasForeignKey(x => x.FriendId));
        });

        modelBuilder.Entity<Website>(e =>
            e.OwnsMany(p => p.Headings, x =>
            {
                x.WithOwner().HasForeignKey();
            })
        );
    }
}
