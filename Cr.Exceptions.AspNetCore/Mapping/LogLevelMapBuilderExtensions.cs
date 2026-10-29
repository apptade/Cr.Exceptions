using Microsoft.Extensions.Logging;

namespace Cr.Exceptions.AspNetCore;

public static class LogLevelMapBuilderExtensions
{
    extension(LogLevelMapBuilder builder)
    {
        public LogLevelMapBuilder AddDefaultMappings()
        {
            return builder
                .Map<CrException>(LogLevel.Debug)
                .Map<InternalException>(LogLevel.Error);
        }
    }
}