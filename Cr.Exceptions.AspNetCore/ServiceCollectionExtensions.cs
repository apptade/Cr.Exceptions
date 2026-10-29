using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Cr.Exceptions.AspNetCore;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCrExceptionHandling()
        {
            return services
                .AddCrExceptionHandler()
                .AddCrStatusCodeMapping()
                .AddCrLogLevelMapping();
        }

        public IServiceCollection AddCrExceptionHandler()
        {
            return services
                .AddCustomProblemDetails()
                .AddExceptionHandler<CrExceptionHandler>();
        }

        public IServiceCollection AddCrStatusCodeMapping(Action<StatusCodeMapBuilder>? configurator = null)
        {
            configurator ??= static builder => builder.AddDefaultMappings();

            var builder = new StatusCodeMapBuilder();
            configurator(builder);

            return services.AddSingleton(builder.Build());
        }

        public IServiceCollection AddCrLogLevelMapping(Action<LogLevelMapBuilder>? configurator = null)
        {
            configurator ??= static builder => builder.AddDefaultMappings();

            var builder = new LogLevelMapBuilder();
            configurator(builder);

            return services.AddSingleton(builder.Build());
        }

        private IServiceCollection AddCustomProblemDetails()
        {
            return services.AddProblemDetails(static options =>
            {
                options.CustomizeProblemDetails = static context =>
                {
                    var traceId = Activity.Current?.TraceId.ToHexString()
                        ?? context.HttpContext.TraceIdentifier;

                    context.ProblemDetails.Extensions[ProblemDetailsExtensionNames.TraceId] = traceId;
                };
            });
        }
    }
}