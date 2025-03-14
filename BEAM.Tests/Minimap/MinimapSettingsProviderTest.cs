using Avalonia.Headless.XUnit;
using BEAM.Image.Minimap.Utility;

namespace BEAM.Tests.Minimap;

[Collection("GlobalTests")]
public class MinimapSettingsProviderTests
{
    [AvaloniaFact]
    public void Constructor_ShouldInitializeDefaultMinimaps()
    {
        var provider = new MinimapSettingsProvider();
        Assert.NotEmpty(provider.GetDefaultObjects());
    }

    [AvaloniaFact]
    public void GetDefaultObject_ShouldReturnCurrentDefault()
    {
        var provider = new MinimapSettingsProvider();
        var defaultObject = provider.GetDefaultObject();

        Assert.NotNull(defaultObject);
    }

    [AvaloniaFact]
    public void SetDefaultObject_ShouldUpdateCurrentDefault()
    {
        var provider = new MinimapSettingsProvider();
        var newDefault = provider.GetDefaultObjects()[0];
        provider.SetDefaultObject(newDefault);

        Assert.Equal(newDefault, provider.GetDefaultObject());
    }
}