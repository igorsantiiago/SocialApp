using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatingApp.API.Data.Mappings;

public class AppUserMap : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasColumnName("UserName")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasColumnName("PasswordHash")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(1000);

        builder.Property(x => x.BirthDate)
            .IsRequired()
            .HasColumnName("BirthDate");

        builder.Property(x => x.KnownAs)
            .IsRequired()
            .HasColumnName("KnownAs")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        builder.Property(x => x.LastActivity)
            .IsRequired()
            .HasColumnName("LastActivity");

        builder.Property(x => x.Gender)
            .IsRequired()
            .HasColumnName("Gender")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(30);

        builder.Property(x => x.Introduction)
            .IsRequired()
            .HasColumnName("Introduction")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(4000);

        builder.Property(x => x.LookingFor)
            .IsRequired()
            .HasColumnName("LookingFor")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(1000);

        builder.Property(x => x.Interests)
            .IsRequired()
            .HasColumnName("Interests")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(1000);

        builder.Property(x => x.City)
            .IsRequired()
            .HasColumnName("City")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasColumnName("Country")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.HasMany(x => x.Photos).WithOne(x => x.AppUser)
            .HasForeignKey(x => x.AppUserId)
            .HasConstraintName("FK_AppUser_PhotosId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UserRoles).WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}
