﻿using System.Diagnostics.CodeAnalysis;

namespace Aimo.Core;

/// <summary>
/// Extensions for <see cref="IDictionary{TKey,TValue}" /> and <see cref="IReadOnlyDictionary{TKey, TValue}" />.
/// </summary>
public static class DictionaryExtensions
{
#if !NETSTANDARD2_1
    /// <summary>
    /// Returns a value that corresponds to the given key or default if the key doesn't exist.
    /// </summary>
    public static TValue? GetValueOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary,
        [NotNull] TKey key)
    {
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
        return (dictionary.TryGetValue(key, out var result) ? result : default);
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.
    }
#endif
}