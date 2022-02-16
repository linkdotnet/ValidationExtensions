using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public class MaxLengthIfAttribute : MaxLengthAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;
    private readonly bool inverse;

    public MaxLengthIfAttribute(string propertyName, object? isValue, int length, bool inverse = false)
        : base(length)
    {
        this.isValue = isValue;
        this.inverse = inverse;
        this.propertyName = propertyName;
    }

    public override string FormatErrorMessage(string name)
    {
        var inverseString = !inverse ? string.Empty : "not ";
        return $"The field '{name}' must be a string or array type with a maximum length of '{Length}' when '{propertyName}' is {inverseString}'{isValue}'.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var shouldCheck = validationContext.IsRequired(propertyName, isValue, inverse);

        return !shouldCheck ? ValidationResult.Success : base.IsValid(value, validationContext);
    }
}