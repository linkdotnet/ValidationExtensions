using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class MaxLengthIfNotAttribute : MaxLengthAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;

    public MaxLengthIfNotAttribute(string propertyName, object? isValue, int length)
        : base(length)
    {
        this.isValue = isValue;
        this.propertyName = propertyName;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field '{name}' must be a string or array type with a maximum length of '{Length}' when '{propertyName}' is not '{isValue}'.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var shouldCheck = validationContext.IsRequired(propertyName, isValue, true);

        return !shouldCheck ? ValidationResult.Success : base.IsValid(value, validationContext);
    }
}