using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;

// Experimental UI access without enabling the stock free Tower action.
// Only the owned content copies are changed; original Tower widgets stay intact.
static class ReadOnlyMerchantActions
{
    internal static int Block(UAsset asset)
    {
        int count = 0;
        foreach (var widget in asset.Exports.OfType<NormalExport>()) {
            var priorities = widget.Data.OfType<ArrayPropertyData>().SingleOrDefault(p => p.Name.ToString() == "TransactionClassPrio");
            if (priorities == null) continue;
            if (priorities.ArrayType.ToString() != "ObjectProperty" || priorities.Value.Length != 1)
                throw new InvalidDataException("Unexpected Tower action configuration.");
            priorities.Value = Array.Empty<PropertyData>();
            var enabled = widget.Data.OfType<BoolPropertyData>().SingleOrDefault(p => p.Name.ToString() == "bIsEnabled");
            if (enabled == null) widget.Data.Add(new BoolPropertyData(new FName(asset, "bIsEnabled")) { Value = false });
            else enabled.Value = false;
            count++;
        }
        if (count != ExpectedCount(asset)) throw new InvalidDataException("Expected the live and cooked-template Tower action bindings.");
        Validate(asset); return count;
    }
    internal static void Validate(UAsset asset)
    {
        var widgets = asset.Exports.OfType<NormalExport>().Where(w => w.Data.Any(p => p.Name.ToString() == "TransactionClassPrio")).ToArray();
        if (widgets.Length != ExpectedCount(asset) || widgets.Any(w => w.Data.OfType<ArrayPropertyData>().Single(p => p.Name.ToString() == "TransactionClassPrio").Value.Length != 0
            || w.Data.OfType<BoolPropertyData>().Single(p => p.Name.ToString() == "bIsEnabled").Value))
            throw new InvalidDataException("Read-only merchant action bindings must be empty and disabled.");
    }
    static int ExpectedCount(UAsset asset) => asset.Exports.OfType<ClassExport>().Single().ObjectName.ToString() is var name
        && (name.Contains("Powersmith") || name.Contains("TowerMerchantBlacksmith")) ? 7 : 2;
    internal static void SelfTest(string source)
    {
        foreach (var mode in new[] { 0, 1 }) {
            var asset = new UAsset(Path.Combine(source, "Dungeons/Content/Content_Season1/UI/Merchant/UMG_TowerMerchantArtisanContent.uasset"), EngineVersion.VER_UE4_22);
            var widget = asset.Exports.OfType<NormalExport>().First(w => w.Data.Any(p => p.Name.ToString() == "TransactionClassPrio"));
            var original = (ArrayPropertyData)widget.Data.OfType<ArrayPropertyData>().Single(p => p.Name.ToString() == "TransactionClassPrio").Clone();
            Block(asset);
            if (mode == 0) widget.Data.OfType<ArrayPropertyData>().Single(p => p.Name.ToString() == "TransactionClassPrio").Value = original.Value;
            else widget.Data.OfType<BoolPropertyData>().Single(p => p.Name.ToString() == "bIsEnabled").Value = true;
            bool rejected = false; try { Validate(asset); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Unblocked Tower action accepted.");
        }
        Console.WriteLine("Two read-only action rejection checks passed; no upgrades executed.");
    }
}
