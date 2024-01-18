using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatingApp.API;

public class UserLikeMap : IEntityTypeConfiguration<UserLike>
{
    public void Configure(EntityTypeBuilder<UserLike> builder)
    {
        builder.ToTable("Likes");

        builder.HasKey(key => new { key.SourceUserId, key.TargetUserId });

        builder.HasOne(source => source.SourceUser).WithMany(like => like.LikedUsers)
            .HasForeignKey(source => source.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(target => target.TargetUser).WithMany(like => like.LikedByUsers)
            .HasForeignKey(target => target.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
        // .OnDelete(DeleteBehavior.Cascade); MUDAR QUANDO USAR SqlServer para evitar conflito.
    }
}
