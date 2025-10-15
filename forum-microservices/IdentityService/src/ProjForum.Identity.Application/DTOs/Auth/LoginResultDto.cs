namespace ProjForum.Identity.Application.DTOs.Auth;

public record LoginResultDto(
    OperationResultDto Result,
    string? Token = null,
    string? RefreshToken = null,
    DateTime? RefreshTokenExpires = null
);