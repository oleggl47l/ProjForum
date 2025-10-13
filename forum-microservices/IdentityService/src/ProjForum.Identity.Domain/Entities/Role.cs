using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Domain.Entities;

public class Role : IdentityRole
{
    public bool IsActive { get; set; }
}
