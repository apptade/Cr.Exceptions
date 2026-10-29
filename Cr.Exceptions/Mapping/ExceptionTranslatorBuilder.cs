namespace Cr.Exceptions;

public class ExceptionTranslatorBuilder : MapBuilder<Type, Func<Exception, CrException>>
{
    public ExceptionTranslatorBuilder Map<TException>(Func<Exception, CrException> translator) where TException : Exception
    {
        AddPair(typeof(TException), translator);

        return this;
    }

    public ExceptionTranslator Build()
        => new(BuildFrozenDictionary());
}