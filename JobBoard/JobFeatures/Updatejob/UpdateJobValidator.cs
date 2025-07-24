using FluentValidation;

namespace JobBoard.JobFeatures.Updatejob;

public class UpdateJobValidator : AbstractValidator<UpdateJobDto>
{
    public UpdateJobValidator()
    {

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(j => j.Title).MaximumLength(100).WithMessage("title's length is greater than 100.");
        RuleFor(j => j.Description).MaximumLength(500).WithMessage("description's length must be less than 500.");

        RuleFor(j => j.JobId)
            .NotNull().WithMessage("jobId can't be null.")
            .NotEmpty().WithMessage("jobId can't be empty.");


    }

}
