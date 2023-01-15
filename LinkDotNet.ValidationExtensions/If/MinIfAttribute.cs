﻿using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Used for specifying a minimum value when the condition is (not) met.
/// </summary>
public sealed class MinIfAttribute : MinAttribute
{
    private readonly string propertyName;
    private readonly object? isValue;
    private readonly bool inverse;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the depending property.</param>
    /// <param name="isValue">Required value. If <see cref="propertyName"/> is <see cref="isValue"/> then the property is not required.</param>
    /// <param name="minimum">The inclusive minimum value.</param>
    /// <param name="inverse">If set to true, the value is not required when <see cref="propertyName"/> is
    /// not <see cref="isValue"/>.</param>
    public MinIfAttribute(string propertyName, object isValue, int minimum, bool inverse = false)
        : base(minimum)
    {
        this.isValue = isValue;
        this.inverse = inverse;
        this.propertyName = propertyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the depending property.</param>
    /// <param name="isValue">Required value. If <see cref="propertyName"/> is <see cref="isValue"/> then the property is not required.</param>
    /// <param name="minimum">The inclusive minimum value.</param>
    /// <param name="inverse">If set to true, the value is not required when <see cref="propertyName"/> is
    /// not <see cref="isValue"/>.</param>
    public MinIfAttribute(string propertyName, object isValue, double minimum, bool inverse = false)
        : base(minimum)
    {
        this.isValue = isValue;
        this.inverse = inverse;
        this.propertyName = propertyName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var shouldCheck = validationContext.IsRequired(propertyName, isValue, inverse);

        return !shouldCheck ? ValidationResult.Success : base.IsValid(value, validationContext);
    }
}