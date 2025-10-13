using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Posts;

public class GetAllPostsQuery : IRequest<IEnumerable<PostModel>>;