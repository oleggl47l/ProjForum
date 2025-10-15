using ProjForum.Identity.Domain.Identities;

namespace ProjForum.Identity.Domain.Interfaces;

public interface IJwtGenerator
{
    string GenerateToken(User user, IList<string> roles);
    (string RefreshToken, DateTime Expires) GenerateRefreshToken();
}