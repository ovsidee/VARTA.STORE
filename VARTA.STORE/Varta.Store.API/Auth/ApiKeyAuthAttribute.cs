using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Varta.Store.API.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string potentialApiKey = null;

        // 1. Try to get from Header
        if (context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var headerApiKey))
        {
            potentialApiKey = headerApiKey;
        }

        // 2. If not in header, try Query Parameter (DayZ friendly)
        if (string.IsNullOrEmpty(potentialApiKey) && context.HttpContext.Request.Query.ContainsKey("apiKey"))
        {
            potentialApiKey = context.HttpContext.Request.Query["apiKey"];
        }

        if (string.IsNullOrEmpty(potentialApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API Key is missing. Provide it via 'X-Api-Key' header or 'apiKey' query parameter.");
            return;
        }

        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = configuration["GameApi:Key"];

        if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(potentialApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key.");
            return;
        }

        await next();
    }
}
