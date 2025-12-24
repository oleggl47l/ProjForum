using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Comments;

public class GetAllCommentsQuery : IRequest<IEnumerable<CommentModel>>;