using System.Security.Cryptography;
using Newtonsoft.Json;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

if (args.Length != 3) throw new ArgumentException("Usage: CookedEconomyPatcher <private-PatchSources-root> <fresh-output-root> <balance.json>");
var source = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
if (Directory.Exists(output) || File.Exists(output)) throw new IOException("Output exists; choose a fresh stage.");
if (output.StartsWith(source + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || output == source)
    throw new IOException("Output must be outside the source collection.");
var cfg = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(args[2]));
var records = new List<object>();
var patches = new[] {
    (Path: "Dungeons/Content/Content_DLC4/Decor/Prefab/Functional/GoldChests/BP_GoldChest_Small.uasset", Export: "ConsumableDrop_GEN_VARIABLE", Category: "EDropCategory::Gold", OldMin: 4, OldMax: 6, Min: (int)cfg["gold"]!["normalChest"]![0]!, Max: (int)cfg["gold"]!["normalChest"]![1]!),
    (Path: "Dungeons/Content/Content_DLC4/Decor/Prefab/Functional/GoldChests/BP_GoldChest_Rare.uasset", Export: "ConsumableDrop_GEN_VARIABLE", Category: "EDropCategory::Gold", OldMin: 8, OldMax: 10, Min: (int)cfg["gold"]!["rareChest"]![0]!, Max: (int)cfg["gold"]!["rareChest"]![1]!),
    (Path: "Dungeons/Content/Decor/Prefabs/_Urns/LootUrnsBlueprints/BP_LootUrnBase.uasset", Export: "EmeraldDrop_GEN_VARIABLE", Category: "EDropCategory::Emerald", OldMin: 3, OldMax: 7, Min: (int)cfg["emeralds"]!["observedBaseUrnFirstPass"]![0]!, Max: (int)cfg["emeralds"]!["observedBaseUrnFirstPass"]![1]!)
};
// Validate every source before creating any output.
var loaded = patches.Select(p => {
    if (p.Min < p.OldMin || p.Max < p.Min || p.Max > 1000) throw new InvalidDataException("Invalid reward range.");
    var input = Path.Combine(source, p.Path.Replace('/', Path.DirectorySeparatorChar));
    var asset = new UAsset(input, EngineVersion.VER_UE4_22);
    var export = asset.Exports.OfType<NormalExport>().Single(x => x.ObjectName.ToString() == p.Export);
    var data = export.Data.OfType<StructPropertyData>().Single(x => x.Name.ToString() == "DropData");
    var category = data.Value.OfType<EnumPropertyData>().Single(x => x.Name.ToString() == "Category");
    if (category.Value.ToString() != p.Category) throw new InvalidDataException("Drop category mismatch: " + p.Path);
    var min = data.Value.OfType<IntPropertyData>().Single(x => x.Name.ToString() == "MinAmount");
    var max = data.Value.OfType<IntPropertyData>().Single(x => x.Name.ToString() == "MaxAmount");
    if (min.Value != p.OldMin || max.Value != p.OldMax) throw new InvalidDataException("Unexpected vanilla reward values: " + p.Path);
    min.Value = p.Min; max.Value = p.Max;
    return (Patch: p, Input: input, Asset: asset);
}).ToArray();
// Camp reward uses a separate native LobbyChest scalar, not the urn/chest DropData struct.
var campPath = "Dungeons/Content/Decor/Prefabs/RewardChest/BP_LobbyChest.uasset";
var campInput = Path.Combine(source, campPath.Replace('/', Path.DirectorySeparatorChar));
var campAsset = new UAsset(campInput, EngineVersion.VER_UE4_22);
var campDefault = campAsset.Exports.OfType<NormalExport>().Single(x => x.ObjectName.ToString() == "Default__BP_LobbyChest_C");
var campReward = campDefault.Data.OfType<IntPropertyData>().Single(x => x.Name.ToString() == "EmeraldsReward");
var campAfter = (int)cfg["emeralds"]!["campChest"]!;
if (campReward.Value != 50 || campAfter < 50 || campAfter > 1000) throw new InvalidDataException("Unexpected/invalid Camp emerald reward.");
campReward.Value = campAfter;
Directory.CreateDirectory(output);
try {
    foreach (var item in loaded) {
        var p = item.Patch;
        var destination = Path.Combine(output, p.Path.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        item.Asset.Write(destination);
        var reread = new UAsset(destination, EngineVersion.VER_UE4_22);
        // Compare the full parsed package, including graph/dependency data, with the exact expected mutation.
        if (item.Asset.SerializeJson() != reread.SerializeJson()) throw new InvalidDataException("Package semantic re-read mismatch: " + p.Path);
        var export = reread.Exports.OfType<NormalExport>().Single(x => x.ObjectName.ToString() == p.Export);
        var data = export.Data.OfType<StructPropertyData>().Single(x => x.Name.ToString() == "DropData");
        if (data.Value.OfType<IntPropertyData>().Single(x => x.Name.ToString() == "MinAmount").Value != p.Min
            || data.Value.OfType<IntPropertyData>().Single(x => x.Name.ToString() == "MaxAmount").Value != p.Max)
            throw new InvalidDataException("Reward range not preserved.");
        records.Add(new { package = p.Path, component = p.Export, category = p.Category, before = new[] {p.OldMin,p.OldMax}, after = new[] {p.Min,p.Max},
            inputSha256 = Hash(item.Input), outputSha256 = Hash(destination), roundTrip = true });
    }
    var campDestination = Path.Combine(output, campPath.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(campDestination)!);
    campAsset.Write(campDestination);
    var campReread = new UAsset(campDestination, EngineVersion.VER_UE4_22);
    if (campAsset.SerializeJson() != campReread.SerializeJson()) throw new InvalidDataException("Camp reward package re-read mismatch.");
    records.Add(new { package = campPath, component = "Default__BP_LobbyChest_C", property = "EmeraldsReward", before = 50, after = campAfter,
        inputSha256 = Hash(campInput), outputSha256 = Hash(campDestination), roundTrip = true });
    File.WriteAllText(Path.Combine(output,"ECONOMY_PATCH_REPORT.json"), JsonConvert.SerializeObject(records,Formatting.Indented));
    Console.WriteLine("Patched and re-opened four reward packages; native bytecode unchanged.");
} catch {
    Directory.Delete(output,true);
    throw;
}
static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
