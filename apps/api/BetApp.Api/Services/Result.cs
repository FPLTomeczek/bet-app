namespace BetApp.Api.Services;

/// <summary>
/// A single domain-level validation failure: which field is wrong and why.
/// Deliberately framed in domain terms (field name + message), NOT in HTTP terms —
/// the service layer must not know about ModelState or ValidationProblem. The
/// controller translates these into whatever the transport (HTTP) expects.
/// </summary>
public record ValidationError(string Field, string Message);

/// <summary>
/// Outcome of a service operation: either success carrying a value, or failure
/// carrying the validation errors that explain why. Lets a service report "this
/// input was invalid" without reaching for exceptions or HTTP-specific types.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public IReadOnlyList<ValidationError> Errors { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<ValidationError> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) =>
        new(true, value, Array.Empty<ValidationError>());

    public static Result<T> Invalid(IReadOnlyList<ValidationError> errors) =>
        new(false, default, errors);
}
