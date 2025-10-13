using MediatR;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class DeletePostCommandHandler(IPostRepository postRepository) : IRequestHandler<DeletePostCommand, Unit>
{
    public async Task<Unit> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.Id);
        if (post == null)
            throw new KeyNotFoundException($"Post with id {request.Id} not found.");

        await postRepository.Remove(post.Id);
        return Unit.Value;
    }
}