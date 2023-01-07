using System;
#if !NET48
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#endif

namespace LinkDotNet.ValidationExtensions;

internal static class ArgumentNullExceptionHelper
{
#if NET48
    internal static void ThrowIfNull(object? argument, string paramName)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
#else
    internal static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        => ArgumentNullException.ThrowIfNull(argument);
#endif

}
