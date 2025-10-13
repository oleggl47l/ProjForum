using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Tags;

public class CreateTagCommandHandler(ITagRepository tagRepository)
    : IRequestHandler<CreateTagCommand, Unit>
{
    public async Task<Unit> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        if (await tagRepository.TagExistsByNameAsync(request.Name))
            throw new InvalidOperationException($"Tag with name '{request.Name}' already exists.");
        
        var tag = new Domain.Entities.Tag
        {
            Name = request.Name,
        };

        await tagRepository.AddAsync(tag);

        return Unit.Value;
    }
}