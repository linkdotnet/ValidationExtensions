using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Used for specifying a range that accepts another property-name for 'Minimum' or 'Maximum'.
/// </summary>
public class DynamicRangeAttribute : ValidationAttribute
{
    private readonly Func<ValidationContext, object> getMinimum;
    private readonly Func<ValidationContext, object> getMaximum;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute"/> class.
    /// Allows for specifying range for arbitrary types. The minimum and maximum strings
    /// will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimum">The minimum allowable value.</param>
    /// <param name="maximumPropertyName">The property-name of maximum.</param>
    public DynamicRangeAttribute(Type type, object minimum, string maximumPropertyName)
        : base()
    {
        OperandType = type;
        getMinimum = (ValidationContext validationContext) => minimum;
        getMaximum = (ValidationContext validationContext) => GetActualValue(validationContext, OperandType, "Maximum", maximumPropertyName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute"/> class.
    /// Allows for specifying range for arbitrary types. The minimum and maximum strings
    /// will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimumPropertyName">The property-name of minimum.</param>
    /// <param name="maximum">The maximum allowable value.</param>
    public DynamicRangeAttribute(Type type, string minimumPropertyName, object maximum)
       : base()
    {
        OperandType = type;
        getMinimum = (ValidationContext validationContext) => GetActualValue(validationContext, OperandType, "Minimum", minimumPropertyName);
        getMaximum = (ValidationContext validationContext) => maximum;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute"/> class.
    /// Allows for specifying range for arbitrary types. The minimum and maximum strings
    /// will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimumPropertyName">The property-name of minimum.</param>
    /// <param name="maximumPropertyName">The property-name of maximum.</param>
    public DynamicRangeAttribute(Type type, string minimumPropertyName, string maximumPropertyName)
       : base()
    {
        OperandType = type;
        getMinimum = (ValidationContext validationContext) => GetActualValue(validationContext, OperandType, "Minimum", minimumPropertyName);
        getMaximum = (ValidationContext validationContext) => GetActualValue(validationContext, OperandType, "Maximum", maximumPropertyName);
    }

    /// <summary>
    /// Gets the type of the <see cref="Minimum" /> and <see cref="Maximum" /> values
    /// (e.g. Int32, Double, or some custom type).
    /// </summary>
    /// <value> The type of the <see cref="Minimum" /> and <see cref="Maximum" /> values. </value>
    public Type OperandType { get; }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var minimumAsText = getMinimum.Invoke(validationContext).ToString();
        var maximumAsText = getMaximum.Invoke(validationContext).ToString();

        // Create RangeAttribute instance for validate value in actual range
        var rangeAttribute = new RangeAttribute(OperandType, minimumAsText, maximumAsText);

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
    /// Get actual value of subject; subject is 'Minimum' or 'Maximum'.
    /// </summary>
    /// <param name="validationContext">
    /// A <see cref="ValidationContext" /> instance that provides
    /// context about the validation operation, such as the object and member being validated.
    /// </param>
    /// <param name="subjectType"> The type of subject. </param>
    /// <param name="subjectName"> The name of subject. </param>
    /// <param name="propertyName"> The propertyName or value of subject. </param>
    /// <returns> The actual value of subject. </returns>
    /// <exception cref="ArgumentNullException"> is thrown when propertyNameOrValue is null or nothing. </exception>
    /// <exception cref="InvalidOperationException"> is thrown when PropertyType of propertyName has not been the same as OperandType.</exception>
    private static object GetActualValue(ValidationContext validationContext, Type subjectType, string subjectName, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new InvalidOperationException($"The '{subjectName} PropertyName' cannot be null or empty.");
        }

        var propertyInfo = validationContext.ObjectType.GetProperty(propertyName);

        if (propertyInfo is null)
        {
            throw new InvalidOperationException($"The '{propertyName}' property not found (introduced for '{subjectName}' in range).");
        }

        var propertyType = propertyInfo.PropertyType;
        if (propertyType != subjectType)
        {
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // it is nullable so we extrcat underlying type
                propertyType = propertyType.GetGenericArguments()[0];
            }

            if (propertyType != subjectType)
            {
                throw new InvalidOperationException($"The '{propertyName}' type must be the same as the OperandType (introduced for '{subjectName}' in range).");
            }
        }

        var value = propertyInfo.GetValue(validationContext.ObjectInstance);

        if (value is null)
        {
            throw new InvalidOperationException($"The value of '{propertyName}' property cannot be null (introduced for '{subjectName}' in range).");
        }

        return value;
    }
}