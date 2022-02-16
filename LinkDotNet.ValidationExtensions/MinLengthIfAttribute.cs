using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class MinLengthIfAttribute : MinLengthAttribute
{
    private string propertyName;
    private object? isValue;

    public MinLengthIfAttribute(string propertyName, object? isValue, int length)
        : base(length)
    {
        this.isValue = isValue;
        this.propertyName = propertyName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var shouldCheck = validationContext.IsRequired(propertyName, value, false, out var requiredActualValue);

        if (!shouldCheck)
        {
            return ValidationResult.Success;
        }

        if (requiredActualValue == null || requiredActualValue.Equals(isValue))
        {
            return value == null
                ? base.IsValid(value, validationContext)
                : ValidationResult.Success;
        }

        return base.IsValid(value, validationContext);
    }
}