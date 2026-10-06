using System.Reflection;
using System.Text.Json;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;

// Reuse the retail merchant root's existing decision and owning-player graph.
// Only its content dispatch is changed. No transaction/payment graph is added.
static class MerchantScreens
{
    internal const string Folder = "Mods/MinecraftDungeonsRebalance/Camp";
    internal static readonly string[] Services = { "Uniquesmith", "Powersmith", "Gildsmith" };
    internal static string Screen(string service) => "UMG_RebalanceCamp" + service;
    internal static string Content(string service) => Screen(service) + "Content";
    internal static string ClassPath(string name) => "/Game/" + Folder + "/" + name + "." + name + "_C";

    internal static void BindActor(UAsset actor, string service)
    {
        _ = new FName(actor, "SoftObjectProperty");
        var cdo = actor.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString().StartsWith("Default__"));
        if (cdo.Data.Any(p => p.Name.ToString() == "mMerchantWidgetClass"))
            throw new InvalidDataException("Unexpected existing actor screen override.");
        cdo.Data.Add(new SoftObjectPropertyData(new FName(actor, "mMerchantWidgetClass")) {
            Value = new FSoftObjectPath(null!, new FName(actor, ClassPath(Screen(service))), null!)
        });
        ValidateActor(actor, service);
    }
    internal static void ValidateActor(UAsset actor, string service)
    {
        var cdo = actor.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString().StartsWith("Default__"));
        var binding = cdo.Data.OfType<SoftObjectPropertyData>().Single(p => p.Name.ToString() == "mMerchantWidgetClass");
        if (binding.Value.AssetPath.AssetName.ToString() != ClassPath(Screen(service)))
            throw new InvalidDataException("Wrong Camp merchant screen binding.");
    }
    internal static (string Source, string Service, UAsset Asset, Dictionary<string, string> PreservedGraphs, int RelocatedNames)[] Prepare(string source)
    {
        VerifyContracts();
        var path = Path.Combine(source, "Dungeons/Content/UI/Merchant/UMG_Merchant.uasset");
        return Services.Select(service => {
            var asset = new UAsset(path, EngineVersion.VER_UE4_22);
            var original = asset.Exports.OfType<FunctionExport>().Select(f => f.ObjectName.ToString()).ToHashSet();
            var name = Screen(service);
            int relocated = 0;
            for (int i = 0; i < asset.GetNameMapIndexList().Count; i++) {
                var before = asset.GetNameReference(i).Value;
                var after = before switch {
                    "UMG_Merchant" => name,
                    "UMG_Merchant_C" => name + "_C",
                    "Default__UMG_Merchant_C" => "Default__" + name + "_C",
                    "ExecuteUbergraph_UMG_Merchant" => "ExecuteUbergraph_" + name,
                    "/Game/UI/Merchant/UMG_Merchant" => "/Game/" + Folder + "/" + name,
                    _ => before
                };
                if (before != after) { asset.SetNameReference(i, new FString(after)); relocated++; }
            }
            var dispatch = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "GetSoftContentWidget");
            dispatch.ScriptBytecode = new KismetExpression[] {
                new EX_Return { ReturnExpression = new EX_SoftObjectConst { Value = new EX_StringConst { Value = ClassPath(Content(service)) } } },
                new EX_EndOfScript()
            };
            dispatch.ScriptBytecodeRaw = null;
            using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, asset);
            dispatch.ScriptBytecodeSize = dispatch.ScriptBytecode.Sum(e => ExpressionSerializer.WriteExpression(e, writer));
            // Resolve hashes AFTER identity relocation. All functions other than
            // dispatch must retain their entire parsed graph on write/reopen.
            var preserved = asset.Exports.OfType<FunctionExport>().Where(f => f != dispatch)
                .ToDictionary(f => f.ObjectName.ToString(), f => Graph(asset, f));
            if (!original.Contains("OnDecisionToBeMade") || !original.Contains("InstDecisionContent"))
                throw new InvalidDataException("Native decision event graph missing.");
            Validate(asset, service, preserved);
            return (path, service, asset, preserved, relocated);
        }).ToArray();
    }
    internal static void Validate(UAsset asset, string service, Dictionary<string, string> preserved)
    {
        var dispatch = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "GetSoftContentWidget");
        if (asset.Exports.OfType<FunctionExport>().Count() != preserved.Count + 1)
            throw new InvalidDataException("Native merchant function removed or added.");
        if (!dispatch.FunctionFlags.HasFlag(EFunctionFlags.FUNC_Event) || !dispatch.FunctionFlags.HasFlag(EFunctionFlags.FUNC_BlueprintEvent)
            || dispatch.ScriptBytecode.Length != 2 || dispatch.ScriptBytecode[0] is not EX_Return ret
            || ret.ReturnExpression is not EX_SoftObjectConst constant || constant.Value is not EX_StringConst text
            || text.Value != ClassPath(Content(service)) || dispatch.ScriptBytecode[1] is not EX_EndOfScript)
            throw new InvalidDataException("Invalid Camp content dispatch.");
        foreach (var f in asset.Exports.OfType<FunctionExport>().Where(f => f != dispatch))
            if (!preserved.TryGetValue(f.ObjectName.ToString(), out var graph) || graph != Graph(asset, f))
                throw new InvalidDataException("Native merchant decision/input graph changed.");
        if (asset.Imports.Any(i => i.ObjectName.ToString() == "/Game/UI/Merchant/UMG_Merchant"))
            throw new InvalidDataException("Original merchant self-reference remains.");
        // The stock decision widget is retained, not accidentally renamed by
        // prefix replacement of UMG_Merchant into a Rebalance class name.
        if (!asset.Imports.Any(i => i.ObjectName.ToString() == "UMG_MerchantItemDecision_C"))
            throw new InvalidDataException("Stock item decision widget reference missing.");
    }
    static string Graph(UAsset asset, FunctionExport function)
    {
        using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, asset);
        foreach (var expression in function.ScriptBytecode) ExpressionSerializer.WriteExpression(expression, writer);
        return Convert.ToHexString(stream.ToArray());
    }
    internal static void SelfTest(string source)
    {
        void Reject(Action test) {
            try { test(); } catch (InvalidDataException) { return; }
            throw new Exception("Broken merchant screen accepted.");
        }
        Reject(() => {
            var screen = Prepare(source)[0];
            var dispatch = screen.Asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "GetSoftContentWidget");
            ((EX_StringConst)((EX_SoftObjectConst)((EX_Return)dispatch.ScriptBytecode[0]).ReturnExpression).Value).Value = ClassPath(Content("Gildsmith"));
            Validate(screen.Asset, screen.Service, screen.PreservedGraphs);
        });
        Reject(() => {
            var screen = Prepare(source)[0];
            screen.Asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "OnDecisionToBeMade").ScriptBytecode = Array.Empty<KismetExpression>();
            Validate(screen.Asset, screen.Service, screen.PreservedGraphs);
        });
        Reject(() => {
            var actor = new UAsset(Path.Combine(source, "Dungeons/Content/Content_Season1/Decor/Prefab/Merchants/BP_TowerArtisanMerchant.uasset"), EngineVersion.VER_UE4_22);
            BindActor(actor, "Uniquesmith"); ValidateActor(actor, "Powersmith");
        });
        Console.WriteLine("Three merchant screen rejection checks passed; no gameplay calls or writes.");
    }
    static void VerifyContracts()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NativeUpgradeContracts.json")!;
        using var document = JsonDocument.Parse(stream); var root = document.RootElement;
        if (!root.GetProperty("dependencyClosureComplete").GetBoolean()) throw new InvalidDataException("Merchant capture incomplete.");
        var widget = root.GetProperty("Classes").EnumerateArray().Single(c => c.GetProperty("Name").GetString() == "MerchantBaseWidget");
        var decision = widget.GetProperty("Functions").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == "OnDecisionToBeMade");
        var items = decision.GetProperty("Parameters")[0];
        if ((decision.GetProperty("Flags").GetUInt32() & 0x800) == 0 || items.GetProperty("Type").GetString() != "ArrayProperty"
            || items.GetProperty("Inner").GetProperty("Target").GetString() != "/Script/Dungeons.InventoryItemData")
            throw new InvalidDataException("Native decision event contract changed.");
        var actor = root.GetProperty("Classes").EnumerateArray().Single(c => c.GetProperty("Name").GetString() == "MerchantActor");
        var screen = actor.GetProperty("Fields").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == "mMerchantWidgetClass");
        if (screen.GetProperty("Type").GetString() != "SoftClassProperty" || screen.GetProperty("Size").GetInt32() != 40)
            throw new InvalidDataException("Native merchant screen field changed.");
    }
}
