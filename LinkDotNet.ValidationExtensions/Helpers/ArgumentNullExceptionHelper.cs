using System;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#endif

namespace LinkDotNet.ValidationExtensions;

internal static class ArgumentNullExceptionHelper
{
#if NET6_0_OR_GREATER
    internal static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        => ArgumentNullException.ThrowIfNull(argument);    
#else
    internal static void ThrowIfNull(object? argument, string paramName)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
#endif

}
