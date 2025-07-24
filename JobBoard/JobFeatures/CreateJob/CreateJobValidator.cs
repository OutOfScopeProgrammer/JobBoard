using FluentValidation;

namespace JobBoard.JobFeatures.CreateJob;

public class CreateJobValidator : AbstractValidator<CreateJobDto>
{

    public CreateJobValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(j => j.Title)
            .NotEmpty().WithMessage("title can't be null.")
            .NotNull().WithMessage("title can't be empty.")
            .MaximumLength(100).WithMessage("title's length is greater than 100.");


        RuleFor(j => j.Description)
            .NotNull().WithMessage("description can't be null.")
            .NotEmpty().WithMessage("description can't be empty.")
            .MaximumLength(500).WithMessage("description's length must be less than 500.");


        RuleFor(j => j.Salary)
            .NotNull().WithMessage("salary can't be null.")
            .NotEmpty().WithMessage("salart can't be empty.")
            .GreaterThan(0).WithMessage("salary must be greater than 0.");
    }
}
