using System.Diagnostics.CodeAnalysis;

namespace Cr.Exceptions;

public class ExceptionFactory : Map<string, Func<CrException>>
{
    internal ExceptionFactory(FrozenDictionary<string, Func<CrException>> frozenDictionary) : base(frozenDictionary) { }

    public CrException Create(string code)
        => ExecuteFactory(GetValue(code));

    public bool TryCreate(string code, [MaybeNullWhen(false)] out CrException exception)
        => (exception = TryGetValue(code, out var factory) ? ExecuteFactory(factory) : null) != null;

    private static CrException ExecuteFactory(Func<CrException> factory)
    {
        return factory() ?? throw new InvalidOperationException($"The exception factory '{factory.Method}' returned null.");
    }
}