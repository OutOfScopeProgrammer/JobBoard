using FluentValidation;
using JobBoard.IdentityFeature.UserLogin;

namespace JobBoard.IdentityFeatures.UserLogin;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(l => l.Email)
                .EmailAddress().WithMessage("email address is not valid.")
                .NotEmpty().WithMessage("email address can't be empty.")
                .NotNull().WithMessage("email address can't be null.");

        RuleFor(l => l.Password)
                .NotNull().WithMessage("password can't be empty.")
                .NotEmpty().WithMessage("password can't be null.");
    }

}
