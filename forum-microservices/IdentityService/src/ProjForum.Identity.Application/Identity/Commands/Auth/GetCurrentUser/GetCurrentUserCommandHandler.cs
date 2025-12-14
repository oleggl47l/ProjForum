using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.Auth;

namespace ProjForum.Identity.Application.Identity.Commands.Auth.GetCurrentUser;

public class GetCurrentUserCommandHandler : IRequestHandler<GetCurrentUserCommand, GetCurrentUserResultDto>
{
    public async Task<GetCurrentUserResultDto> Handle(GetCurrentUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var claimsList = request.claims.ToList();

            var userIdString = claimsList
                .FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier or JwtRegisteredClaimNames.Sub)?.Value;

            Guid userId;
            if (!userIdString.IsNullOrEmpty())
            {
                var parseResult = Guid.TryParse(userIdString, out userId);
                if (!parseResult)
                {
                    throw new FormatException("Invalid user id");
                }
            }
            else
            {
                userId = Guid.Empty;
            }

            var userName = claimsList
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var userRoles = claimsList
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return new GetCurrentUserResultDto(
                new OperationResultDto(true, "Data received successfully"),
                new CurrentUserDto(
                    userId,
                    userName ?? string.Empty,
                    userRoles)
            );
        }
        catch (FormatException e)
        {
            return new GetCurrentUserResultDto(
                new OperationResultDto(false, e.Message)
            );
        }

        catch (Exception e)
        {
            return new GetCurrentUserResultDto(
                new OperationResultDto(false, $"Something went wrong: {e.Message}")
            );
        }
    }
}