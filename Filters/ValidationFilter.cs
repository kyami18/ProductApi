using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductApi.Extensions;

namespace ProductApi.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(argument.GetType());

            var validator = context.HttpContext.RequestServices
                .GetService(validatorType) as IValidator;

            if (validator is null)
                continue;

            var validationContext =
                new ValidationContext<object>(argument);

            var validationResult = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                context.Result = new BadRequestObjectResult(
                    validationResult.ToApiResponse());

                return;
            }
        }

        await next();
    }
}