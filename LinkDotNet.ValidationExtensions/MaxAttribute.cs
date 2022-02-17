using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Used for specifying a minimum value
/// </summary>
public class MaxAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaxAttribute"/> class.
    /// </summary>
    /// <param name="maximum">The inclusive maximum value.</param>
    public MaxAttribute(int maximum)
        : base(int.MinValue, maximum)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxAttribute"/> class.
    /// </summary>
    /// <param name="maximum">The inclusive maximum value.</param>
    public MaxAttribute(double maximum)
        : base(double.MinValue, maximum)
    {
    }
}