using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class AddTagToPostCommandHandler(IPostRepository postRepository, ITagRepository tagRepository) : IRequestHandler<AddTagToPostCommand, Unit>
{
    public async Task<Unit> Handle(AddTagToPostCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.PostId);
        if (post == null)
            throw new KeyNotFoundException($"Post with id {request.PostId} not found.");

        var tag = await tagRepository.GetByIdAsync(request.TagId);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with id {request.TagId} not found.");

        await postRepository.AddTagToPostAsync(request.PostId, request.TagId);

        return Unit.Value;
    }
}