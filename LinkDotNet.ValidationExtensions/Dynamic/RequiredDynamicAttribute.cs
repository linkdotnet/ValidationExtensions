﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LinkDotNet.ValidationExtensions;

/// <summary>
/// Validation attribute that can accept method to validate complicated requirement(s) and more readable code.
/// </summary>
public sealed class RequiredDynamicAttribute : ValidationAttribute
{
    private static readonly List<BindingFlags> BindingFlagsForSearch = new()
    {
        BindingFlags.Static | BindingFlags.Public,
        BindingFlags.Static | BindingFlags.NonPublic,
        BindingFlags.Instance | BindingFlags.Public,
        BindingFlags.Instance | BindingFlags.NonPublic,
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
        return IsRequired(validationContext, methodName)
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
            : ValidationResult.Success;
    }

    private static bool IsRequired(ValidationContext validationContext, string methodName)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(validationContext, nameof(validationContext));
        ArgumentNullExceptionHelper.ThrowIfNullOrEmpty(methodName, nameof(methodName));

        var owningType = validationContext.ObjectType;
        var methodInfo = GetSuitableMethod(owningType, methodName);
        if (methodInfo == null)
        {
            throw new NotSupportedException($"Can't find {methodName} suitable method on searched type: {owningType.Name}");
        }

        return Convert.ToBoolean(methodInfo.Invoke(validationContext.ObjectInstance, new[] { validationContext.ObjectInstance }));
    }

    private static MethodInfo? GetSuitableMethod(Type owningType, string methodName)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(owningType, nameof(owningType));
        ArgumentNullExceptionHelper.ThrowIfNullOrEmpty(methodName, nameof(methodName));

        return BindingFlagsForSearch
            .Select(bindingFlag => owningType.GetMethod(methodName, bindingFlag))
            .FirstOrDefault(methodInfo => IsValidMethod(methodInfo, owningType));
    }

    private static bool IsValidMethod(MethodInfo? methodInfo, Type owningType)
    {
        if (methodInfo == null)
        {
            return false;
        }

        var parameters = methodInfo.GetParameters();
        if (parameters.Length != 1 || parameters[0].ParameterType != owningType)
        {
            return false;
        }

        return methodInfo.ReturnType == typeof(bool);
    }
}