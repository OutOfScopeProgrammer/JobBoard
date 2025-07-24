
using FluentValidation;

namespace JobBoard.Shared.EndpointFilters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model is null)
            return Results.BadRequest();

        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is not null)
        {
            var result = validator.Validate(model);
            if (!result.IsValid)
            {
                var error = ValidationErrorBuilder(result.Errors);
                return Results.BadRequest(error);
            }
        }
        return await next(context);
    }



    private Dictionary<string, string> ValidationErrorBuilder(List<FluentValidation.Results.ValidationFailure> errors)
    {
        var dict = new Dictionary<string, string>();
        errors.ForEach(e => dict.Add(e.PropertyName, e.ErrorMessage));
        return dict;
    }
}
