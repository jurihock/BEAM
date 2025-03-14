using Avalonia.Headless.XUnit;
using BEAM.Image.Minimap.Utility;

namespace BEAM.Tests.Minimap;

[Collection("GlobalTests")]
public class MinimapSettingsProviderTestsTwo
{
    [AvaloniaFact]
    public void Constructor_ShouldPopulateDefaultMinimaps()
    {
        var provider = new MinimapSettingsProvider();
        var defaults = provider.GetDefaultObjects();
        Assert.NotEmpty(defaults);
    }

    [AvaloniaFact]
    public void GetDefaultObject_ShouldReturnNonNullDefault()
    {
        var provider = new MinimapSettingsProvider();
        var defaultObject = provider.GetDefaultObject();
        Assert.NotNull(defaultObject);
    }

    [AvaloniaFact]
    public void SetDefaultObject_ShouldUpdateCurrentDefault()
    {
        var provider = new MinimapSettingsProvider();
        var newDefault = provider.GetDefaultObjects().First();
        provider.SetDefaultObject(newDefault);
        Assert.Equal(newDefault, provider.GetDefaultObject());
    }
    
    [AvaloniaFact]
    public void GetDefaultClones_ShouldReturnClonesAndCurrentDefaultClone()
    {
        var provider = new MinimapSettingsProvider();
        var clones = provider.GetDefaultClones();
        Assert.NotEmpty(clones.AllPossible);
        Assert.NotNull(clones.Active);
    }
}