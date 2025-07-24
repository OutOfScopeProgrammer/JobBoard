
using FluentValidation;
using JobBoard.IdentityFeature.PasswordReset;

namespace JobBoard.IdentityFeatures.PasswordReset;

public class PasswordResetValidator : AbstractValidator<ResetDto>
{

    public PasswordResetValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.Email)
                .EmailAddress().WithMessage("email address is not valid.")
                .NotEmpty().WithMessage("email address can't be empty.")
                .NotNull().WithMessage("email address can't be null.");

        RuleFor(r => r.NewPassword)
                .NotNull().WithMessage("password can't be empty.")
                .NotEmpty().WithMessage("password can't be null.")
                .MinimumLength(8).WithMessage("password must be at least 8 characters long.")
                .Must(ContainsUppercase).WithMessage("password must contain at least one uppercase letter.")
                .Must(ContainsDigit).WithMessage("password must contain at least one digit.")
                .Must(ContainLowercase).WithMessage("password must contain at least one lowercase character.");

        RuleFor(r => r.OldPassword)
            .NotNull().WithMessage("old password can't be empty.")
            .NotEmpty().WithMessage("old password can't be null.");
    }


    private bool ContainsUppercase(string password)
        => password.Any(char.IsUpper);


    private bool ContainsDigit(string password)
        => password.Any(char.IsDigit);
    private bool ContainLowercase(string password)
        => password.Any(char.IsLower);
}
