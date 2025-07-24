using FluentValidation;
using JobBoard.UserFeature.ApplyToJob;

namespace JobBoard.JobApplicationFeatures.ApplyToJob;

public class ApplyToJobValidator : AbstractValidator<ApplyToJobDto>
{
    public ApplyToJobValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(a => a.Description)
           .NotNull().WithMessage("description can't be null.")
           .NotEmpty().WithMessage("description can't be empty.")
           .MaximumLength(500).WithMessage("description's length must be less than 500.");

        RuleFor(a => a.JobId)
                .NotNull().WithMessage("jobId can't be null.")
                .NotEmpty().WithMessage("jobId can't be empty.");

    }
}
