using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NP.Utilities;

namespace BEAM.Image.Minimap;

public static class MinimapSettingsUtilityHelper
{
    private static NumberMapper<Type>? _mapping;
    private static List<Minimap>? _defaultMinimaps;
    private static Minimap? _currentDefault;
    
    private static int _currentSelection = 0;
    private static void CreateMapping()
    {
        var minimaps = Assembly.GetExecutingAssembly().GetTypes().Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(Image.Minimap.Minimap))).ToList();
        _mapping = new NumberMapper<Type>(minimaps);
    }

    
    public static ImmutableList<Minimap> GetDefaultMinimaps()
    {
        if(_mapping is null) CreateMapping();
        if (_defaultMinimaps is null)
        {
            _defaultMinimaps = new List<Minimap>(); 
            _defaultMinimaps.AddAll(_mapping?.ForAllGenerics(TypeToMinimap) ?? new List<Minimap>());
        }

        return _defaultMinimaps.ToImmutableList();
    }

    public static int GetCurrentSelection()
    {
        return _currentSelection;  
    }
    
    public static Minimap GetDefaultMinimap()
    {
        if(_mapping is null) CreateMapping();
        if(_currentDefault is null) _currentDefault = TypeToMinimap(_mapping.FromNumber(0));
        return _currentDefault;
    }
    
    public static void SetDefaultMinimap(Minimap? newMinimapDefault)
    {
        if (newMinimapDefault is null)
        {
            return;
        }
        _currentDefault = newMinimapDefault;
    }

    public static List<Minimap> GetNewMinimapSet()
    {
        return ReplaceEveryEntry<Minimap, Minimap>(GetDefaultMinimaps(), x => x.Clone());
    }
    
    public static List<K> ReplaceEveryEntry<T,K>(this IEnumerable<T> inputList, Func<T, K> conversion)
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
        if(_mapping is null) CreateMapping();
#pragma warning disable CS8602 // Dereference of a possibly null reference. This is a false positive, as the method CreateMapping() will always initialize _mapping
        return _mapping.GetSize() > 0;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    public static bool SetSelection(int newValue)
    {
        if(_mapping is null) CreateMapping();
#pragma warning disable CS8602 // Dereference of a possibly null reference. This is a false positive, as the method CreateMapping() will always initialize _mapping
        if(newValue < 0 || newValue >= _mapping.GetSize()) return false;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        _currentSelection = newValue;
        return true;
    }

    private static Minimap TypeToMinimap(Type T) 
    {
        var result = T.GetConstructors().Where(t => t.GetParameters().Length == 0).ToList();
        if (!result.Any())
        {
            throw new ArgumentException("Input Type 'T' should have a parameterless constructor");
        }
        return (Minimap)result.First().Invoke([]);
        
    }


    private class NumberMapper<T> where T : notnull
    {
        private readonly int _size;
        private readonly T[] _fromIndex;
        private readonly Dictionary<T, int> _fromType;

        public NumberMapper(List<T> toMap)
        {
            _size = toMap.Count;
            _fromIndex = new T[_size];
            _fromType = new(_size);
            int i = 0;
            foreach (T element in toMap)
            {
                _fromIndex[i] = toMap[i];
                _fromType.Add(element, i);
                i++;
            }
        }

        public T FromNumber(int number)
        {
            if (number < 0 || number >= _size) throw new ArgumentOutOfRangeException(nameof(number));
            return _fromIndex[number];
        }

        public int FromType(T type)
        {
            return _fromType.GetValueOrDefault(type, -1);
        }

        public int GetSize()
        {
            return _size;
        }

        public List<K> ForAllGenerics<K>(Func<T, K> conversion)
        {
            List<K> output = new List<K>();
            foreach (var element in _fromIndex)
            {
                output.Add(conversion(element));
            }

            return output;
        }
    }
}