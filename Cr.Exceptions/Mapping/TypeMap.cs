using System.Diagnostics.CodeAnalysis;

namespace Cr.Exceptions;

public abstract class TypeMap<TValue> : Map<Type, TValue>
{
    protected TypeMap(FrozenDictionary<Type, TValue> frozenDictionary) : base(frozenDictionary) { }

    protected TValue GetByHierarchy(Type? type)
        => TryGetByHierarchy(type, out var value) ? value : throw CreateKeyNotFoundException(type);

    protected bool TryGetByHierarchy(Type? type, [MaybeNullWhen(false)] out TValue value)
    {
        for (; type is not null; type = type.BaseType)
        {
            if (TryGetValue(type, out value))
            {
                return true;
            }
        }

        value = default;
        return false;
    }
}