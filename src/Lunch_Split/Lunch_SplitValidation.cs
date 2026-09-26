namespace Lunch_Split.Validation;
public record ValidationResult(bool IsValid, string ErrorMessage)
{
    public static ValidationResult Ok() => new(true, string.Empty);
 
    public static ValidationResult Fail(string message) => new(false, message);
}
 