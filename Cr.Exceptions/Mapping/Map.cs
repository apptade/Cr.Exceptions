using System.Diagnostics.CodeAnalysis;

namespace Cr.Exceptions;

public abstract class Map<TKey, TValue> where TKey : notnull
{
    protected Map(FrozenDictionary<TKey, TValue> frozenDictionary)
    {
        ArgumentNullException.ThrowIfNull(frozenDictionary);

        FrozenDictionary = frozenDictionary;
    }

    public FrozenDictionary<TKey, TValue> FrozenDictionary { get; }

    protected TValue GetValue(TKey key)
        => TryGetValue(key, out var value) ? value : throw CreateKeyNotFoundException(key);

    protected bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        => FrozenDictionary.TryGetValue(key, out value);

    protected KeyNotFoundException CreateKeyNotFoundException(TKey? key)
        => new($"Key '{key?.ToString() ?? "null"}' was not found in {GetType().Name}");
}