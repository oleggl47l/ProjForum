using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Identity.Application.Identity.Commands.Roles.CreateRole;
using ProjForum.Identity.Application.Identity.Commands.Roles.DeleteRole;
using ProjForum.Identity.Application.Identity.Commands.Roles.UpdateRole;
using ProjForum.Identity.Application.Identity.Queries.Roles.GetAllRoles;
using ProjForum.Identity.Application.Identity.Queries.Roles.GetRole;

namespace ProjForum.Identity.Api.Controllers.V1;

[Route("api/v1/[controller]/[action]")]
[ApiController]

//TODO:  update controller
public class RoleController(IMediator mediator) : ControllerBase
{
    // [HttpGet("{id}")]
    // public async Task<ActionResult> GetById(string id)
    // {
    //     var role = await mediator.Send(new GetRoleQuery { RoleId = id });
    //     return Ok(role);
    // }
    //
    // [HttpGet]
    // public async Task<ActionResult> GetAll()
    // {
    //     var roles = await mediator.Send(new GetAllRolesQuery());
    //     return Ok(roles);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
    // {
    //     var result = await mediator.Send(command);
    //     return Ok(result);
    // }
    //
    // [HttpPatch]
    // public async Task<IActionResult> Update([FromBody] UpdateRoleCommand command)
    // {
    //     var result = await mediator.Send(command);
    //     return Ok(result);
    // }
    //
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(string id)
    // {
    //     await mediator.Send(new DeleteRoleCommand { RoleId = id });
    //     return Ok();
    // }
}