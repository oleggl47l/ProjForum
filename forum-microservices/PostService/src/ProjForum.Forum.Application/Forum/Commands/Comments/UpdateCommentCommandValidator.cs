using FluentValidation;

namespace ProjForum.Forum.Application.Forum.Commands.Comments;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Comment ID is required.");
    }
}