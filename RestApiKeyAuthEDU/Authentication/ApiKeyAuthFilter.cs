using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestApiKeyAuthEDU.Authentication;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // check if the header from the request has the API key set as header
        // Use context.HttpContext to access the request context here
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(AuthConstants.ApiKeyMissing);
            return;
        }

        // check if the transmitted API key matches the configured one
        var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
        if (!apiKey.Equals(extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(AuthConstants.ApiKeyInvalid);
            return;
        }

        // The filter returns and end the call if key auth fails or just continues.
        // Call for next is not needed here.
    }
}