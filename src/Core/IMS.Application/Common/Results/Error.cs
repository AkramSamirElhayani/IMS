namespace IMS.Application.Common.Results;

public record Error(string Code, string Message)
{
    public static Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string message) => new("NotFound", message);
    public static Error Validation(string message) => new("Validation", message);
    public static Error Conflict(string message) => new("Conflict", message);
    public static Error Unauthorized(string message) => new("Unauthorized", message);
}
