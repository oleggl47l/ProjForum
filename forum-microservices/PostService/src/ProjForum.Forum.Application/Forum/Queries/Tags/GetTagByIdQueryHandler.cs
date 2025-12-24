using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Tags;

public class GetTagByIdQueryHandler(ITagRepository tagRepository) :
    IRequestHandler<GetTagByIdQuery, TagModel?>
{
    public async Task<TagModel?> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByIdAsync(request.Id);
        return tag is null
            ? null
            : new TagModel
            {
                Id = tag.Id,
                Name = tag.Name,
            };
    }
}