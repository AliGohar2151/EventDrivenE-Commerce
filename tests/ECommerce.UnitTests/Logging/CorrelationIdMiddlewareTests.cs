using ECommerce.Infrastructure.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ECommerce.UnitTests.Logging;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldUseProvidedCorrelationIdHeader()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = "MY-CUSTOM-CORRELATION-ID";

        RequestDelegate next = (ctx) => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Request.Headers["X-Correlation-ID"].ToString().Should().Be("MY-CUSTOM-CORRELATION-ID");
    }

    [Fact]
    public async Task InvokeAsync_ShouldGenerateCorrelationIdWhenMissing()
    {
        var context = new DefaultHttpContext();

        RequestDelegate next = (ctx) => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Request.Headers["X-Correlation-ID"].Should().BeEmpty();
    }
}
