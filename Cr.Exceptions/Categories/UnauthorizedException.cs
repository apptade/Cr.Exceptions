namespace Cr.Exceptions;

public class UnauthorizedException : CrException
{
    private const string DefaultMessage = "Authentication is required to access this resource.";

    public UnauthorizedException(Exception? innerException = null) : base(DefaultMessage, innerException) { }

    public UnauthorizedException(ImmutableArray<CrError> errors, Exception? innerException = null) : base(errors, DefaultMessage, innerException) { }
}