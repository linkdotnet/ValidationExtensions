using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Used for specifying a range constraint when the condition is (not) met.
/// </summary>
public sealed class RangeIfAttribute : RangeAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;
    private readonly bool inverse;

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the depending property.</param>
    /// <param name="isValue">Required value. If <see cref="propertyName"/> is <see cref="isValue"/> then the property is not required.</param>
    /// <param name="minimum">The minimum value, inclusive.</param>
    /// <param name="maximum">The maximum value, inclusive.</param>
    /// <param name="inverse">If set to true, the value is not required when <see cref="propertyName"/> is
    /// not <see cref="isValue"/>.</param>
    public RangeIfAttribute(string propertyName, object? isValue, double minimum, double maximum, bool inverse = false)
        : base(minimum, maximum)
    {
        this.inverse = inverse;
        this.propertyName = propertyName;
        this.isValue = isValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the depending property.</param>
    /// <param name="isValue">Required value. If <see cref="propertyName"/> is <see cref="isValue"/> then the property is not required.</param>
    /// <param name="minimum">The minimum value, inclusive.</param>
    /// <param name="maximum">The maximum value, inclusive.</param>
    /// <param name="inverse">If set to true, the value is not required when <see cref="propertyName"/> is
    /// not <see cref="isValue"/>.</param>
    public RangeIfAttribute(string propertyName, object? isValue, int minimum, int maximum, bool inverse = false)
        : base(minimum, maximum)
    {
        this.inverse = inverse;
        this.propertyName = propertyName;
        this.isValue = isValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the depending property.</param>
    /// <param name="isValue">Required value. If <see cref="propertyName"/> is <see cref="isValue"/> then the property is not required.</param>
    /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
    /// <param name="minimum">The minimum value, inclusive.</param>
    /// <param name="maximum">The maximum value, inclusive.</param>
    /// <param name="inverse">If set to true, the value is not required when <see cref="propertyName"/> is
    /// not <see cref="isValue"/>.</param>
    public RangeIfAttribute(string propertyName, object? isValue, Type type, string minimum, string maximum, bool inverse = false)
        : base(type, minimum, maximum)
    {
        this.inverse = inverse;
        this.propertyName = propertyName;
        this.isValue = isValue;
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var shouldCheck = validationContext.IsRequired(propertyName, isValue, inverse);

        return !shouldCheck ? ValidationResult.Success : base.IsValid(value, validationContext);
    }
}