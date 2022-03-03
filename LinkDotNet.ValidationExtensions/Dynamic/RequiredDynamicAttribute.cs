using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Validation attribute that can accept method to validate complicated requirement(s) and more readable code.
/// </summary>
public class RequiredDynamicAttribute : ValidationAttribute
{
    private readonly string methodName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredDynamicAttribute"/> class.
    /// </summary>
    /// <param name="methodName">Name of method that check requirement(s).
    /// The method must have one parameter of owning type and return Boolean value.
    /// If requirement(s) not met return true; else return false. </param>
    /// <exception cref="ArgumentNullException">methodName can not be null.</exception>
    public RequiredDynamicAttribute(string methodName)
    {
        this.methodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        return $"{name} is required because {ErrorMessageString}";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isRequired = IsRequired(validationContext, methodName);
        if (isRequired)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        else
        {
            return ValidationResult.Success;
        }
    }

    private static bool IsRequired(ValidationContext validationContext, string methodName)
    {
        ArgumentNullException.ThrowIfNull(validationContext);
        ArgumentNullException.ThrowIfNull(methodName);

        var owningType = validationContext.ObjectType;
        var methodInfo = owningType.GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (methodInfo == null)
        {
            methodInfo = owningType.GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        }

        if (methodInfo == null)
        {
            throw new NotSupportedException($"Can't find {methodName} method on searched type: {owningType.Name}");
        }

        var methodParameters = methodInfo.GetParameters();
        if (methodParameters.Length != 1 && methodParameters[0].ParameterType == owningType)
        {
            throw new NotSupportedException($"{methodName} on searched type: {owningType.Name} must have one parameter of {owningType.Name} type!");
        }

        if (methodInfo.ReturnType != typeof(bool))
        {
            throw new NotSupportedException($"{methodName} on searched type: {owningType.Name} must return boolean!");
        }

        return Convert.ToBoolean(methodInfo.Invoke(null, new object[] { validationContext.ObjectInstance }));
    }
}
