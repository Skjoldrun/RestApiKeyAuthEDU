namespace RestApiKeyAuthEDU.Authentication;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // check if the header from the request has the API key set as header
        if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(AuthConstants.ApiKeyMissing);
            return;
        }

        // check if the transmitted API key matches the configured one
        var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(AuthConstants.ApiKeyInvalid);
            return;
        }

        await _next(context); // continue in the pipeline and leave this middleware
    }
}