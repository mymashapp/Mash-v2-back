using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Aimo.Core;

public static class NullExtensions
{
    [Obsolete("Use is null")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull([NotNullWhen(false)] this object? obj) => obj is null;

    [Obsolete("Use is not null")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNull([NotNullWhen(true)] this object? obj) => obj is not null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T> obj) => !obj?.Any() ?? true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>([NotNullWhen(true)] this IEnumerable<T> obj) => !IsNullOrEmpty(obj);
}