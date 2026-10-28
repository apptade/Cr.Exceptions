namespace Cr.Exceptions;

public class ExceptionFactoryBuilder : MapBuilder<string, Func<CrException>>
{
    public ExceptionFactoryBuilder Map(string code, Func<CrException> factory)
    {
        ArgumentException.ThrowIfNullOrEmpty(code);

        AddPair(code, factory);

        return this;
    }

    public ExceptionFactory Build()
        => new(BuildFrozenDictionary(StringComparer.Ordinal));
}