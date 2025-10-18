using FluentValidation;

namespace ProjForum.Identity.Application.Identity.Commands.Users.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("The User ID field is required!");

        RuleFor(x => x.UserName)
            .MaximumLength(20).WithMessage("User Name must not exceed 20 characters.")
            .When(x => !string.IsNullOrEmpty(x.UserName));


        RuleFor(x => x.Email)
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        // RuleFor(x => x.NewPassword)
        //     .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
        //     .Matches(@"[A-ZА-Я]").WithMessage("Password must contain at least one uppercase letter.")
        //     .Matches(@"[a-zа-я]").WithMessage("Password must contain at least one lowercase letter.")
        //     .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
        //     .Matches(@"[\W_]")
        //     .WithMessage("Password must contain at least one special character (e.g., !, @, #, $, etc.).");
    }
}