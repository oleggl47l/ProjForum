namespace ProjForum.Identity.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string UserName,
    string Email,
    bool Active,
    int AccessFailedCount,
    IEnumerable<string> Roles
);