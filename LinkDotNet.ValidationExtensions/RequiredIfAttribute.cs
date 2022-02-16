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
        ArgumentNullException.ThrowIfNull(validationContext);
        var property = validationContext.ObjectType.GetProperty(propertyName);

        if (property == null)
        {
            throw new NotSupportedException($"Can't find {propertyName} on searched type: {validationContext.ObjectType.Name}");
        }

        var requiredIfTypeActualValue = property.GetValue(validationContext.ObjectInstance);

        if (requiredIfTypeActualValue == null && isValue != null)
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