using MediatR;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Tags;

public class GetAllTagsQuery : IRequest<IEnumerable<TagModel>>;