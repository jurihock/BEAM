using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BEAM.Exceptions;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;

namespace BEAM.Image.Minimap;

public class SettingsStorer
{
    private List<IMinimapAlgorithm> _defaultAlgorithms;
    private IMinimapAlgorithm? _currentDefaultAlgorithm;
    
    private List<Minimap> _defaultMinimaps;
    private Minimap? _currentDefaultMinimap;
    
    public SettingsStorer()
    {
        var algoCopy = PlotAlgorithmSettingsUtilityHelper.GetDefaultClones();
        _defaultAlgorithms = algoCopy.AllPossible.ToList();
        _currentDefaultAlgorithm = algoCopy.Active;
        var mapCopy = MinimapSettingsUtilityHelper.GetDefaultClones();
        _defaultMinimaps = mapCopy.AllPossible.ToList();
        _currentDefaultMinimap = mapCopy.Active;
        if(_currentDefaultAlgorithm is null | _defaultMinimaps is null )
        {
            throw new InvalidStateException();
        }
    }
    
    public ImmutableList<IMinimapAlgorithm> GetDefaultAlgorithms()
    {
        return _defaultAlgorithms.ToImmutableList();
    }
    
    
    public void SetDefaultAlgorithm(IMinimapAlgorithm? newAlgorithmDefault)
    {
        
        if (newAlgorithmDefault is null)
        {
            return;
        }
        Console.WriteLine("Default set to+ " + newAlgorithmDefault.GetName());
        _currentDefaultAlgorithm = newAlgorithmDefault;
    }
    
    public IMinimapAlgorithm GetDefaultAlgorithm()
    {
        if (_currentDefaultAlgorithm is null)
        {
            _currentDefaultAlgorithm = _defaultAlgorithms.First();
        }
        return _currentDefaultAlgorithm;
    }
    
    public SettingsTransferObject<IMinimapAlgorithm> GetDefaultAlgorithmClones()
    {
        IMinimapAlgorithm? defaultClone = null;
        List<IMinimapAlgorithm> allPossible = new List<IMinimapAlgorithm>();
        foreach (var entry in _defaultAlgorithms!)
        {
            IMinimapAlgorithm clone = entry.Clone();
            if (entry.Equals(_currentDefaultAlgorithm))
            {
                defaultClone = clone;
            }

            allPossible.Add(clone);
        }
        return new SettingsTransferObject<IMinimapAlgorithm>(allPossible.ToImmutableList(), defaultClone);
    }
    
    
    public ImmutableList<Minimap> GetDefaultMinimaps()
    {

        return _defaultMinimaps.ToImmutableList();
    }
    
    
    public void SetDefaultMinimap(Minimap? newMinimapDefault)
    {
        if (newMinimapDefault is null)
        {
            return;
        }
        _currentDefaultMinimap = newMinimapDefault;
    }

    public bool ExistsAnyAlgorithm()
    {
        return _defaultAlgorithms.Count > 0;
    }

    public bool ExistAnyMinimap()
    {
        return _defaultMinimaps.Count > 0;
    }
    
    
    
    public Minimap? GetDefaultMinimap()
    {
        return _currentDefaultMinimap;
    }
    
    public SettingsTransferObject<Minimap> GetDefaultMinimapClones()
    {
        Minimap? defaultClone = null;
        List<Minimap> allPossible = new List<Minimap>();
        foreach (var entry in _defaultMinimaps!)
        {
            Minimap clone = entry.Clone();
            if (entry.Equals(_currentDefaultMinimap))
            {
                defaultClone = clone;
            }

            allPossible.Add(clone);
        }
        return new SettingsTransferObject<Minimap>(allPossible.ToImmutableList(), defaultClone);
    }
}