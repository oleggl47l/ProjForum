using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Identity.Application.DTOs.Role;

namespace ProjForum.Identity.Application.Identity.Commands.Roles.UpdateRole;

public record UpdateRoleCommand(Guid Id, string Name, bool IsActive) : IRequest<UpdateRoleResultDto>;