using System.Collections.Immutable;

namespace BEAM.Image.Minimap.Utility;

/// <summary>
/// A class responsible for saving a collection of objects.
/// One of these abjects is marked as a default. Useful for settings e.g. selecting a minimap.
/// </summary>
/// <typeparam name="TResult">The type which is being managed by the provider.</typeparam>
public interface ISettingsProvider<TResult>
{
    /// <summary>
    /// Returns an immutable list of the real instances of the default objects.
    /// </summary>
    /// <returns></returns>
    public ImmutableList<TResult> GetDefaultObjects();

    /// <summary>
    /// Sets the object which is currently being marked as the default of all possible default objects.
    /// This parameter should be an element in the list returned by <see cref="GetDefaultObjects"/>.
    /// </summary>
    /// <param name="newDefaultObject">The new default object out of all possible default objects.</param>
    public void SetDefaultObject(TResult? newDefaultObject);

    /// <summary>
    /// Checks whether there are any possible default objects.
    /// This is useful for checking whether the provider has found any manageable objects.
    /// </summary>
    /// <returns>A boolean representing whether there are any possible default objects.</returns>
    public bool ExistAny();
    
    /// <summary>
    /// Sets the object which is currently being marked as the default of all possible default objects.
    /// Returns null only if there are no possible default objects, meaning that <see cref="GetDefaultObjects"/> returns an empty list.
    /// </summary>
    /// <returns>The default object or null of none exists.</returns>
    public TResult? GetDefaultObject();

    /// <summary>
    /// Returns a clone of all possible default objects alongside the active one.
    /// The instance of the active object is can exactly be found in the list of the clones of all possible default objects.
    /// </summary>
    /// <returns>An <see cref="SettingsTransferObject{TResult}"/> containing the clones of the currently default object
    /// and all possible default objects.</returns>
    public SettingsTransferObject<TResult> GetDefaultClones();

}