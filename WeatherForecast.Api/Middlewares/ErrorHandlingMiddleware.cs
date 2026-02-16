using ClickCredit.Foundation.Exceptions;
using ClickCredit.Foundation.Exceptions.Models;
using WeatherForecast.Api.Exceptions;

namespace WeatherForecast.Api.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError($"Error occurred: {exception}");

        switch (exception)
        {
            case UnauthorizedException ex:
                await WriteErrorAsync(context, ex.Message, HttpStatusCode.Unauthorized);
                break;
            case BusinessLogicException ex:
                await WriteErrorAsync(context, ex.Message, HttpStatusCode.NotFound);
                break;
            case BadRequestException ex:
                await WriteErrorAsync(context, ex.Message, HttpStatusCode.BadRequest);
                break;
            case MethodNotAllowedException ex:
                await WriteErrorAsync(context, ex.Message, HttpStatusCode.MethodNotAllowed);
                break;
            case ConflictException ex:
                await WriteErrorAsync(context, ex.Message, HttpStatusCode.Conflict);
                break;
            case ApiException ex:
                await WriteErrorAsync(context, ex.Message, HttpStatusCode.BadGateway);
                break;
            default:
                await WriteErrorAsync(context, "Something went wrong.", HttpStatusCode.InternalServerError);
                break;
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, string error, HttpStatusCode statusCode)
    {
        var result = JsonConvert.SerializeObject(new ExceptionResponse { Error = error });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(result);
    }

}
