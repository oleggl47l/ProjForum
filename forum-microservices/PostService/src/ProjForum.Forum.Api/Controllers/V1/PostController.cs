using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Forum.Application.Forum.Commands.Posts;
using ProjForum.Forum.Application.Forum.Queries.Posts;

namespace ProjForum.Forum.Api.Controllers.V1;

[Route("api/v1/[controller]/[action]")]
[ApiController]
[Authorize]
public class PostController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostByIdAsync(Guid id)
    {
        var post = await mediator.Send(new GetPostByIdQuery { Id = id });
        return Ok(post);
    }

    [HttpGet]
    public async Task<IActionResult> GetPostsAsync()
    {
        var posts = await mediator.Send(new GetAllPostsQuery());
        return Ok(posts);
    }

    [HttpGet("{id}/tag")]
    public async Task<IActionResult> GetPostsByTagAsync(Guid id)
    {
        var posts = await mediator.Send(new GetPostsByTagQuery { TagId = id });
        return Ok(posts);
    }

    [HttpGet("{id}/category")]
    public async Task<IActionResult> GetPostsByCategory(Guid id)
    {
        var posts = await mediator.Send(new GetPostsByCategoryQuery { CategoryId = id });
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync(CreatePostCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> UpdatePostAsync(UpdatePostCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePostAsync(DeletePostCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddTagToPost(AddTagToPostCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveTagFromPost(RemoveTagFromPostCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}