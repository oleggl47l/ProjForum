namespace ProjForum.Identity.Application.DTOs.Auth;

public record GetCurrentUserResultDto(
    OperationResultDto Result,
    CurrentUserDto? CurrentUser = null
);