using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NP.Utilities;

namespace BEAM.Image.Minimap.Utility;

public class MinimapSettingsProvider : ISettingsProvider<Minimap>
{
    private readonly List<Minimap> _defaultMinimaps;
    private Minimap? _currentDefault;
    private readonly Type _defaultType = typeof(PlotMinimap);
    public MinimapSettingsProvider()
    {
        _defaultMinimaps = new List<Minimap>();
        _defaultMinimaps.AddAll(Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(Minimap)))
            .ToList().ReplaceEveryEntry(TypeToMinimap));
        if (_currentDefault is null)
        {
            var defaults = _defaultMinimaps.Where(t => t.GetType().Equals(_defaultType)).ToList();
            if (!defaults.IsNullOrEmpty())
            {
                _currentDefault = defaults.First();
            } else
            {
                _currentDefault ??= _defaultMinimaps.First();
            }
        }
    }
    
    private Minimap TypeToMinimap(Type T) 
    {
        var result = T.GetConstructors().Where(t => t.GetParameters().Length == 0).ToList();
        if (!result.Any())
        {
            throw new ArgumentException("Input Type 'T' should have a parameterless constructor");
        }
        return (Minimap)result.First().Invoke([]);
        
    }
    
    public ImmutableList<Minimap> GetDefaultObjects()
    {
        return _defaultMinimaps.ToImmutableList();
    }

    public void SetDefaultObject(Minimap? newMinimapDefault)
    {
        if (newMinimapDefault is null)
        {
            return;
        }
        _currentDefault = newMinimapDefault;
    }

    public bool ExistAny()
    {
        return _defaultMinimaps.Count > 0;
    }

    public Minimap? GetDefaultObject()
    {
        return !ExistAny() ? null : _currentDefault;
    }

    public SettingsTransferObject<Minimap> GetDefaultClones()
    {
        Minimap? defaultClone = null;
        List<Minimap> allPossible = new List<Minimap>();
        foreach (var entry in _defaultMinimaps)
        {
            Minimap clone = entry.Clone();
            if (entry.Equals(_currentDefault))
            {
                defaultClone = clone;
            }

            allPossible.Add(clone);
        }
        return new SettingsTransferObject<Minimap>(allPossible.ToImmutableList(), defaultClone);
    }
    
}