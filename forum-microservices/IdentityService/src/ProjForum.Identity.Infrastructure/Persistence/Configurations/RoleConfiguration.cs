using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjForum.Identity.Infrastructure.Persistence.Entities;

namespace ProjForum.Identity.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}