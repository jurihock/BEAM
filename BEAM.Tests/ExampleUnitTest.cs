namespace BEAM.Tests;

public class ExampleUnitTest
{
    // [Fact]: testcase
    // This one passes
    [Fact]
    public void ExampleTestCase()
    {
        // Fail instant with
        // Assert.Fail();
        
        Assert.Equal(4, 2 + 2);
    }

    // [Theory]: Only works for passed data
    // Here: for 1, 3, 5 (checks for oddness)
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void ExampleTheory(int i)
    {
        Assert.Equal(1, i % 2);
    }
}