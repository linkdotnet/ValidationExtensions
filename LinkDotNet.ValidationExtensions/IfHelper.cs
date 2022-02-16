using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.ValidationExtensions;

public static class IfHelper
{
    public static bool IsRequired(
        this ValidationContext? validationContext,
        string propertyName,
        object? requiredValue,
        bool invert,
        out object? requiredActualValue)
    {
        ArgumentNullException.ThrowIfNull(validationContext);
        ArgumentNullException.ThrowIfNull(propertyName);

        var owningType = validationContext.ObjectType;
        var property = owningType.GetProperty(propertyName);

        if (property == null)
        {
            throw new NotSupportedException($"Can't find {propertyName} on searched type: {owningType.Name}");
        }

        var requiredIfTypeActualValue = property.GetValue(validationContext.ObjectInstance);
        requiredActualValue = requiredIfTypeActualValue;

        if (requiredIfTypeActualValue == null && requiredValue != null)
        {
            return false;
        }

        return true;
    }
}