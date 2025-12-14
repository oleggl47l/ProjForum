using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.Auth;
using ProjForum.Identity.Application.Identity.Commands.Auth.GetCurrentUser;
using ProjForum.Identity.Application.Identity.Commands.Auth.Login;
using ProjForum.Identity.Application.Identity.Commands.Auth.Logout;
using ProjForum.Identity.Application.Identity.Commands.Auth.Register;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjForum.Identity.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpGet("getCurrentUser")]
    [Authorize]
    [SwaggerOperation(OperationId = "GetCurrentUser", Summary = "Get current user",
        Description = "Gives information about the current user.")]
    public async Task<ActionResult<GetCurrentUserResultDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrentUserCommand(HttpContext.User.Claims), cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(OperationId = "LoginUser", Summary = "Login user",
        Description = "Logs in a user and returns JWT and refresh token.")]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(OperationId = "RegisterUser", Summary = "Register user",
        Description = "Registers a new user with specified roles.")]
    public async Task<ActionResult<OperationResultDto>> Register([FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(OperationId = "LogoutUser", Summary = "Logout user",
        Description = "Logs out the currently authenticated user.")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await mediator.Send(new LogoutCommand(), cancellationToken);
        return NoContent();
    }
}