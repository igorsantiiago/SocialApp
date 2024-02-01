using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatingApp.API.Data.Migrations;

public class PhotoMap : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable("Photos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("Id")
            .HasColumnType("INT")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Url)
            .IsRequired()
            .HasColumnName("Url")
            .HasColumnType("NVARCHAR");

        builder.Property(x => x.IsProfile)
            .HasColumnName("IsProfile");

        builder.Property(x => x.PublicId)
            .IsRequired()
            .HasColumnName("PublicID")
            .HasColumnType("NVARCHAR");
    }
}
