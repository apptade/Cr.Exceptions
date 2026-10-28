namespace Cr.Exceptions;

public class ForbiddenException : CrException
{
    private const string DefaultMessage = "You do not have permission to perform this operation.";

    public ForbiddenException(Exception? innerException = null) : base(DefaultMessage, innerException) { }

    public ForbiddenException(ImmutableArray<CrError> errors, Exception? innerException = null) : base(errors, DefaultMessage, innerException) { }
}