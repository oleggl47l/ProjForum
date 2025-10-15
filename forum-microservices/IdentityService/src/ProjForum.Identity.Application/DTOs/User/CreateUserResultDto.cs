namespace ProjForum.Identity.Application.DTOs.User;

public record CreateUserResultDto(
    OperationResultDto Result,
    UserDto? User
);