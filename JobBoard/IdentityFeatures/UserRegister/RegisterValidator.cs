using System.ComponentModel;
using FluentValidation;
using JobBoard.Domain.Entities;
using JobBoard.Identity.UserRegister;
using JobBoard.Infrastructure.Auth;

namespace JobBoard.IdentityFeatures.UserRegister;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(r => r.Email)
            .EmailAddress().WithMessage("email address is not valid.")
            .NotEmpty().WithMessage("email address can't be empty.")
            .NotNull().WithMessage("email address can't be null.");

        RuleFor(r => r.Password)
            .NotNull().WithMessage("password can't be empty.")
            .NotEmpty().WithMessage("password can't be null.")
            .MinimumLength(8).WithMessage("password must be at least 8 characters long.")
            .Must(ContainsUppercase).WithMessage("password must contain at least one uppercase letter.")
            .Must(ContainsDigit).WithMessage("password must contain at least one digit.")
            .Must(ContainLowercase).WithMessage("password must contain at least one lowercase character.");

        _ = RuleFor(r => r.RoleName)
            .Must(RoleName => RoleName.ToUpper() == ApplicationRoles.ADMIN.ToString()
                    || RoleName.ToUpper() == ApplicationRoles.EMPLOYEE.ToString()
                    || RoleName.ToUpper() == ApplicationRoles.APPLICANT.ToString())
                    .WithMessage("role is invalid.");
    }


    private bool ContainsUppercase(string password)
        => password.Any(char.IsUpper);


    private bool ContainsDigit(string password)
        => password.Any(char.IsDigit);
    private bool ContainLowercase(string password)
        => password.Any(char.IsLower);
}
