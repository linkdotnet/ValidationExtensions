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
        ArgumentNullException.ThrowIfNull(validationContext);
        var property = validationContext.ObjectType.GetProperty(propertyName);

        if (property == null)
        {
            throw new NotSupportedException($"Can't find {propertyName} on searched type: {validationContext.ObjectType.Name}");
        }

        var requiredIfNotTypeActualValue = property.GetValue(validationContext.ObjectInstance);

        if (requiredIfNotTypeActualValue == null && isNotValue != null)
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