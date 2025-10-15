using MediatR;
using ProjForum.Identity.Application.DTOs.User;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;