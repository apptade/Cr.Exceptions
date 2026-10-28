namespace Cr.Exceptions;

public class InternalException : CrException
{
    private const string DefaultMessage = "An unexpected internal error occurred.";

    public InternalException(Exception? innerException = null) : base(DefaultMessage, innerException) { }

    public InternalException(ImmutableArray<CrError> errors, Exception? innerException = null) : base(errors, DefaultMessage, innerException) { }
}