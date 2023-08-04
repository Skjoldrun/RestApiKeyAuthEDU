namespace RestApiKeyAuthEDU.Authentication;

public sealed class UnauthorizedHttpObjectResult : IResult, IStatusCodeHttpResult
{
    private readonly object _body;

    public UnauthorizedHttpObjectResult(object body)
    {
        _body = body;
    }

    /// <summary>
    /// Gets the HTTP status code: <see cref="StatusCodes.Status401Unauthorized" />
    /// </summary>
    public int StatusCode => StatusCodes.Status401Unauthorized;

    int? IStatusCodeHttpResult.StatusCode => StatusCode;

    /// <inheritdoc />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));

        httpContext.Response.StatusCode = StatusCode;
        if (_body is string s)
        {
            await httpContext.Response.WriteAsync(s);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(_body);
    }
}