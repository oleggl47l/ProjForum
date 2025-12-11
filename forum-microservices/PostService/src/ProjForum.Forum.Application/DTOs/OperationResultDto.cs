namespace ProjForum.Forum.Application.DTOs;

public record OperationResultDto(
    bool Success,
    string Message
);