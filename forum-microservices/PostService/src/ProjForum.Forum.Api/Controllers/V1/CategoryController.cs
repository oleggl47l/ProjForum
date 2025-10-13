using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Forum.Application.Forum.Commands.Categories;
using ProjForum.Forum.Application.Forum.Queries.Categories;

namespace ProjForum.Forum.Api.Controllers.V1;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryAsync(Guid id)
    {
        var category = await mediator.Send(new GetCategoryByIdQuery { Id = id });
        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var categories = await mediator.Send(new GetAllCategoriesQuery());
        return Ok(categories);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCategory([FromBody] DeleteCategoryCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}