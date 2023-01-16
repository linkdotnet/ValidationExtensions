using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Specifies the minimum length of collection/string data allowed in a property when the condition is (not) met.
/// </summary>
public sealed class MinLengthIfAttribute : MinLengthAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;
    private readonly bool inverse;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinLengthIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the depending property.</param>
    /// <param name="isValue">Required value. If <see cref="propertyName"/> is <see cref="isValue"/> then the property is not required.</param>
    /// <param name="length">The minimum allowable length of collection/string data. Value must be greater than or equal to zero.</param>
    /// <param name="inverse">If set to true, the value is not required when <see cref="propertyName"/> is
    /// not <see cref="isValue"/>.</param>
    public MinLengthIfAttribute(string propertyName, object? isValue, int length, bool inverse = false)
        : base(length)
    {
        this.isValue = isValue;
        this.inverse = inverse;
        this.propertyName = propertyName;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        var inverseString = !inverse ? string.Empty : "not ";
        return $"The field '{name}' must be a string or array type with a minimum length of '{Length}' when '{propertyName}' is {inverseString}'{isValue}'.";
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var shouldCheck = validationContext.IsRequired(propertyName, isValue, inverse);

        return !shouldCheck ? ValidationResult.Success : base.IsValid(value, validationContext);
    }
}