using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Identity.Application.Identity.Commands.Users.AddRoleToUser;
using ProjForum.Identity.Application.Identity.Commands.Users.BlockUser;
using ProjForum.Identity.Application.Identity.Commands.Users.CreateUser;
using ProjForum.Identity.Application.Identity.Commands.Users.DeleteUser;
using ProjForum.Identity.Application.Identity.Commands.Users.RemoveRoleFromUser;
using ProjForum.Identity.Application.Identity.Commands.Users.UnblockUser;
using ProjForum.Identity.Application.Identity.Commands.Users.UpdateUser;
using ProjForum.Identity.Application.Identity.Queries.Users.GetAllUsers;
using ProjForum.Identity.Application.Identity.Queries.Users.GetUser;
using ProjForum.Identity.Application.Identity.Queries.Users.GetUsersByRoleId;

namespace ProjForum.Identity.Api.Controllers.V1;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet("/api/v1/[controller]/{id}")]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var user = await mediator.Send(new GetUserQuery { UserId = id });
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpGet("{roleId}/users")]
    public async Task<IActionResult> GetUsersByRoleId([FromRoute] string roleId)
    {
        var users = await mediator.Send(new GetUsersByRoleIdQuery { RoleId = roleId });
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await mediator.Send(new DeleteUserCommand { UserId = id });
        return NoContent();
    }

    [HttpPost("{userId}/roles/{roleName}")]
    public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
    {
        await mediator.Send(new AddRoleToUserCommand { UserId = userId, RoleName = roleName });
        return Ok("Role added successfully");
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
    {
        await mediator.Send(new RemoveRoleFromUserCommand { UserId = userId, RoleName = roleName });
        return Ok("Role removed successfully");
    }

    [HttpPut]
    public async Task<IActionResult> Block([FromBody] BlockUserCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Unblock([FromBody] UnblockUserCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}