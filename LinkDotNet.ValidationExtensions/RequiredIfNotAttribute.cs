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
        var errorMessage = $"Property {name} is required when {propertyName} is not {isNotValue}";
        return ErrorMessage ?? errorMessage;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isRequired = validationContext.IsRequired(propertyName, isNotValue, false, out var requiredIfNotTypeActualValue);
        if (!isRequired)
        {
            return ValidationResult.Success;
        }

        if (requiredIfNotTypeActualValue == null || !requiredIfNotTypeActualValue.Equals(isNotValue))
        {
            return value == null
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
                : ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}