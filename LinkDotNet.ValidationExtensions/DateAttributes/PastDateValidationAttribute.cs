using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public sealed class PastDateValidationAttribute : ValidationAttribute
{
    public bool UseUtc { get; set; } = true;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var now = UseUtc ? DateTime.UtcNow : DateTime.Now;

        return (DateTime)value >= now
            ? new ValidationResult("The scheduled publish date must be in the future.")
            : ValidationResult.Success;
    }
}