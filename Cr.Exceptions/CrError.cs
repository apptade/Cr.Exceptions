namespace Cr.Exceptions;

public record class CrError
{
    public CrError(string code, string? message = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(code);

        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string? Message { get; }
}