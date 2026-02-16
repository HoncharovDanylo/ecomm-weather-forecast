using Infrastructure.Configs;
using Microsoft.AspNetCore.Authorization;

namespace WeatherForecast.Api.AuthorizationHandlers.ApiKeyAuthorization;

public class ApiKeyAuthorizationHandler : AuthorizationHandler<ApiKeyAuthorizationRequirement>
{
    private const string AuthorizationHeaderName = "X-Api-Key";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMainConfiguration _configuration;

    public ApiKeyAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IMainConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyAuthorizationRequirement requirement)
    {
            var httpContext = _httpContextAccessor.HttpContext;
            if (!httpContext.Request.Headers.TryGetValue(AuthorizationHeaderName, out var receivedApiKey))
            {
                context.Fail();
                throw new Exception("Access is not allowed"); //TODO: Change to UnauthorizedException
            }

            if(receivedApiKey == _configuration.InternalApiKey)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
                throw new Exception("Access is not allowed"); //TODO: Change to UnauthorizedException
            }
            return Task.CompletedTask;
    }
}