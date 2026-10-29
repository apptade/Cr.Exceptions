using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cr.Exceptions.AspNetCore.Tests.Component;

public sealed class CrExceptionHandlerTests
{
    private static readonly JsonSerializerOptions _prettyJsonOptions = new(JsonSerializerOptions.Web)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly ITestOutputHelper _output;

    public CrExceptionHandlerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public Task Should_Return_404_For_NotFoundException()
    {
        return AssertHandlerResult(new NotFoundException([new("Test")]), StatusCodes.Status404NotFound, canCreateActivity: true);
    }

    [Fact]
    public Task Should_Return_500_For_UnhandledException()
    {
        return AssertHandlerResult(new InvalidOperationException(), StatusCodes.Status500InternalServerError, canCreateActivity: true);
    }

    [Fact]
    public Task Should_Return_500_For_UnhandledException_When_ActivityIsMissing()
    {
        return AssertHandlerResult(new InvalidOperationException(), StatusCodes.Status500InternalServerError, canCreateActivity: false);
    }

    private async Task AssertHandlerResult(Exception exception, int expectedStatusCode, bool canCreateActivity)
    {
        using var activity = canCreateActivity ? new Activity("TestActivity").Start() : null;
        using var provider = CreateServiceProvider();
        using var responseStream = new MemoryStream();

        var handler = provider.GetRequiredService<IExceptionHandler>();
        var context = new DefaultHttpContext() { Response = { Body = responseStream } };

        Assert.True(await handler.TryHandleAsync(context, exception, CancellationToken.None));
        AssertHttpContext(context, expectedStatusCode);

        responseStream.Position = 0;

        var problem = await JsonSerializer.DeserializeAsync<TestProblemDetails>(responseStream, JsonSerializerOptions.Web);
        var expectedTraceId = activity?.TraceId.ToHexString() ?? context.TraceIdentifier;

        AssertProblemDetails(exception, problem, expectedStatusCode, expectedTraceId);

        _output.WriteLine(JsonSerializer.Serialize(problem, _prettyJsonOptions));
    }

    private static void AssertHttpContext(HttpContext context, int expectedStatusCode)
    {
        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        Assert.Contains(System.Net.Mime.MediaTypeNames.Application.ProblemJson, context.Response.ContentType);
    }

    private static void AssertProblemDetails(Exception exception, TestProblemDetails? problem, int expectedStatusCode, string expectedTraceId)
    {
        Assert.NotNull(problem);

        Assert.False(string.IsNullOrEmpty(problem.Type));
        Assert.False(string.IsNullOrEmpty(problem.Title));

        Assert.Equal(expectedStatusCode, problem.Status);

        Assert.True(problem.Extensions.TryGetValue(ProblemDetailsExtensionNames.TraceId, out var traceId));
        Assert.Equal(expectedTraceId, traceId!.ToString());

        if (exception is CrException crException && !crException.Errors.IsDefault && !problem.Errors.IsDefault)
        {
            Assert.True(crException.Errors.SequenceEqual(problem.Errors));
        }
    }

    private static ServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddLogging()
            .AddCrExceptionHandling()
            .BuildServiceProvider();
    }

    private sealed class TestProblemDetails : ProblemDetails
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImmutableArray<CrError> Errors { get; set; }
    }
}