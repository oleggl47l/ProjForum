using MediatR;
using ProjForum.Identity.Application.DTOs.User;

namespace ProjForum.Identity.Application.Identity.Queries.Users.GetPagedUsers;

public record GetPagedUsersQuery(int PageIndex, int PageSize) : IRequest<PagedUsersDto>;
