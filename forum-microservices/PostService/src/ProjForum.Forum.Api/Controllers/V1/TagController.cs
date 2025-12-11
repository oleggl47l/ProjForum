using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Forum.Application.DTOs;
using ProjForum.Forum.Application.Forum.Commands.Tags;
using ProjForum.Forum.Application.Forum.Queries.Tags;
using ProjForum.Forum.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjForum.Forum.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class TagController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [SwaggerOperation(OperationId = "GetTagById")]
    public async Task<ActionResult<TagModel>> GetTag(Guid id, CancellationToken cancellationToken)
    {
        var tag = await mediator.Send(new GetTagByIdQuery { Id = id }, cancellationToken);
        if (tag is null)
            return NotFound(new OperationResultDto(false, "Tag not found"));

        return Ok(tag);
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllTags")]
    public async Task<ActionResult<IReadOnlyList<TagModel>>> GetTags(CancellationToken cancellationToken)
    {
        var tags = await mediator.Send(new GetAllTagsQuery(), cancellationToken);
        return Ok(tags);
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreateTag")]
    public async Task<ActionResult<Unit>> CreateTag([FromBody] CreateTagCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpPatch]
    [SwaggerOperation(OperationId = "UpdateTag")]
    public async Task<ActionResult<Unit>> UpdateTag([FromBody] UpdateTagCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpDelete]
    [SwaggerOperation(OperationId = "DeleteTag")]
    public async Task<ActionResult<Unit>> DeleteTag([FromBody] DeleteTagCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}