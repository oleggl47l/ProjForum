using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Tags;

public class UpdateTagCommandHandler(ITagRepository tagRepository) : IRequestHandler<UpdateTagCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByIdAsync(request.Id);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with id {request.Id} not found");

        if (request.Name != null && await tagRepository.TagExistsByNameAsync(request.Name))
            throw new InvalidOperationException($"Tag with name '{request.Name}' already exists.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            tag.Name = request.Name;

        await tagRepository.Update(tag);

        return Unit.Value;
    }
}