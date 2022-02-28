using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Validation attribute that can accept static method to validate complicated requirement(s) and more readable code.
/// </summary>
public class RequiredByStaticMethodAttribute : ValidationAttribute
{
    private readonly string staticMethodName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredByStaticMethodAttribute"/> class.
    /// </summary>
    /// <param name="staticMethodName">Name of static method that check requirement(s).
    /// The static method must have one parameter of owning type and return Boolean value.
    /// If requirement(s) not met return true; else return false. </param>
    /// <exception cref="ArgumentNullException">staticMethodName can not be null.</exception>
    public RequiredByStaticMethodAttribute(string staticMethodName)
    {
        this.staticMethodName = staticMethodName ?? throw new ArgumentNullException(nameof(staticMethodName));
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        return $"{name} is required because {ErrorMessageString}";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isRequired = IsRequired(validationContext, staticMethodName);
        if (isRequired)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        else
        {
            return ValidationResult.Success;
        }
    }

    private static bool IsRequired(ValidationContext validationContext, string staticMethodName)
    {
        ArgumentNullException.ThrowIfNull(validationContext);
        ArgumentNullException.ThrowIfNull(staticMethodName);

        var owningType = validationContext.ObjectType;
        var staticMethodInfo = owningType.GetMethod(staticMethodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (staticMethodInfo == null)
        {
            staticMethodInfo = owningType.GetMethod(staticMethodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        }

        if (staticMethodInfo == null)
        {
            throw new NotSupportedException($"Can't find {staticMethodName} static method on searched type: {owningType.Name}");
        }

        var staticMethodParameters = staticMethodInfo.GetParameters();
        if (staticMethodParameters.Length != 1 && staticMethodParameters[0].ParameterType == owningType)
        {
            throw new NotSupportedException($"{staticMethodName} on searched type: {owningType.Name} must have one parameter of {owningType.Name} type!");
        }

        if (staticMethodInfo.ReturnType != typeof(bool))
        {
            throw new NotSupportedException($"{staticMethodName} on searched type: {owningType.Name} must return boolean!");
        }

        return Convert.ToBoolean(staticMethodInfo.Invoke(null, new object[] { validationContext.ObjectInstance }));
    }
}
