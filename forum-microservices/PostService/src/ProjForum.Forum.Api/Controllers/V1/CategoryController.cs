using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Forum.Application.DTOs;
using ProjForum.Forum.Application.Forum.Commands.Categories;
using ProjForum.Forum.Application.Forum.Queries.Categories;
using ProjForum.Forum.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjForum.Forum.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [SwaggerOperation(OperationId = "GetCategoryById")]
    public async Task<ActionResult<CategoryModel>> GetCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await mediator.Send(new GetCategoryByIdQuery { Id = id }, cancellationToken);
        if (category is null)
            return NotFound(new OperationResultDto(false, "Category not found"));

        return Ok(category);
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "GetAllCategories")]
    public async Task<ActionResult<IReadOnlyList<CategoryModel>>> GetCategoriesAsync(
        CancellationToken cancellationToken)
    {
        var categories = await mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "CreateCategory")]
    public async Task<ActionResult<Unit>> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpPatch]
    [SwaggerOperation(OperationId = "UpdateCategory")]
    public async Task<ActionResult<Unit>> UpdateCategory([FromBody] UpdateCategoryCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpDelete]
    [SwaggerOperation(OperationId = "DeleteCategory")]
    public async Task<ActionResult<Unit>> DeleteCategory([FromBody] DeleteCategoryCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}