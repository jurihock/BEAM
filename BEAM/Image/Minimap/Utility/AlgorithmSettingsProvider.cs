using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using BEAM.Image.Minimap.MinimapAlgorithms;
using NP.Utilities;

namespace BEAM.Image.Minimap.Utility;

/// <summary>
/// A provider for concrete <see cref="IMinimapAlgorithm"/>.
/// </summary>
public class AlgorithmSettingsProvider : ISettingsProvider<IMinimapAlgorithm>

{

    private readonly List<IMinimapAlgorithm> _defaultAlgorithms;
    private IMinimapAlgorithm? _currentDefault;
    private readonly Type _defaultType = typeof(RenderedPixelAllThresholdAlgorithm);
    public AlgorithmSettingsProvider()
    {
        _defaultAlgorithms = new List<IMinimapAlgorithm>();
        _defaultAlgorithms.AddAll(Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Contains(typeof(IMinimapAlgorithm)))
            .ToList().ReplaceEveryEntry(TypeToAlgorithm));
        var defaults = _defaultAlgorithms.Where(t => t.GetType().Equals(_defaultType)).ToList();
        if (!defaults.IsNullOrEmpty())
        {
            _currentDefault = defaults.First();
        }
        else
        {
            _currentDefault ??= _defaultAlgorithms.First();
        }
    }

    private IMinimapAlgorithm TypeToAlgorithm(Type T)
    {
        var result = T.GetConstructors().Where(t => t.GetParameters().Length == 0).ToList();
        if (!result.Any())
        {
            throw new ArgumentException("Input Type 'T' should have a parameterless constructor");
        }
        return (IMinimapAlgorithm)result.First().Invoke([]);

    }

    public ImmutableList<IMinimapAlgorithm> GetDefaultObjects()
    {
        return _defaultAlgorithms.ToImmutableList();
    }

    public void SetDefaultObject(IMinimapAlgorithm? newAlgorithmDefault)
    {
        if (newAlgorithmDefault is null)
        {
            return;
        }
        _currentDefault = newAlgorithmDefault;
    }

    public bool ExistAny()
    {
        return _defaultAlgorithms.Count > 0;
    }

    public IMinimapAlgorithm? GetDefaultObject()
    {
        return !ExistAny() ? null : _currentDefault;
    }

    public SettingsTransferObject<IMinimapAlgorithm> GetDefaultClones()
    {
        IMinimapAlgorithm? defaultClone = null;
        List<IMinimapAlgorithm> allPossible = new List<IMinimapAlgorithm>();
        foreach (var entry in _defaultAlgorithms)
        {
            IMinimapAlgorithm clone = entry.Clone();
            if (entry.Equals(_currentDefault))
            {
                defaultClone = clone;
            }

            allPossible.Add(clone);
        }
        return new SettingsTransferObject<IMinimapAlgorithm>(allPossible.ToImmutableList(), defaultClone);
    }
}