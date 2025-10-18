using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.User;
using ProjForum.Identity.Application.Identity.Commands.Users.AddRoleToUser;
using ProjForum.Identity.Application.Identity.Commands.Users.BlockUser;
using ProjForum.Identity.Application.Identity.Commands.Users.CreateUser;
using ProjForum.Identity.Application.Identity.Commands.Users.DeleteUser;
using ProjForum.Identity.Application.Identity.Commands.Users.RemoveRoleFromUser;
using ProjForum.Identity.Application.Identity.Commands.Users.UnblockUser;
using ProjForum.Identity.Application.Identity.Commands.Users.UpdateUser;
using ProjForum.Identity.Application.Identity.Queries.Users.GetPagedUsers;
using ProjForum.Identity.Application.Identity.Queries.Users.GetUserByEmail;
using ProjForum.Identity.Application.Identity.Queries.Users.GetUserById;
using ProjForum.Identity.Application.Identity.Queries.Users.GetUsersByRoleId;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjForum.Identity.Api.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
// TODO: придумать, что с какими правами юзать
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить пользователя по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(OperationId = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Получить пользователя по Email
    /// </summary>
    [HttpGet("by-email")]
    [SwaggerOperation(OperationId = "GetUserByEmail")]
    public async Task<ActionResult<UserDto>> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByEmailQuery(email), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Получить список пользователей постранично
    /// </summary>
    [HttpGet("paged")]
    [SwaggerOperation(OperationId = "GetPagedUsers")]
    public async Task<ActionResult<PagedUsersDto>> GetPaged(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPagedUsersQuery(pageIndex, pageSize), cancellationToken);
        return Ok(result);
    }


    /// <summary>
    /// Получить пользователей по Id роли
    /// </summary>
    [HttpGet("by-role/{roleId:guid}")]
    [SwaggerOperation(OperationId = "GetUsersByRoleId")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetByRoleId(Guid roleId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUsersByRoleIdQuery(roleId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateUser")]
    public async Task<ActionResult<CreateUserResultDto>> Create([FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.User.Id }, result);
    }

    /// <summary>
    /// Обновить пользователя
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(OperationId = "UpdateUser")]
    public async Task<ActionResult<UpdateUserResultDto>> Update(Guid id, [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { UserId = id };
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(OperationId = "DeleteUser")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Заблокировать пользователя
    /// </summary>
    [HttpPost("{id:guid}/block")]
    [SwaggerOperation(OperationId = "BlockUser")]
    public async Task<ActionResult<OperationResultDto>> Block(Guid id, [FromQuery] int minutes,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new BlockUserCommand(id, minutes), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Разблокировать пользователя
    /// </summary>
    [HttpPost("{id:guid}/unblock")]
    [SwaggerOperation(OperationId = "UnblockUser")]
    public async Task<ActionResult<OperationResultDto>> Unblock(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UnblockUserCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Добавить роль пользователю
    /// </summary>
    [HttpPost("{id:guid}/roles/add")]
    [SwaggerOperation(OperationId = "AddRoleToUser")]
    public async Task<ActionResult<OperationResultDto>> AddRole(Guid id, [FromQuery] string roleName,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddRoleToUserCommand(id, roleName), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Удалить роль у пользователя
    /// </summary>
    [HttpPost("{id:guid}/roles/remove")]
    [SwaggerOperation(OperationId = "RemoveRoleFromUser")]
    public async Task<ActionResult<OperationResultDto>> RemoveRole(Guid id, [FromQuery] string roleName,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveRoleFromUserCommand(id, roleName), cancellationToken);
        return Ok(result);
    }
}
