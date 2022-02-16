using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;
    private readonly bool inverse;

    public RequiredIfAttribute(string propertyName, object? isValue, bool inverse = false)
    {
        this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        this.isValue = isValue;
        this.inverse = inverse;
    }

    public override string FormatErrorMessage(string name)
    {
        var inverseString = !inverse ? string.Empty : "not ";
        var errorMessage = $"Property '{name}' is required when '{propertyName}' is {inverseString}'{isValue}'";
        return ErrorMessage ?? errorMessage;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isRequired = validationContext.IsRequired(propertyName, isValue, inverse);
        if (!isRequired)
        {
            return ValidationResult.Success;
        }

        var validationResult = value == null
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
            : ValidationResult.Success;
        return validationResult;
    }
}