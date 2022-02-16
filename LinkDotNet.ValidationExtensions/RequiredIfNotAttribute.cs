using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class RequiredIfNotAttribute : ValidationAttribute
{
    private readonly string propertyName;
    private readonly object? isNotValue;

    public RequiredIfNotAttribute(string propertyName, object? isNotValue)
    {
        this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        this.isNotValue = isNotValue;
    }

    public override string FormatErrorMessage(string name)
    {
        var errorMessage = $"Property '{name}' is required when '{propertyName}' is not '{isNotValue}'";
        return ErrorMessage ?? errorMessage;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isRequired = validationContext.IsRequired(
            propertyName,
            isNotValue,
            invert: true);
        if (!isRequired)
        {
            return ValidationResult.Success;
        }

        return value == null
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
            : ValidationResult.Success;
    }
}