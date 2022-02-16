using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class MinLengthIfNotAttribute : MinLengthAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;

    public MinLengthIfNotAttribute(string propertyName, object? isValue, int length)
        : base(length)
    {
        this.isValue = isValue;
        this.propertyName = propertyName;
    }

    public override string FormatErrorMessage(string name)
    {
        var errorMessage = $"The field '{name}' must be a string or array type with a minimum length of '{Length}' when '{propertyName}' is not '{isValue}'.";
        return ErrorMessage ?? errorMessage;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var shouldCheck = validationContext.IsRequired(propertyName, isValue, true);

        return !shouldCheck ? ValidationResult.Success : base.IsValid(value, validationContext);
    }
}