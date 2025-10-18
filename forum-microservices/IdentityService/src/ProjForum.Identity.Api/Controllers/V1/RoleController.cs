using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.Role;
using ProjForum.Identity.Application.Identity.Commands.Roles.CreateRole;
using ProjForum.Identity.Application.Identity.Commands.Roles.DeleteRole;
using ProjForum.Identity.Application.Identity.Commands.Roles.UpdateRole;
using ProjForum.Identity.Application.Identity.Queries.Roles.GetAllRoles;
using ProjForum.Identity.Application.Identity.Queries.Roles.GetRole;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjForum.Identity.Api.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admin")]
public class RolesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить список всех ролей
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(OperationId = "GetAllRoles")]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await mediator.Send(new GetAllRolesQuery(), cancellationToken);
        return Ok(roles);
    }

    /// <summary>
    /// Получить роль по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [SwaggerOperation(OperationId = "GetRoleById")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var role = await mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        if (role is null)
            return NotFound(new OperationResultDto(false, "Role not found"));

        return Ok(role);
    }

    /// <summary>
    /// Создать новую роль
    /// </summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateRole")]
    public async Task<ActionResult<CreateRoleResultDto>> Create([FromBody] CreateRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Role.Id }, result);
    }
    
    /// <summary>
    /// Обновить роль
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(OperationId = "UpdateRole")]
    public async Task<ActionResult<UpdateRoleResultDto>> Update(Guid id, [FromBody] UpdateRoleCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { Id = id };
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Удалить роль
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(OperationId = "DeleteRole")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteRoleCommand(id), cancellationToken);
        return NoContent();
    }
}
