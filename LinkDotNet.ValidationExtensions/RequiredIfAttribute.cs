using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;

    public RequiredIfAttribute(string propertyName, object? isValue)
    {
        this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        this.isValue = isValue;
    }

    public override string FormatErrorMessage(string name)
    {
        var errorMessage = $"Property {name} is required when {propertyName} is {isValue}";
        return ErrorMessage ?? errorMessage;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isRequired = validationContext.IsRequired(propertyName, isValue, false, out var requiredIfTypeActualValue);
        if (!isRequired)
        {
            return ValidationResult.Success;
        }

        if (requiredIfTypeActualValue == null || requiredIfTypeActualValue.Equals(isValue))
        {
            return value == null
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
                : ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}