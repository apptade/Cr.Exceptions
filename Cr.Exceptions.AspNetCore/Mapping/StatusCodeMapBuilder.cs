using System.Net;

namespace Cr.Exceptions.AspNetCore;

public class StatusCodeMapBuilder : MapBuilder<Type, int>
{
    public StatusCodeMapBuilder Map<TException>(int statusCode) where TException : CrException
    {
        ThrowIfInvalidStatusCode(statusCode);

        AddPair(typeof(TException), statusCode);

        return this;
    }

    public StatusCodeMap Build()
        => new(BuildFrozenDictionary());

    private static void ThrowIfInvalidStatusCode(int statusCode)
    {
        if (!Enum.IsDefined((HttpStatusCode)statusCode))
        {
            throw new ArgumentOutOfRangeException(nameof(statusCode), $"'{statusCode}' is not a standard HTTP status code.");
        }
    }
}