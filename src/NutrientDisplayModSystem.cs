using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace NutrientDisplay;

public class NutrientDisplayModSystem : ModSystem
{
    private Harmony? _harmony;
    public static Dictionary<string, string> CropNutrientCache { get; } = new();

    public override bool ShouldLoad(EnumAppSide side) => side == EnumAppSide.Client;

    public override void StartClientSide(ICoreClientAPI api)
    {
        _harmony = new Harmony("com.nutrientdisplay.patch");
        _harmony.PatchAll();

        api.Event.LevelFinalize += () => BuildNutrientCache(api);
    }

    private void BuildNutrientCache(ICoreClientAPI api)
    {
        foreach (var item in api.World.Items)
        {
            if (item is not ItemPlantableSeed) continue;

            string? cropType = item.Variant?["type"];
            if (string.IsNullOrEmpty(cropType)) continue;

            var domain = item.Code?.Domain ?? "game";
            var cropBlockCode = new AssetLocation(domain, $"crop-{cropType}-1");
            var cropBlock = api.World.GetBlock(cropBlockCode);

            if (cropBlock?.CropProps != null)
            {
                var nutrient = cropBlock.CropProps.RequiredNutrient.ToString();
                var itemCode = item.Code?.ToString() ?? "";
                CropNutrientCache[itemCode] = nutrient;
            }
        }
    }

    public override void Dispose()
    {
        _harmony?.UnpatchAll("com.nutrientdisplay.patch");
        base.Dispose();
    }
}

[HarmonyPatch(typeof(ItemStack), nameof(ItemStack.GetName))]
public static class SeedNamePatch
{
    [HarmonyPostfix]
    public static void Postfix(ItemStack __instance, ref string __result)
    {
        if (__instance?.Collectible is not ItemPlantableSeed) return;

        var itemCode = __instance.Collectible.Code?.ToString() ?? "";
        if (NutrientDisplayModSystem.CropNutrientCache.TryGetValue(itemCode, out var nutrient))
        {
            if (!__result.Contains($"({nutrient})"))
            {
                __result = $"{__result} ({nutrient})";
            }
        }
    }
}
