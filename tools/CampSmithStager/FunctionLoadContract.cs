using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.UnrealTypes;
using UAssetAPI.FieldTypes;

// Event-driven package loading must create a function only after its native
// class/archetype are serialized. Match the supplied original Function exports.
static class FunctionLoadContract
{
    internal static void Add(UAsset asset, FunctionExport function)
    {
        if (function.TemplateIndex.IsNull()) throw new InvalidDataException("Original Function archetype missing.");
        function.SerializationBeforeCreateDependencies.Add(function.ClassIndex);
        function.SerializationBeforeCreateDependencies.Add(function.TemplateIndex);
        Validate(asset, function);
    }
    internal static void Validate(UAsset asset, FunctionExport function)
    {
        if (!function.ClassIndex.IsImport() || !function.TemplateIndex.IsImport())
            throw new InvalidDataException("Generated Function native class/archetype must be imported.");
        var type = function.ClassIndex.ToImport(asset);
        var template = function.TemplateIndex.ToImport(asset);
        if (type.ClassName.ToString() != "Class" || type.ObjectName.ToString() != "Function"
            || template.ClassName.ToString() != "Function" || template.ObjectName.ToString() != "Default__Function"
            || type.OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/CoreUObject"
            || template.OuterIndex.Index != type.OuterIndex.Index
            || !function.SerializationBeforeCreateDependencies.Any(p => p.Index == function.ClassIndex.Index)
            || !function.SerializationBeforeCreateDependencies.Any(p => p.Index == function.TemplateIndex.Index)
            || !function.CreateBeforeCreateDependencies.Any(p => p.Index == function.OuterIndex.Index))
            throw new InvalidDataException("Generated Function lacks required event-driven creation prerequisites.");
    }
    internal static void ValidateOwned(UAsset asset)
    {
        foreach (var function in asset.Exports.OfType<FunctionExport>().Where(f => f.ObjectName.ToString().StartsWith("Rebalance", StringComparison.Ordinal)))
        {
            Validate(asset, function);
            bool locals = false, outputs = false; int returns = 0;
            foreach (var child in function.Children) {
                if (!child.IsExport() || child.ToExport(asset) is not PropertyExport field)
                    throw new InvalidDataException("Generated Function field must be an owned property.");
                var p = field.Property;
                if (p is UBoolProperty b && (b.ElementSize != 1 || !b.NativeBool))
                    throw new InvalidDataException("Generated Boolean must serialize a one-byte native size.");
                if (p.PropertyFlags.HasFlag(EPropertyFlags.CPF_Parm)) {
                    if (locals) throw new InvalidDataException("Generated parameter follows a local.");
                    outputs |= p.PropertyFlags.HasFlag(EPropertyFlags.CPF_OutParm);
                    if (p.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm)) returns++;
                } else locals = true;
            }
            if (returns > 1 || outputs && !function.FunctionFlags.HasFlag(EFunctionFlags.FUNC_HasOutParms)
                || locals && !function.FunctionFlags.HasFlag(EFunctionFlags.FUNC_HasDefaults))
                throw new InvalidDataException("Generated Function signature/local initialization contract invalid.");
        }
    }
    internal static void SelfTest(string source)
    {
        Action<UAsset, FunctionExport>[] breakLoad = {
            (a, f) => f.TemplateIndex = new FPackageIndex(0),
            (a, f) => f.SerializationBeforeCreateDependencies.RemoveAll(p => p.Index == f.ClassIndex.Index),
            (a, f) => f.SerializationBeforeCreateDependencies.RemoveAll(p => p.Index == f.TemplateIndex.Index),
            (a, f) => f.CreateBeforeCreateDependencies.Clear()
        };
        foreach (var mutation in breakLoad) {
            var asset = CampPlacement.Prepare(source);
            var function = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "RebalanceSpawnCampSmiths");
            mutation(asset, function);
            bool rejected = false;
            try { ValidateOwned(asset); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Invalid Function creation contract accepted.");
        }
        Console.WriteLine("Four Function load-contract rejection checks passed.");
    }
}
