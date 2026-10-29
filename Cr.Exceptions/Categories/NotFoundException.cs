namespace Cr.Exceptions;

public class NotFoundException : CrException
{
    private const string DefaultMessage = "The requested resource was not found.";

    public NotFoundException(Exception? innerException = null) : base(DefaultMessage, innerException) { }

    public NotFoundException(ImmutableArray<CrError> errors, Exception? innerException = null) : base(errors, DefaultMessage, innerException) { }
}