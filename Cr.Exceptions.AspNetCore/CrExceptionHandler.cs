using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cr.Exceptions.AspNetCore;

public sealed class CrExceptionHandler : IExceptionHandler
{
    private static readonly ImmutableArray<CrError> DefaultInternalErrors = [new("InternalError", "An unexpected internal error occurred.")];

    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<CrExceptionHandler> _logger;
    private readonly StatusCodeMap _statusCodeMap;
    private readonly LogLevelMap _logLevelMap;

    public CrExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<CrExceptionHandler> logger,
        StatusCodeMap statusCodeMap,
        LogLevelMap logLevelMap)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
        _statusCodeMap = statusCodeMap;
        _logLevelMap = logLevelMap;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            _logger.LogResponseAlreadyStarted(exception);
            return ValueTask.FromResult(false);
        }

        var errors = DefaultInternalErrors;
        var statusCode = StatusCodes.Status500InternalServerError;

        var exceptionType = exception.GetType();
        var exceptionTypeName = exceptionType.FullName ?? exceptionType.Name;

        if (exception is CrException crException)
        {
            errors = crException.Errors;

            if (_statusCodeMap.TryFind(crException, out var code))
            {
                statusCode = code;
            }
            else
            {
                _logger.LogMissingHttpStatusMapping(exception, exceptionTypeName);
            }

            if (!_logLevelMap.TryFind(crException, out var logLevel))
            {
                logLevel = LogLevel.Debug;
                _logger.LogMissingLogLevelMapping(exceptionTypeName, logLevel);
            }

            _logger.LogCrExceptionOccurred(logLevel, exception, exceptionTypeName);
        }
        else
        {
            _logger.LogUnknownException(exception, exceptionTypeName);
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Status = statusCode, },
        };

        if (!errors.IsDefaultOrEmpty) AddProblemDetailsExtension(problemDetailsContext.ProblemDetails, ProblemDetailsExtensionNames.Errors, errors);

        return TryWriteResponseAsync(exception, problemDetailsContext);
    }

    private void AddProblemDetailsExtension(ProblemDetails problemDetails, string key, object? value)
    {
        if (!problemDetails.Extensions.TryAdd(key, value))
        {
            problemDetails.Extensions[key] = value;
            _logger.LogProblemDetailsExtensionOverwritten(key);
        }
    }

    private async ValueTask<bool> TryWriteResponseAsync(Exception exception, ProblemDetailsContext problemDetailsContext)
    {
        var isWritten = await _problemDetailsService.TryWriteAsync(problemDetailsContext);

        if (!isWritten)
        {
            _logger.LogFailedToWriteProblemDetails(exception);
        }

        return isWritten;
    }
}