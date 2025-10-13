using FluentValidation;
using ProjForum.Forum.Domain.Interfaces;

namespace ProjForum.Forum.Application.Forum.Commands.Posts;

public class AddTagToPostCommandValidator : AbstractValidator<AddTagToPostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;

    public AddTagToPostCommandValidator(IPostRepository postRepository, ITagRepository tagRepository)
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;

        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("Post ID is required.")
            .MustAsync(PostExists).WithMessage("Post does not exist.");

        RuleFor(x => x.TagId)
            .NotEmpty().WithMessage("Tag ID is required.")
            .MustAsync(TagExists).WithMessage("Tag does not exist.");
        
        RuleFor(x => new { x.PostId, x.TagId })
            .MustAsync(TagNotYetAssignedToPost)
            .WithMessage("The post already contains this tag.");
    }

    private async Task<bool> PostExists(Guid postId, CancellationToken cancellationToken)
    {
        return await _postRepository.GetByIdAsync(postId) != null;
    }

    private async Task<bool> TagExists(Guid tagId, CancellationToken cancellationToken)
    {
        return await _tagRepository.GetByIdAsync(tagId) != null;
    }
    
    private async Task<bool> TagNotYetAssignedToPost(dynamic args, CancellationToken cancellationToken)
    {
        var postId = (Guid)args.PostId;
        var tagId = (Guid)args.TagId;

        var postTags = await _postRepository.GetTagsByPostIdAsync(postId);
        return postTags.All(t => t.Id != tagId);
    }
}