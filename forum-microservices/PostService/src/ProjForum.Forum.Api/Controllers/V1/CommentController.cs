using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Forum.Application.DTOs;
using ProjForum.Forum.Application.Forum.Commands.Comments;
using ProjForum.Forum.Application.Forum.Queries.Comments;
using ProjForum.Forum.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjForum.Forum.Api.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/[controller]/[action]")]
public class CommentController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [SwaggerOperation(OperationId = "GetCommentById")]
    public async Task<ActionResult<CommentModel>> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var comment = await mediator.Send(new GetCommentByIdQuery { Id = id }, cancellationToken);
        if (comment is null)
            return NotFound(new OperationResultDto(false, "Comment not found"));

        return Ok(comment);
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllComments")]
    public async Task<ActionResult<IReadOnlyList<CommentModel>>> GetCommentsAsync(CancellationToken cancellationToken)
    {
        var comments = await mediator.Send(new GetAllCommentsQuery(), cancellationToken);
        return Ok(comments);
    }

    [HttpGet("{authorId:guid}/author")]
    [SwaggerOperation(OperationId = "GetAllCommentsByAuthor")]
    public async Task<ActionResult<IReadOnlyList<CommentModel>>> GetCommentsByAuthorAsync(Guid authorId,
        CancellationToken cancellationToken)
    {
        var comments = await mediator.Send(new GetCommentsByAuthorQuery { AuthorId = authorId }, cancellationToken);
        return Ok(comments);
    }

    [HttpGet("{postId:guid}/post")]
    [SwaggerOperation(OperationId = "GetAllCommentsByPost")]
    public async Task<ActionResult<IReadOnlyList<CommentModel>>> GetCommentsByPostAsync(Guid postId,
        CancellationToken cancellationToken)
    {
        var comments = await mediator.Send(new GetCommentsByPostQuery { PostId = postId }, cancellationToken);
        return Ok(comments);
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreateComment")]
    public async Task<ActionResult<Unit>> CreateCommentAsync(CreateCommentCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPatch]
    [SwaggerOperation(OperationId = "UpdateComment")]
    public async Task<ActionResult<Unit>> UpdateCommentAsync(UpdateCommentCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [SwaggerOperation(OperationId = "DeleteComment")]
    public async Task<ActionResult<Unit>> DeleteCommentAsync(DeleteCommentCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }
}