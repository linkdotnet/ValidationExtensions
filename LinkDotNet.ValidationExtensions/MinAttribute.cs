using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Used for specifying a minimum value
/// </summary>
public class MinAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinAttribute"/> class.
    /// </summary>
    /// <param name="minimum">The inclusive minimum value.</param>
    public MinAttribute(int minimum)
        : base(minimum, int.MaxValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinAttribute"/> class.
    /// </summary>
    /// <param name="minimum">The inclusive minimum value.</param>
    public MinAttribute(double minimum)
        : base(minimum, double.MaxValue)
    {
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        return $"The field '{name}' must be at least {Minimum}.";
    }
}