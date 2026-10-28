using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Cr.Exceptions.AspNetCore;

public class LogLevelMap : TypeMap<LogLevel>
{
    internal LogLevelMap(FrozenDictionary<Type, LogLevel> frozenDictionary) : base(frozenDictionary) { }

    public bool TryFind(CrException exception, [MaybeNullWhen(false)] out LogLevel logLevel)
        => TryGetByHierarchy(exception.GetType(), out logLevel);
}