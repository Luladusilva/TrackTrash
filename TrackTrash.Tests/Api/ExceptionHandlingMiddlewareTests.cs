using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using TrackTrashAPI.Middleware;

namespace TrackTrash.Tests.Api;

public class ExceptionHandlingMiddlewareTests
{
    [Theory]
    [InlineData(typeof(InvalidOperationException), 400)]
    [InlineData(typeof(KeyNotFoundException), 404)]
    [InlineData(typeof(UnauthorizedAccessException), 401)]
    public async Task InvokeAsync_MapsKnownExceptionToStatusCodeAndMessage(Type exceptionType, int expectedStatusCode)
    {
        var exception = (Exception)Activator.CreateInstance(exceptionType, "expected message")!;
        var context = CreateContext();
        var middleware = CreateMiddleware(_ => throw exception);

        await middleware.InvokeAsync(context);

        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        Assert.Equal("expected message", await ReadErrorAsync(context));
    }

    [Fact]
    public async Task InvokeAsync_HidesMessageOfUnexpectedException()
    {
        var context = CreateContext();
        var middleware = CreateMiddleware(_ => throw new ArgumentException("internal detail"));

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("An unexpected error occurred.", await ReadErrorAsync(context));
    }

    [Fact]
    public async Task InvokeAsync_HidesMessageOfWrappedProviderException()
    {
        var context = CreateContext();
        var wrapped = new InvalidOperationException("provider detail", new TimeoutException("connection refused"));
        var middleware = CreateMiddleware(_ => throw wrapped);

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("An unexpected error occurred.", await ReadErrorAsync(context));
    }

    [Fact]
    public async Task InvokeAsync_LeavesSuccessfulResponseUntouched()
    {
        var context = CreateContext();
        var middleware = CreateMiddleware(httpContext =>
        {
            httpContext.Response.StatusCode = 204;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        Assert.Equal(204, context.Response.StatusCode);
    }

    private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new ExceptionHandlingMiddleware(next, NullLogger<ExceptionHandlingMiddleware>.Instance);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string?> ReadErrorAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        return document.RootElement.GetProperty("error").GetString();
    }
}
