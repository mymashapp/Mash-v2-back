using System.Diagnostics.CodeAnalysis;

namespace Aimo.Core;

/// <summary>
/// Extensions for <see cref="ICollection{T}" />.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds multiple items to the collection.
    /// </summary>
    public static void AddRange<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items)
    {
#pragma warning disable CS8777
        if (items!.IsNullOrEmpty()) return;
#pragma warning restore CS8777

        foreach (var item in items)
            collection.Add(item);
    }
}