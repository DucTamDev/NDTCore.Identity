using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.API.Filters;

/// <summary>
/// Action filter for validating model state
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    /// <summary>
    /// Executes the action filter asynchronously
    /// </summary>
    /// <param name="context">The action executing context</param>
    /// <param name="next">The action execution delegate</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse<object>.Failure(
                message: "Validation failed",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: errors.ToDictionary(e => "General", e => new List<string> { e }));

            response.TraceId = context.HttpContext.TraceIdentifier;

            context.Result = new BadRequestObjectResult(response);

            return;
        }

        await next();
    }
}