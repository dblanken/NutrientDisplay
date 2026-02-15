using NutrientDisplay;

namespace NutrientDisplay.Tests;

public class SeedNamePatchTests : IDisposable
{
    public SeedNamePatchTests()
    {
        NutrientDisplayModSystem.CropNutrientCache.Clear();
    }

    public void Dispose()
    {
        NutrientDisplayModSystem.CropNutrientCache.Clear();
    }

    [Fact]
    public void AppendNutrient_WhenItemInCache_AppendsNutrient()
    {
        NutrientDisplayModSystem.CropNutrientCache["game:seeds-flax"] = "K";

        var result = SeedNamePatch.AppendNutrient("Flax Seeds", "game:seeds-flax");

        Assert.Equal("Flax Seeds (K)", result);
    }

    [Fact]
    public void AppendNutrient_WhenNutrientAlreadyPresent_DoesNotDuplicate()
    {
        NutrientDisplayModSystem.CropNutrientCache["game:seeds-flax"] = "K";

        var result = SeedNamePatch.AppendNutrient("Flax Seeds (K)", "game:seeds-flax");

        Assert.Equal("Flax Seeds (K)", result);
    }

    [Fact]
    public void AppendNutrient_WhenItemNotInCache_ReturnsNameUnchanged()
    {
        var result = SeedNamePatch.AppendNutrient("Flax Seeds", "game:seeds-flax");

        Assert.Equal("Flax Seeds", result);
    }

    [Fact]
    public void AppendNutrient_WhenCacheIsEmpty_ReturnsNameUnchanged()
    {
        var result = SeedNamePatch.AppendNutrient("Flax Seeds", "game:seeds-unknown");

        Assert.Equal("Flax Seeds", result);
    }
}
