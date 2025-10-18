using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Infrastructure.Persistence.Entities;

public class UserEntity : IdentityUser
{
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; }
    public bool Active { get; set; } = true;
}