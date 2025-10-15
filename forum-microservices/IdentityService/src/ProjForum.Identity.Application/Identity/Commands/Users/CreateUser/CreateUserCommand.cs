using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Identity.Application.DTOs.User;

namespace ProjForum.Identity.Application.Identity.Commands.Users.CreateUser;

public record CreateUserCommand(
    string UserName,
    string Email,
    string Password,
    IEnumerable<string> Roles
) : IRequest<CreateUserResultDto>;