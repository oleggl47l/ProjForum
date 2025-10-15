using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Identity.Application.DTOs.Role;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.CreateRole;

public record CreateRoleCommand(string Name, bool IsActive) : IRequest<CreateRoleResultDto>;