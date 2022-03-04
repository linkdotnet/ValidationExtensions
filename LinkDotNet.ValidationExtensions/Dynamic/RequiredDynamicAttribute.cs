using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Validation attribute that can accept method to validate complicated requirement(s) and more readable code.
/// </summary>
public class RequiredDynamicAttribute : ValidationAttribute
{
    private static readonly List<System.Reflection.BindingFlags> BindingFlagsForSearch = new()
    {
        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic,
        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public,
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
    };

    private readonly string methodName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredDynamicAttribute"/> class.
    /// </summary>
    /// <param name="methodName">Name of method that check requirement(s).
    /// The method must have one parameter of owning type and return Boolean value.
    /// If requirement(s) not met return true; else return false. </param>
    /// <param name="errorMessage">The error message to associate with a validation control.
    /// Final error message is: 'Parameter name' is required because 'errorMessage'. </param>
    /// <exception cref="ArgumentNullException">methodName can not be null.</exception>
    /// <exception cref="ArgumentNullException">errorMessage can not be null.</exception>
    public RequiredDynamicAttribute(string methodName, string errorMessage)
        : base(errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentNullException(nameof(errorMessage));
        }

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
        var methodInfo = GetSuitableMethod(owningType, methodName);
        if (methodInfo == null)
        {
            throw new NotSupportedException($"Can't find {methodName} suitable method on searched type: {owningType.Name}");
        }

        return Convert.ToBoolean(methodInfo.Invoke(null, new object[] { validationContext.ObjectInstance }));
    }

    private static System.Reflection.MethodInfo? GetSuitableMethod(Type owningType, string methodName)
    {
        ArgumentNullException.ThrowIfNull(owningType);
        ArgumentNullException.ThrowIfNull(methodName);

        foreach (var bindingFlagForSearch in BindingFlagsForSearch)
        {
            var methodInfo = owningType.GetMethod(methodName, bindingFlagForSearch);
            if (methodInfo == null)
            {
                continue;
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1)
            {
                continue;
            }

            if (parameters[0].ParameterType != owningType)
            {
                continue;
            }

            if (methodInfo.ReturnType != typeof(bool))
            {
                continue;
            }

            return methodInfo;
        }

        return null;
    }
}
