using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProjForum.Identity.Api.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
//TODO:  update controller
public class AuthController(IMediator mediator) : ControllerBase
{
    // [HttpPost("login")]
    // public async Task<IActionResult> Login([FromBody] LoginQuery query)
    // {
    //     var result = await mediator.Send(query);
    //     return Ok(result);
    // }
}