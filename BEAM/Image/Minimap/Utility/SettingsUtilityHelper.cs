using System;
using System.Collections.Immutable;
using BEAM.Image.Minimap.MinimapAlgorithms;

namespace BEAM.Image.Minimap.Utility;

/// <summary>
/// A static wrapper for accessing different providers based on their type without needing the instantiate and store them every time.
/// </summary>
/// <typeparam name="T">The concrete type of the provider the caller desires the access.</typeparam>
public static class SettingsUtilityHelper<T>
{
    /// <summary>
    /// The provider used to handle the current calls. This is different for every Type T.
    /// </summary>
    private static readonly ISettingsProvider<T>? Provider;
    
    /// <summary>
    /// Static class constructor being called the first time a method if the static class with the concrete generic type is being called.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the type T has no corresponding provider.</exception>
    static SettingsUtilityHelper()
    {
        if (typeof(T).IsAssignableTo(typeof(Minimap)))
        {
            Provider = (ISettingsProvider<T>?)new MinimapSettingsProvider();
        }
        else if (typeof(T).IsAssignableTo(typeof(IMinimapAlgorithm)))
        {
            Provider = (ISettingsProvider<T>?)new AlgorithmSettingsProvider();
        }
        else
        {
            throw new InvalidOperationException("Unsupported type");
        }
    }
    
    /// <summary>
    /// Sets the object which is currently being marked as the default of all possible default objects.
    /// Returns null only if there are no possible default objects, meaning that <see cref="GetDefaultObjects"/> returns an empty list.
    /// </summary>
    /// <returns></returns>
    public static ImmutableList<T> GetDefaultObjects()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupported type");
        }
        return Provider.GetDefaultObjects();
    }
    
    /// <summary>
    /// Sets the object which is currently being marked as the default of all possible default objects.
    /// This parameter should be an element in the list returned by <see cref="GetDefaultObjects"/>.
    /// </summary>
    /// <param name="newDefaultObject">The new default object out of all possible default objects.</param>
    public static void SetDefaultObject(T? newDefaultObject)
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupported type");
        }
        Provider.SetDefaultObject(newDefaultObject);
    }
    
    /// <summary>
    /// Checks whether there are any possible default objects.
    /// This is useful for checking whether the provider has found any manageable objects.
    /// </summary>
    /// <returns>A boolean representing whether there are any possible default objects.</returns>
    public static bool ExistAny()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupported type");
        }
        return Provider.ExistAny();
    }
    
    /// <summary>
    /// Sets the object which is currently being marked as the default of all possible default objects.
    /// Returns null only if there are no possible default objects, meaning that <see cref="GetDefaultObjects"/> returns an empty list.
    /// </summary>
    /// <returns>The default object or null of none exists.</returns>
    public static T? GetDefaultObject()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupported type");
        }
        return Provider.GetDefaultObject();
    }

    /// <summary>
    /// Returns a clone of all possible default objects alongside the active one.
    /// The instance of the active object is can exactly be found in the list of the clones of all possible default objects.
    /// </summary>
    /// <returns>An <see cref="SettingsTransferObject{T}"/> containing the clones of the currently default object
    /// and all possible default objects.</returns>
    /// <exception cref="InvalidOperationException">If the type T has no corresponding provider.</exception>
    public static SettingsTransferObject<T> GetDefaultClones()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupported type");
        }
        return Provider.GetDefaultClones();
    }
    
}