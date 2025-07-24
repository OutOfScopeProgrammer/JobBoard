using FluentValidation;
using JobBoard.Domain.Enums;

namespace JobBoard.JobApplicationFeatures.ChangeApplicationStatus;

public class ApplicationStatusValidator : AbstractValidator<ApplicationStatusDto>
{
    public ApplicationStatusValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(a => a.Status)
            .NotNull().WithMessage("status can't be null.")
            .NotEmpty().WithMessage("status can't be empty.")
            .Must(status => status == Status.Rejected
                    || status == Status.Accepted
                    || status == Status.Submitted
                    || status == Status.UnderReview)
            .WithMessage("status is not valid.");
    }

}
