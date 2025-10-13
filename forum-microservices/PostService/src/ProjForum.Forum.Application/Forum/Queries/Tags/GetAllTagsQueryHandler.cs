using MediatR;
using ProjForum.Forum.Domain.Interfaces;
using ProjForum.Forum.Domain.Models;

namespace ProjForum.Forum.Application.Forum.Queries.Tags;

public class GetAllTagsQueryHandler(ITagRepository tagRepository)
    : IRequestHandler<GetAllTagsQuery, IEnumerable<TagModel>>
{
    public async Task<IEnumerable<TagModel>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await tagRepository.GetAllAsync();

        var tagModels = tags.Select(tag => new TagModel
        {
            Id = tag.Id,
            Name = tag.Name
        });
        
        return tagModels;
    }
}