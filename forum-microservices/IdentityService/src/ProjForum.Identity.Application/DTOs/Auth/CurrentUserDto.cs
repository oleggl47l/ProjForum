namespace ProjForum.Identity.Application.DTOs.Auth;

public record CurrentUserDto(
    Guid Id,
    string UserName,
    IEnumerable<string> Roles
);