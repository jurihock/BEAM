using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using BEAM.Exceptions;
using BEAM.Image.Minimap.MinimapAlgorithms;
using Microsoft.VisualBasic.CompilerServices;
using NP.Utilities;

namespace BEAM.Image.Minimap.Utility;

public static class PlotAlgorithmSettingsUtilityHelper
{
    private static List<IMinimapAlgorithm>? _defaultAlgorithms;
    private static IMinimapAlgorithm? _currentDefault;
    private static readonly Type DefaultType = typeof(PixelSumAlgorithm);
    
    private static void GenerateAlgorithms()
    {
        _defaultAlgorithms = new List<IMinimapAlgorithm>(); 
        _defaultAlgorithms.AddAll(Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Contains(typeof(IMinimapAlgorithm)))
            .ToList().ReplaceEveryEntry(TypeToAlgorithm));
        var defaults = _defaultAlgorithms.Where(t => t.GetType().Equals(DefaultType)).ToList();
        if (!defaults.IsNullOrEmpty())
        {
            _currentDefault = defaults.First();
        } else
        {
            _currentDefault ??= _defaultAlgorithms.First();
        }
    }

    
    public static ImmutableList<IMinimapAlgorithm> GetDefaultAlgorithms()
    {
        if (_defaultAlgorithms is null)
        {
            GenerateAlgorithms();
        }

#pragma warning disable CS8604 // Possible null reference argument. This is a false positive, as the method GenerateMinimaps() will always initialize _defaultMinimaps
        return _defaultAlgorithms.ToImmutableList();
#pragma warning restore CS8604 // Possible null reference argument.
    }
    
    
    public static void SetDefaultAlgorithm(IMinimapAlgorithm? newAlgorithmDefault)
    {
        
        if (newAlgorithmDefault is null)
        {
            Console.WriteLine("Default not set to due to null");
            return;
        }
        Console.WriteLine("Default set to+ " + newAlgorithmDefault.GetName());
        _currentDefault = newAlgorithmDefault;
    }
    
    
    private static List<K> ReplaceEveryEntry<T, K>(this IEnumerable<T> inputList, Func<T, K> conversion)
    {
        List<K> output = new List<K>();
        foreach (var element in inputList)
        {
            output.Add(conversion(element));
        }

        return output;
    }

    public static bool ExistAny()
    {
        if(_defaultAlgorithms is null) GenerateAlgorithms();
#pragma warning disable CS8602 // Dereference of a possibly null reference. This is a false positive, as the method GenerateMinimaps() will always initialize _defaultMinimaps
        return _defaultAlgorithms.Count > 0;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
    

    private static IMinimapAlgorithm TypeToAlgorithm(Type T) 
    {
        var result = T.GetConstructors().Where(t => t.GetParameters().Length == 0).ToList();
        if (!result.Any())
        {
            throw new ArgumentException("Input Type 'T' should have a parameterless constructor");
        }
        return (IMinimapAlgorithm)result.First().Invoke([]);
        
    }
    public static IMinimapAlgorithm GetDefaultAlgorithm()
    {
        if(_defaultAlgorithms is null) GenerateAlgorithms();
        if(_currentDefault is null)
        {
            _currentDefault = _defaultAlgorithms!.First();
        }

        if (!ExistAny()) throw new InvalidStateException();
        Console.WriteLine("returned current default as: " + _currentDefault.GetName());
        return _currentDefault;
    }

    public static SettingsTransferObject<IMinimapAlgorithm> GetDefaultClones()
    {
        IMinimapAlgorithm? defaultClone = null;
        List<IMinimapAlgorithm> allPossible = new List<IMinimapAlgorithm>();
        if(_defaultAlgorithms is null) GenerateAlgorithms();
        foreach (var entry in _defaultAlgorithms!)
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

public class SettingsTransferObject<T>(ImmutableList<T> allPossible, T? active)
{
    public T? Active { get; set; } = active;
    public readonly ImmutableList<T> AllPossible = allPossible;
}