using System;
using System.Collections.Immutable;
using BEAM.Image.Minimap.MinimapAlgorithms;

namespace BEAM.Image.Minimap.Utility;

public static class SettingsUtilityHelper<T>
{
    private static readonly ISettingsProvider<T>? Provider;
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
    
    public static ImmutableList<T> GetDefaultObjects()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return Provider.GetDefaultObjects();
    }
    
    public static void SetDefaultObject(T? newObjectDefault)
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        Provider.SetDefaultObject(newObjectDefault);
    }
    
    
    public static bool ExistAny()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return Provider.ExistAny();
    }
    
    public static T? GetDefaultObject()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return Provider.GetDefaultObject();
    }

    public static SettingsTransferObject<T> GetDefaultClones()
    {
        if(Provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return Provider.GetDefaultClones();
    }
    
}