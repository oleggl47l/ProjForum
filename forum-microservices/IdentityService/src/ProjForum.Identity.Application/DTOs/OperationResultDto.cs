namespace ProjForum.Identity.Application.DTOs;

public record OperationResultDto(
    bool Success,
    string Message
);