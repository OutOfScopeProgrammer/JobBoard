using FluentValidation;
using JobBoard.CvFeatures.CreateCv;

namespace JobBoard;

public class CreateCvValidator : AbstractValidator<CreateCvDto>
{
    public CreateCvValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(c => c.City).NotEmpty().WithMessage("city can't be empty.")
            .NotNull().WithMessage("city can't be null.")
            .MaximumLength(50).WithMessage("can't be longer than 50.");

        RuleFor(c => c.FullAddress).MaximumLength(300).WithMessage("can't be longer than 300.");
        RuleFor(c => c.FullName).MaximumLength(300).WithMessage("can't be longer than 300.");
        RuleFor(c => c.ExpectedSalary).GreaterThan(0).WithMessage("salary must be greater than 0.");
    }
}
