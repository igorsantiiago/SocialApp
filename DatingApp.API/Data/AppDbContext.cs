using DatingApp.API.Data.Mappings;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserLike> UserLikes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppRole>().HasMany(x => x.UserRoles).WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();

        modelBuilder.ApplyConfiguration(new AppUserMap());
        modelBuilder.ApplyConfiguration(new UserLikeMap());
        modelBuilder.ApplyConfiguration(new MessageMap());
    }
}
