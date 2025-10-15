namespace ProjForum.Identity.Application.DTOs.User;

public record PagedUsersDto(
    IReadOnlyList<UserDto> Users,
    int TotalCount,
    int PageIndex,
    int PageSize
);