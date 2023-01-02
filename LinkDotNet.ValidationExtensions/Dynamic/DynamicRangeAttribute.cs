using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Used for specifying a range that accepts another property-name for 'Minimum' or 'Maximum'.
/// </summary>
public class DynamicRangeAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute"/> class.
    ///     Allows for specifying range for arbitrary types. The minimum and maximum strings
    ///     will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimum">The minimum allowable value or property-name.</param>
    /// <param name="maximum">The maximum allowable value or property-name.</param>
    public DynamicRangeAttribute(Type type, string minimum, string maximum)
        : base()
    {
        OperandType = type;
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// Gets the type of the <see cref="Minimum" /> and <see cref="Maximum" /> values
    /// (e.g. Int32, Double, or some custom type).
    /// </summary>
    /// <value> The type of the <see cref="Minimum" /> and <see cref="Maximum" /> values. </value>
    public Type OperandType { get; }

    /// <summary>
    /// Gets the minimum value for the range or another property-name.
    /// </summary>
    /// <value> The minimum value for the range or another property-name. </value>
    public string Minimum { get; private set; }

    /// <summary>
    /// Gets the maximum value for the range or another property-name.
    /// </summary>
    /// <value> The maximum value for the range or another property-name. </value>
    public string Maximum { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether string values for <see cref="Minimum"/> and <see cref="Maximum"/>
    /// are parsed in the invariant culture rather than the current culture in effect at the time of the validation
    /// (<see cref="RangeAttribute.ParseLimitsInInvariantCulture"/>).
    /// </summary>
    /// <value> A value for <see cref="RangeAttribute.ParseLimitsInInvariantCulture"/>. </value>
    public bool ParseLimitsInInvariantCulture { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether any conversions necessary from the value being validated to <see cref="OperandType"/>
    /// as set by the <c>type</c> parameter of the <see cref="DynamicRangeAttribute(Type, string, string)"/> constructor are carried
    /// out in the invariant culture rather than the current culture in effect at the time of the validation.
    /// (<see cref="RangeAttribute.ConvertValueInInvariantCulture"/>).
    /// </summary>
    /// <value> A value for <see cref="RangeAttribute.ConvertValueInInvariantCulture"/>. </value>
    public bool ConvertValueInInvariantCulture { get; set; }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Get actual value of minimum and maximum
        Minimum = GetActualValue(validationContext, OperandType, nameof(Minimum), Minimum);
        Maximum = GetActualValue(validationContext, OperandType, nameof(Maximum), Maximum);

        // Create RangeAttribute instance for validate value in actual range
        var rangeAttribute = new RangeAttribute(OperandType, Minimum, Maximum)
        {
            ParseLimitsInInvariantCulture = ParseLimitsInInvariantCulture,
            ConvertValueInInvariantCulture = ConvertValueInInvariantCulture,
        };

        if (ErrorMessage is not null)
        {
            rangeAttribute.ErrorMessage = ErrorMessage;
        }

        if (ErrorMessageResourceName is not null)
        {
            rangeAttribute.ErrorMessageResourceName = ErrorMessageResourceName;
        }

        if (ErrorMessageResourceType is not null)
        {
            rangeAttribute.ErrorMessageResourceType = ErrorMessageResourceType;
        }

        return rangeAttribute.GetValidationResult(value, validationContext);
    }

    /// <summary>
    /// Get actual value of subject.
    /// </summary>
    /// <param name="validationContext">
    /// A <see cref="ValidationContext" /> instance that provides
    /// context about the validation operation, such as the object and member being validated.
    /// </param>
    /// <param name="subjectType"> The type of subject. </param>
    /// <param name="subjectName"> The name of subject. </param>
    /// <param name="propertyNameOrValue"> The propertyName or value of subject. </param>
    /// <returns> The actual value of subject. </returns>
    /// <exception cref="ArgumentNullException"> is thrown when propertyNameOrValue is null or nothing. </exception>
    /// <exception cref="InvalidOperationException"> is thrown when PropertyType of propertyName has not been the same as OperandType.</exception>
    private static string GetActualValue(ValidationContext validationContext, Type subjectType, string subjectName, string propertyNameOrValue)
    {
        if (string.IsNullOrWhiteSpace(propertyNameOrValue))
        {
            throw new ArgumentNullException(subjectName);
        }

        var propertyInfo = validationContext.ObjectType.GetProperty(propertyNameOrValue);

        if (propertyInfo is null)
        {
            return propertyNameOrValue;
        }

        if (propertyInfo.PropertyType != subjectType)
        {
            throw new InvalidOperationException($"The {subjectName} PropertyType must be the same as the OperandType.");
        }

        var value = propertyInfo.GetValue(validationContext.ObjectInstance, null)?.ToString();

        if (value is null)
        {
            throw new InvalidOperationException($"The value of '{propertyNameOrValue}' property cannot be null (introduced for '{subjectName}' in range).");
        }

        return value;
    }
}