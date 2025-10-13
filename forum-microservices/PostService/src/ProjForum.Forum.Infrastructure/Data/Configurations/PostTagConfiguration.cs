using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjForum.Forum.Domain.Entities;

namespace ProjForum.Forum.Infrastructure.Data.Configurations;

public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        builder.HasKey(rt => new { rt.PostId, rt.TagId });

        builder.HasOne(rt => rt.Post)
            .WithMany(r => r.PostTags)
            .HasForeignKey(rt => rt.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rt => rt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(rt => rt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}