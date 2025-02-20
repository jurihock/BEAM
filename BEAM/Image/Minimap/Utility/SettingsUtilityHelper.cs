using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using BEAM.Image.Minimap.MinimapAlgorithms;

namespace BEAM.Image.Minimap.Utility;

public static class SettingsUtilityHelper<T>
{
    private static ISettingsProvider<T>? _provider;
    static SettingsUtilityHelper()
    {
        if (typeof(T).IsAssignableTo(typeof(Minimap)))
        {
            _provider = (ISettingsProvider<T>?)new MinimapSettingsProvider();
        }
        else if (typeof(T).IsAssignableTo(typeof(IMinimapAlgorithm)))
        {
            _provider = (ISettingsProvider<T>?)new AlgorithmSettingsProvider();
        }
        else
        {
            throw new InvalidOperationException("Unsupported type");
        }
    }
    
    public static ImmutableList<T> GetDefaultObjects()
    {
        if(_provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return _provider.GetDefaultObjects();
    }
    
    public static void SetDefaultObject(T? newObjectDefault)
    {
        if(_provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        _provider.SetDefaultObject(newObjectDefault);
    }
    
    
    public static bool ExistAny()
    {
        if(_provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return _provider.ExistAny();
    }
    
    public static T GetDefaultObject()
    {
        if(_provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return _provider.GetDefaultObject()!;
    }

    public static SettingsTransferObject<T> GetDefaultClones()
    {
        if(_provider is null)
        {
            throw new InvalidOperationException("Unsupporterd type");
        }
        return _provider.GetDefaultClones();
    }
    
}