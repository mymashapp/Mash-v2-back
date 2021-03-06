namespace Aimo.Core.Infrastructure;

/// <summary>
/// Provides access to all "singletons" stored by <see cref="Singleton{T}"/>.
/// </summary>
public class BaseSingleton
{
    static BaseSingleton()
    {
        AllSingletons = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Dictionary of type to singleton instances.
    /// </summary>
    public static IDictionary<Type, object> AllSingletons { get; }
}

/// <summary>
/// A statically compiled "singleton" used to store objects throughout the 
/// lifetime of the app domain. Not so much singleton in the pattern's 
/// sense of the word as a standardized way to store single instances.
/// </summary>
/// <typeparam name="T">The type of object to store.</typeparam>
/// <remarks>Access to the instance is not synchronized.</remarks>
public class Singleton<T> : BaseSingleton
{
#pragma warning disable CS8618
    private static T instance;
#pragma warning restore CS8618

    /// <summary>
    /// The singleton instance for the specified type T. Only one instance (at the time) of this object for each type of T.
    /// </summary>
    public static T Instance
    {
        get => instance;
        set
        {
            instance = value;
            AllSingletons[typeof(T)] = value!;
        }
    }
}