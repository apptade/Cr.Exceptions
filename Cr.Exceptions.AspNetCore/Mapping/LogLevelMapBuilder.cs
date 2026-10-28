using Microsoft.Extensions.Logging;

namespace Cr.Exceptions.AspNetCore;

public class LogLevelMapBuilder : MapBuilder<Type, LogLevel>
{
    public LogLevelMapBuilder Map<TException>(LogLevel logLevel) where TException : CrException
    {
        ThrowIfInvalidLogLevel(logLevel);

        AddPair(typeof(TException), logLevel);

        return this;
    }

    public LogLevelMap Build()
        => new(BuildFrozenDictionary());

    private static void ThrowIfInvalidLogLevel(LogLevel logLevel)
    {
        if (!Enum.IsDefined(logLevel))
        {
            throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, $"The value '{logLevel}' is not a valid {nameof(LogLevel)}.");
        }

        if (logLevel is LogLevel.None)
        {
            throw new ArgumentException($"{nameof(LogLevel)}.{nameof(LogLevel.None)} cannot be used for exception mapping.", nameof(logLevel));
        }
    }
}