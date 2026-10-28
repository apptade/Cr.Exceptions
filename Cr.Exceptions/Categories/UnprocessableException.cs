namespace Cr.Exceptions;

public class UnprocessableException : CrException
{
    private const string DefaultMessage = "The request could not be processed.";

    public UnprocessableException(Exception? innerException = null) : base(DefaultMessage, innerException) { }

    public UnprocessableException(ImmutableArray<CrError> errors, Exception? innerException = null) : base(errors, DefaultMessage, innerException) { }
}