using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class RemoveTagFromPostCommandHandler(IPostRepository postRepository, ITagRepository tagRepository)
    : IRequestHandler<RemoveTagFromPostCommand, Unit>
{
    public async Task<Unit> Handle(RemoveTagFromPostCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.PostId);
        if (post == null)
            throw new KeyNotFoundException($"Post with id {request.PostId} not found.");

        var tag = await tagRepository.GetByIdAsync(request.TagId);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with id {request.TagId} not found.");

        await postRepository.RemoveTagFromPostAsync(request.PostId, request.TagId);

        return Unit.Value;
    }
}