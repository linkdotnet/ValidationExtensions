using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Used for specifying a range that accepts another property-name for 'Minimum' or 'Maximum'.
/// </summary>
/// <typeparam name="T">
/// The operand type of range.
/// </typeparam>
public sealed class DynamicRangeAttribute<T> : ValidationAttribute
    where T : IComparable<T>
{
    private readonly Func<ValidationContext, T> getMinimum;
    private readonly Func<ValidationContext, T> getMaximum;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute{T}"/> class.
    /// Allows for specifying range for arbitrary types. The minimum and maximum strings
    /// will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimumPropertyName">The property-name of minimum.</param>
    /// <param name="maximumPropertyName">The property-name of maximum.</param>
    public DynamicRangeAttribute(string minimumPropertyName, string maximumPropertyName)
       : base()
    {
        getMinimum = validationContext => GetActualValue(validationContext, "Minimum", minimumPropertyName);
        getMaximum = validationContext => GetActualValue(validationContext, "Maximum", maximumPropertyName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute{T}"/> class.
    /// Allows for specifying range for arbitrary types. The minimum and maximum strings
    /// will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimumPropertyName">The property-name of minimum.</param>
    /// <param name="maximum">The maximum allowable value.</param>
    public DynamicRangeAttribute(string minimumPropertyName, T maximum)
       : base()
    {
        getMinimum = validationContext => GetActualValue(validationContext, "Minimum", minimumPropertyName);
        getMaximum = _ => maximum;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute{T}"/> class.
    /// Allows for specifying range for arbitrary types. The minimum and maximum strings
    /// will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimum">The minimum allowable value.</param>
    /// <param name="maximumPropertyName">The property-name of maximum.</param>
    public DynamicRangeAttribute(T minimum, string maximumPropertyName)
    {
        getMinimum = _ => minimum;
        getMaximum = validationContext => GetActualValue(validationContext, "Maximum", maximumPropertyName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRangeAttribute{T}"/> class.
    /// Allows for specifying range for arbitrary types. The minimum and maximum strings
    /// will be converted to the target type.
    /// </summary>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimum">The minimum allowable value.</param>
    /// <param name="maximum">The maximum allowable value.</param>
    public DynamicRangeAttribute(T minimum, T maximum)
        : base()
    {
        getMinimum = _ => minimum;
        getMaximum = _ => maximum;
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var operandType = typeof(T);
        var minimumAsText = getMinimum.Invoke(validationContext).ToString()!;
        var maximumAsText = getMaximum.Invoke(validationContext).ToString()!;

        // Create RangeAttribute instance for validate value in actual range
        var rangeAttribute = new RangeAttribute(operandType, minimumAsText, maximumAsText);

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
    /// <param name="subjectName"> The name of subject. </param>
    /// <param name="propertyName"> The propertyName or value of subject. </param>
    /// <returns> The actual value of subject. </returns>
    /// <exception cref="ArgumentNullException"> is thrown when propertyNameOrValue is null or nothing. </exception>
    /// <exception cref="InvalidOperationException"> is thrown when PropertyType of propertyName has not been the same as OperandType.</exception>
    private static T GetActualValue(ValidationContext validationContext, string subjectName, string propertyName)
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

        var subjectType = typeof(T);
        var propertyType = propertyInfo.PropertyType;
        if (propertyType != subjectType)
        {
            var isNullableType = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (isNullableType)
            {
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

        return (T)value;
    }
}