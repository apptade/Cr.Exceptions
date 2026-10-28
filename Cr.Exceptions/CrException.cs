namespace Cr.Exceptions;

public class CrException : Exception
{
    public CrException(string? message = null, Exception? innerException = null) : base(message, innerException)
    {
        Errors = [];
    }

    public CrException(ImmutableArray<CrError> errors, string? message = null, Exception? innerException = null) : base(message, innerException)
    {
        errors.ThrowIfDefault();
        errors.ThrowIfContainsNull();

        Errors = errors;
    }

    public ImmutableArray<CrError> Errors { get; }
}