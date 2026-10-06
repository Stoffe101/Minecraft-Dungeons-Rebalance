using System.Security.Cryptography;
using Newtonsoft.Json;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;

// Private cooked-asset foundation with an optional non-interactive placement preview.
// Do not change native transaction flags, prices, or player inventory here.
if (args.Length == 2 && args[0] == "--self-test-presentation") { UniquePresentation.SelfTest(args[1]); return; }
if (args.Length == 2 && args[0] == "--self-test-selection") { SelectedItemReaders.SelfTest(args[1]); return; }
if (args.Length == 2 && args[0] == "--self-test-screens") { MerchantScreens.SelfTest(args[1]); return; }
if (args.Length == 2 && args[0] == "--self-test-placement") { CampPlacement.SelfTest(args[1]); return; }
if (args.Length == 2 && args[0] == "--self-test-load-contracts") { FunctionLoadContract.SelfTest(args[1]); return; }
if (args.Length == 2 && args[0] == "--check-load-contracts") { FunctionLoadContract.ValidateOwned(new UAsset(args[1], EngineVersion.VER_UE4_22)); Console.WriteLine("Generated Function load contracts validated."); return; }
var placementPreview = args.Length == 3 && args[0] == "--placement-preview";
if (placementPreview) args = args.Skip(1).ToArray();
if (args.Length != 2) throw new ArgumentException("Usage: CampSmithStager <private-PatchSources-root> <fresh-private-output-root>");
var source = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
if (Directory.Exists(output) || File.Exists(output)) throw new IOException("Output exists; choose a fresh stage.");
if (Inside(output, source) || Inside(source, output)) throw new IOException("Source and output must be separate directory trees.");
const string destinationFolder = "Mods/MinecraftDungeonsRebalance/Camp";
var specs = new[] {
    new Spec("Content_Season1/Decor/Prefab/Merchants", "BP_TowerArtisanMerchant", "BP_RebalanceCampUniquesmith", "TowerArtisanMerchantDef", "TowerUniquesmith"),
    new Spec("Content_Season1/Decor/Prefab/Merchants", "BP_TowerBlacksmithMerchant", "BP_RebalanceCampPowersmith", "TowerBlacksmithMerchantDef", "TowerBlacksmith"),
    new Spec("Content_Season1/Decor/Prefab/Merchants", "BP_TowerGilderMerchant", "BP_RebalanceCampGildsmith", "TowerGilderMerchantDef", "TowerGildsmith"),
    new Spec("Content_Season1/UI/Merchant", "UMG_TowerMerchantArtisanContent", "UMG_RebalanceCampUniquesmithContent", null, null),
    new Spec("Content_Season1/UI/Merchant", "UMG_TowerMerchantBlacksmithContent", "UMG_RebalanceCampPowersmithContent", null, null),
    new Spec("Content_Season1/UI/Merchant", "UMG_TowerMerchantGilderContent", "UMG_RebalanceCampGildsmithContent", null, null)
};
string Relocate(string name) {
    foreach (var s in specs) {
        name = name.Replace($"/Game/{s.Folder}/{s.Original}", $"/Game/{destinationFolder}/{s.CloneName}", StringComparison.Ordinal);
        name = name.Replace(s.Original, s.CloneName, StringComparison.Ordinal);
    }
    return name;
}
// Validate all six originals before creating any stage.
var loaded = specs.Select(s => {
    var path = Path.Combine(source, "Dungeons", "Content", s.Folder, s.Original + ".uasset");
    var inputHashes = new[] { path, Path.ChangeExtension(path, ".uexp") }.ToDictionary(p => Path.GetFileName(p)!, Hash);
    var a = new UAsset(path, EngineVersion.VER_UE4_22);
    var cdo = a.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString() == "Default__" + s.Original + "_C");
    if (s.Definition != null) {
        var component = a.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString() == "MerchantDef");
        var reference = component.Data.OfType<ObjectPropertyData>().Single(p => p.Name.ToString() == "MerchantDefinition").Value;
        if (!reference.IsImport() || a.Imports[-reference.Index - 1].ObjectName.ToString() != s.Definition)
            throw new InvalidDataException("Unexpected native merchant definition: " + s.Original);
        foreach (var flag in new[] { "bOneTransactionMerchant", "bOneTransactionPerPlayerMerchant" })
            if (!cdo.Data.OfType<BoolPropertyData>().Single(p => p.Name.ToString() == flag).Value)
                throw new InvalidDataException("Expected original Tower transaction flags: " + s.Original);
        if (cdo.Data.OfType<EnumPropertyData>().Single(p => p.Name.ToString() == "merchantType").Value.ToString() != "EMerchantType::" + s.MerchantType)
            throw new InvalidDataException("Unexpected merchant type: " + s.Original);
    }
    var changed = 0;
    for (var i = 0; i < a.GetNameMapIndexList().Count; i++) {
        var before = a.GetNameReference(i).Value;
        var after = Relocate(before);
        if (before == after) continue;
        a.SetNameReference(i, new FString(after)); changed++;
    }
    if (changed == 0) throw new InvalidDataException("No clone names relocated: " + s.Original);
    var originalFunctions = a.Exports.OfType<FunctionExport>().Count();
    if (s.CloneName == "UMG_RebalanceCampUniquesmithContent") UniquePresentation.Add(a);
    if (s.Definition == null) SelectedItemReaders.Add(a);
    if (s.Definition != null) MerchantScreens.BindActor(a, s.CloneName["BP_RebalanceCamp".Length..]);
    return (Spec: s, Input: path, InputHashes: inputHashes, Asset: a, Changed: changed, OriginalFunctions: originalFunctions);
}).ToArray();
var screens = MerchantScreens.Prepare(source);
var placement = placementPreview ? CampPlacement.Prepare(source) : null;
Directory.CreateDirectory(output);
try {
    var report = new List<object>();
    foreach (var item in loaded) {
        var s = item.Spec;
        var relative = $"Dungeons/Content/{destinationFolder}/{s.CloneName}.uasset";
        var path = Path.Combine(output, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        item.Asset.Write(path);
        var expected = item.Asset.SerializeJson();
        var reread = new UAsset(path, EngineVersion.VER_UE4_22);
        if (expected != reread.SerializeJson()) throw new InvalidDataException("Clone semantic round-trip mismatch: " + s.CloneName);
        if (s.CloneName == "UMG_RebalanceCampUniquesmithContent") UniquePresentation.Validate(reread);
        if (s.Definition == null) SelectedItemReaders.Validate(reread);
        if (s.Definition != null) MerchantScreens.ValidateActor(reread, s.CloneName["BP_RebalanceCamp".Length..]);
        if (reread.GetNameMapIndexList().Any(n => specs.Any(original => n.Value.Contains(original.Original, StringComparison.Ordinal))))
            throw new InvalidDataException("Original self/cross references remain: " + s.CloneName);
        if (!reread.Exports.OfType<NormalExport>().Any(e => e.ObjectName.ToString() == "Default__" + s.CloneName + "_C"))
            throw new InvalidDataException("Clone CDO missing: " + s.CloneName);
        foreach (var (file, hash) in item.InputHashes)
            if (Hash(Path.Combine(Path.GetDirectoryName(item.Input)!, file!)) != hash)
                throw new InvalidDataException("Source changed during staging.");
        report.Add(new { sourcePackage = $"Dungeons/Content/{s.Folder}/{s.Original}.uasset", clonePackage = relative,
            relocatedNames = item.Changed, nativeDefinition = s.Definition, merchantType = s.MerchantType,
            originalHashes = item.InputHashes, outputHashes = new[] { path, Path.ChangeExtension(path, ".uexp") }.ToDictionary(p => Path.GetFileName(p)!, Hash),
            semanticRoundTrip = true, originalFunctionCount = item.OriginalFunctions,
            addedPresentationFunctions = s.CloneName == "UMG_RebalanceCampUniquesmithContent" ? 3 : 0,
            addedSelectionReadFunctions = s.Definition == null ? 2 : 0,
            functionCount = reread.Exports.OfType<FunctionExport>().Count() });
    }
    foreach (var screen in screens) {
        var name = MerchantScreens.Screen(screen.Service);
        var relative = $"Dungeons/Content/{destinationFolder}/{name}.uasset";
        var path = Path.Combine(output, relative);
        var originals = new[] { screen.Source, Path.ChangeExtension(screen.Source, ".uexp") }.ToDictionary(p => Path.GetFileName(p)!, Hash);
        screen.Asset.Write(path);
        var expected = screen.Asset.SerializeJson();
        var reread = new UAsset(path, EngineVersion.VER_UE4_22);
        if (expected != reread.SerializeJson()) throw new InvalidDataException("Camp screen semantic round-trip mismatch.");
        MerchantScreens.Validate(reread, screen.Service, screen.PreservedGraphs);
        foreach (var (file, hash) in originals)
            if (Hash(Path.Combine(Path.GetDirectoryName(screen.Source)!, file)) != hash) throw new InvalidDataException("Merchant source changed.");
        report.Add(new { sourcePackage = "Dungeons/Content/UI/Merchant/UMG_Merchant.uasset", clonePackage = relative,
            relocatedNames = screen.RelocatedNames, nativeDefinition = (string?)null, merchantType = (string?)null,
            originalHashes = originals, outputHashes = new[] { path, Path.ChangeExtension(path, ".uexp") }.ToDictionary(p => Path.GetFileName(p)!, Hash),
            semanticRoundTrip = true, originalFunctionCount = reread.Exports.OfType<FunctionExport>().Count(),
            addedPresentationFunctions = 0, addedSelectionReadFunctions = 0, functionCount = reread.Exports.OfType<FunctionExport>().Count(),
            nativeDecisionGraphsPreserved = true, contentDispatch = MerchantScreens.ClassPath(MerchantScreens.Content(screen.Service)) });
    }
    if (placement != null) {
        const string relative = "Dungeons/Content/Decor/Prefabs/RewardChest/BP_LobbyChest.uasset";
        var path = Path.Combine(output, relative); Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var original = Path.Combine(source, relative);
        var hashes = new[] { original, Path.ChangeExtension(original, ".uexp") }.ToDictionary(p => Path.GetFileName(p)!, Hash);
        placement.Write(path); var expected = placement.SerializeJson();
        var reread = new UAsset(path, EngineVersion.VER_UE4_22);
        if (expected != reread.SerializeJson()) throw new InvalidDataException("Placement round-trip mismatch.");
        CampPlacement.Validate(reread);
        foreach (var (file, hash) in hashes)
            if (Hash(Path.Combine(Path.GetDirectoryName(original)!, file)) != hash) throw new InvalidDataException("Camp chest source changed.");
        report.Add(new { sourcePackage = relative, clonePackage = relative, nativeDefinition = (string?)null,
            originalHashes = hashes, outputHashes = new[] { path, Path.ChangeExtension(path, ".uexp") }.ToDictionary(p => Path.GetFileName(p)!, Hash),
            semanticRoundTrip = true, placementHook = "ReceiveBeginPlay", hostOnly = true, replicated = false, interactionsDisabled = true });
    }
    File.WriteAllText(Path.Combine(output, "CAMP_SMITH_STAGE_REPORT.json"), JsonConvert.SerializeObject(new {
        status = placementPreview ? "camp_placement_preview_only" : "private_asset_foundation_only", deployable = placementPreview,
        functionCreationPreloadsValidated = true, gameplayVerified = false, campPlacementImplemented = placementPreview, npcPreviewOnly = placementPreview,
        interactionsDisabledBySpawn = placementPreview,
        paidTransactionsImplemented = false, uniquePickerImplemented = false,
        uniquePresentationBindingsImplemented = true,
        selectedItemReadBindingsImplemented = true,
        actorScreenBindingsImplemented = true,
        campContentDispatchImplemented = true,
        nativeDecisionFlowRetained = true,
        nativeTowerFlagsPreserved = true, packages = report
    }, Formatting.Indented));
    Console.WriteLine($"Staged and re-opened {(placementPreview ? 10 : 9)} package pairs; no live paid Camp services enabled.");
} catch {
    Directory.Delete(output, true);
    throw;
}
static bool Inside(string path, string root) => path.Equals(root, StringComparison.OrdinalIgnoreCase)
    || path.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
record Spec(string Folder, string Original, string CloneName, string? Definition, string? MerchantType);
