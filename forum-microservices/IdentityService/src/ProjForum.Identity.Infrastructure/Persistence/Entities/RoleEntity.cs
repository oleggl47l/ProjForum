using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Infrastructure.Persistence.Entities;

public class RoleEntity : IdentityRole
{
    public bool IsActive { get; set; } = true;
}