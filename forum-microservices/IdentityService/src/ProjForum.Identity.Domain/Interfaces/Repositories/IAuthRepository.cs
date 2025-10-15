namespace ProjForum.Identity.Domain.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<(bool Success, string ErrorMessage)> RegisterAsync(string userName, string email, string password,
        IEnumerable<string>? roles, CancellationToken cancellationToken = default);

    Task<(bool Success, string Token, string RefreshToken, DateTime RefreshTokenExpires, string ErrorMessage)>
        LoginAsync(string userName, string password, CancellationToken cancellationToken = default);

    Task LogoutAsync(CancellationToken cancellationToken = default);
}