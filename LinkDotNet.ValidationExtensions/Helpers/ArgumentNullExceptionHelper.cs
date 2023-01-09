using System;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#endif

namespace LinkDotNet.ValidationExtensions;

internal static class ArgumentNullExceptionHelper
{
#if !NET7_0_OR_GREATER
    private static readonly string ArgumentEmptyString = "The value cannot be an empty string.";
#endif

#if NET7_0_OR_GREATER
    internal static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        => ArgumentNullException.ThrowIfNull(argument, paramName);
#elif NET6_0
    internal static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        => ArgumentNullException.ThrowIfNull(argument, paramName);
#else
    internal static void ThrowIfNull(object? argument, string paramName)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
#endif

#if NET7_0_OR_GREATER
    internal static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        => ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
#elif NET6_0
    internal static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            ThrowNullOrEmptyException(argument, paramName);
        }
    }

    [DoesNotReturn]
    private static void ThrowNullOrEmptyException(string? argument, string? paramName)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        throw new ArgumentException(ArgumentEmptyString, paramName);
    }
#else
    internal static void ThrowIfNullOrEmpty(string? argument, string paramName)
    {
        if (string.IsNullOrEmpty(argument))
        {
            ThrowNullOrEmptyException(argument, paramName);
        }
    }

    private static void ThrowNullOrEmptyException(string? argument, string paramName)
    {
        ThrowIfNull(argument, paramName);
        throw new ArgumentException(ArgumentEmptyString, paramName);
    }
#endif

}
