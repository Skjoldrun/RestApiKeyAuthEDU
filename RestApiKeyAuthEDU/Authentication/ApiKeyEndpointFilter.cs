namespace RestApiKeyAuthEDU.Authentication;

public class ApiKeyEndpointFilter : IEndpointFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyEndpointFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            return new UnauthorizedHttpObjectResult(AuthConstants.ApiKeyMissing);
        }

        // check if the transmitted API key matches the configured one
        var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
        if (!apiKey.Equals(extractedApiKey))
        {
            return new UnauthorizedHttpObjectResult(AuthConstants.ApiKeyInvalid);
        }

        return await next(context);
    }
}