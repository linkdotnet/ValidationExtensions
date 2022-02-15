using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class RequiredIfNotAttribute : ValidationAttribute
{
    private readonly string _propertyName;
    private readonly object? _isNotValue;

    public RequiredIfNotAttribute(string propertyName, object? isNotValue)
    {
        _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        _isNotValue = isNotValue;
    }

public override string FormatErrorMessage(string name)
{
    var errorMessage = $"Property {name} is required when {_propertyName} is not {_isNotValue}";
    return ErrorMessage ?? errorMessage;
}

protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
{
    ArgumentNullException.ThrowIfNull(validationContext);
    var property = validationContext.ObjectType.GetProperty(_propertyName);

    if (property == null)
    {
        throw new NotSupportedException($"Can't find {_propertyName} on searched type: {validationContext.ObjectType.Name}");
    }

    var requiredIfNotTypeActualValue = property.GetValue(validationContext.ObjectInstance);

    if (requiredIfNotTypeActualValue == null && _isNotValue != null)
    {
        return ValidationResult.Success;
    }

    if (requiredIfNotTypeActualValue == null)
    {
        return value == null
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
            : ValidationResult.Success;
    }

    return ValidationResult.Success;
    }
}