using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NDTCore.Identity.Contracts.Responses;

namespace NDTCore.Identity.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var response = ApiResponse<object>.FailureResponse("Validation failed", 400, errors);

                response.TraceId = context.HttpContext.TraceIdentifier;

                context.Result = new BadRequestObjectResult(response);
                return;
            }

            await next();
        }
    }
}