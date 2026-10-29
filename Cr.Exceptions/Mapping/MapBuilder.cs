namespace Cr.Exceptions;

public abstract class MapBuilder<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _map = [];

    protected void AddPair(TKey key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        if (!_map.TryAdd(key, value))
        {
            throw new ArgumentException($"The key '{key}' ({key.GetType().Name}) has already been added to {GetType().Name}.", nameof(key));
        }
    }

    protected FrozenDictionary<TKey, TValue> BuildFrozenDictionary(IEqualityComparer<TKey>? comparer = null)
        => _map.ToFrozenDictionary(comparer);
}