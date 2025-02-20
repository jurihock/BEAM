using BEAM.Image.Minimap.Utility;

namespace BEAM.Tests.Minimap;

public class MinimapSettingsProviderTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultMinimaps()
    {
        var provider = new MinimapSettingsProvider();
        Assert.NotEmpty(provider.GetDefaultObjects());
    }

    [Fact]
    public void GetDefaultObject_ShouldReturnCurrentDefault()
    {
        var provider = new MinimapSettingsProvider();
        var defaultObject = provider.GetDefaultObject();

        Assert.NotNull(defaultObject);
    }

    [Fact]
    public void SetDefaultObject_ShouldUpdateCurrentDefault()
    {
        var provider = new MinimapSettingsProvider();
        var newDefault = provider.GetDefaultObjects()[0];
        provider.SetDefaultObject(newDefault);

        Assert.Equal(newDefault, provider.GetDefaultObject());
    }
}