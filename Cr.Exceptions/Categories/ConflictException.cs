namespace Cr.Exceptions;

public class ConflictException : CrException
{
    private const string DefaultMessage = "The requested operation could not be completed due to a conflict.";

    public ConflictException(Exception? innerException = null) : base(DefaultMessage, innerException) { }

    public ConflictException(ImmutableArray<CrError> errors, Exception? innerException = null) : base(errors, DefaultMessage, innerException) { }
}