namespace ProjForum.Identity.Application.DTOs.User;

public record UpdateUserResultDto(
    OperationResultDto Result,
    UserDto? User
);