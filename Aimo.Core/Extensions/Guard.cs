using System.Diagnostics.CodeAnalysis;

namespace Aimo.Core;

public static class Guard
{
    [return:NotNull]
    public static T ThrowIfNull<T>(this T obj, string? message = null)
    {
        //ArgumentNullException.ThrowIfNull(obj,nameof(obj));

        if (obj is null)
            throw new AppException(message ?? $"{nameof(obj)} is null");
        return obj;
    }
    [return:NotNull]
    public static IEnumerable<T> ThrowIfNullOrEmpty<T>(this IEnumerable<T> collection, string? message = null)
    {

        if (collection.IsNullOrEmpty())
            throw new AppException(message ?? $"{nameof(collection)} has no items");

        return collection;
    }
    [return:NotNull]
    public static async Task<T> ThrowIfNull<T>(this Task<T> objTask, string? message = null) => (await objTask).ThrowIfNull(message);
    [return:NotNull]
    public static async Task<IEnumerable<T>> ThrowIfNull<T>(this Task<IEnumerable<T>> collectionTask, string? message = null) => (await collectionTask).ThrowIfNullOrEmpty(message);

    public static T ThrowIf<T>(this T obj, bool condition, string? message = null, params object[] args)
    {
        ThrowIf(condition, message, args);
        return obj;
    }

    public static void ThrowIf(bool condition, string? message = null, params object[] args)
    {
        if (!condition) return;

        if (message!.IsNullOrWhiteSpace() && args.IsNotEmpty()) message = message!.Format(args);
        throw new AppException(message ?? "unexpected thing happened");
    }
}