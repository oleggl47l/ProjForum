using FluentValidation;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("Post id is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");
    }
}