using System.Diagnostics.CodeAnalysis;

namespace Cr.Exceptions.AspNetCore;

public class StatusCodeMap : TypeMap<int>
{
    internal StatusCodeMap(FrozenDictionary<Type, int> frozenDictionary) : base(frozenDictionary) { }

    public bool TryFind(CrException exception, [MaybeNullWhen(false)] out int statusCode)
        => TryGetByHierarchy(exception.GetType(), out statusCode);
}