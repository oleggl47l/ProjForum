using System.Security.Claims;
using MediatR;
using ProjForum.Identity.Application.DTOs.Auth;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.GetCurrentUser;

public record GetCurrentUserCommand(IEnumerable<Claim> claims) : IRequest<GetCurrentUserResultDto>;