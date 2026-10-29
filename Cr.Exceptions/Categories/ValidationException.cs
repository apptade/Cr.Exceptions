namespace Cr.Exceptions;

public class ValidationException : CrException
{
    private const string DefaultMessage = "The provided data is invalid.";

    public ValidationException(Exception? innerException = null) : base(DefaultMessage, innerException) { }

    public ValidationException(ImmutableArray<CrError> errors, Exception? innerException = null) : base(errors, DefaultMessage, innerException) { }
}