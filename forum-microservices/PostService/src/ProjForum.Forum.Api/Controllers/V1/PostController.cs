using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Forum.Application.DTOs;
using ProjForum.Forum.Application.Forum.Commands.Posts;
using ProjForum.Forum.Application.Forum.Queries.Posts;
using ProjForum.Forum.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjForum.Forum.Api.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/[controller]/[action]")]
public class PostController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [SwaggerOperation(OperationId = "GetPostById")]
    public async Task<ActionResult<PostModel>> GetPostByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var post = await mediator.Send(new GetPostByIdQuery { Id = id }, cancellationToken);
        if (post is null)
            return NotFound(new OperationResultDto(false, "Post not found"));

        return Ok(post);
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllPosts")]
    public async Task<ActionResult<IReadOnlyList<PostModel>>> GetPostsAsync(
        CancellationToken cancellationToken)
    {
        var posts = await mediator.Send(new GetAllPostsQuery(), cancellationToken);
        return Ok(posts);
    }

    [HttpGet("{id:guid}/tag")]
    [SwaggerOperation(OperationId = "GetAllPostsByTag")]
    public async Task<ActionResult<IReadOnlyList<SimplePostModel>>> GetPostsByTagAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var posts = await mediator.Send(new GetPostsByTagQuery { TagId = id }, cancellationToken);
        return Ok(posts);
    }

    [HttpGet("{id:guid}/category")]
    [SwaggerOperation(OperationId = "GetAllPostsByCategory")]
    public async Task<ActionResult<IReadOnlyList<SimplePostModel>>> GetPostsByCategory(Guid id,
        CancellationToken cancellationToken)
    {
        var posts = await mediator.Send(new GetPostsByCategoryQuery { CategoryId = id }, cancellationToken);
        return Ok(posts);
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreatePost")]
    public async Task<ActionResult<Unit>> CreatePostAsync(CreatePostCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPatch]
    [SwaggerOperation(OperationId = "UpdatePost")]
    public async Task<ActionResult<Unit>> UpdatePostAsync(UpdatePostCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [SwaggerOperation(OperationId = "DeletePost")]
    public async Task<ActionResult<Unit>> DeletePostAsync(DeletePostCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "AddTagToPost")]
    public async Task<ActionResult<Unit>> AddTagToPost(AddTagToPostCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    [SwaggerOperation(OperationId = "DeleteTagFromPost")]
    public async Task<ActionResult<Unit>> RemoveTagFromPost(RemoveTagFromPostCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}