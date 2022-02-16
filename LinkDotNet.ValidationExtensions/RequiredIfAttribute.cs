using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Validation attribute to indicate that a property field or parameter is required when the condition is (not) met.
/// </summary>
public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;
    private readonly bool inverse;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the depending property.</param>
    /// <param name="isValue">Required value. If <see cref="propertyName"/> is <see cref="isValue"/> then the property is not required.</param>
    /// <param name="inverse">If set to true, the value is not required when <see cref="propertyName"/> is
    /// not <see cref="isValue"/>.</param>
    public RequiredIfAttribute(string propertyName, object? isValue, bool inverse = false)
    {
        this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        this.isValue = isValue;
        this.inverse = inverse;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        var inverseString = !inverse ? string.Empty : "not ";
        var errorMessage = $"Property '{name}' is required when '{propertyName}' is {inverseString}'{isValue}'";
        return ErrorMessage ?? errorMessage;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isRequired = validationContext.IsRequired(propertyName, isValue, inverse);
        if (!isRequired)
        {
            return ValidationResult.Success;
        }

        var validationResult = value == null
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
            : ValidationResult.Success;
        return validationResult;
    }
}