using System.Reflection;
using System.Text.Json;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.FieldTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.UnrealTypes;

// Read the existing physical slot/item and complete native item record. No
// reconstruction from a display name, new item creation, payment or mutation.
static class SelectedItemReaders
{
    static readonly (string Name, string Owner, string Result, bool Struct, int Size)[] Specs = {
        ("RebalanceSelectedSlotItem", "InventoryItemSlot", "InventoryItem", false, 8),
        ("RebalanceSelectedItemData", "InventoryItem", "InventoryItemData", true, 120)
    };

    public static void Add(UAsset asset)
    {
        VerifyContracts();
        var owner = asset.Exports.OfType<ClassExport>().Single();
        FPackageIndex Index(Export e) => FPackageIndex.FromExport(asset.Exports.IndexOf(e));
        FPackageIndex Import(string type, string name, FPackageIndex outer) {
            var existing = asset.Imports.FindIndex(i => i.ClassPackage.ToString() == "/Script/CoreUObject"
                && i.ClassName.ToString() == type && i.ObjectName.ToString() == name && i.OuterIndex.Index == outer.Index);
            return existing >= 0 ? FPackageIndex.FromImport(existing)
                : asset.AddImport(new Import("/Script/CoreUObject", type, outer, name, false, asset));
        }
        FPackageIndex Package(string name) => Import("Package", name, new FPackageIndex(0));
        FPackageIndex Class(string package, string name) => Import("Class", name, Package(package));
        foreach (var spec in Specs) {
            if (asset.Exports.Any(e => e.ObjectName.ToString() == spec.Name)) throw new InvalidDataException("Selected-item readers already added.");
            var fn = (FunctionExport)asset.Exports.OfType<FunctionExport>().First(f => f.Children.Length == 0).Clone();
            fn.ObjectName = new FName(asset, spec.Name); fn.OuterIndex = Index(owner);
            fn.SuperIndex = fn.SuperStruct = new FPackageIndex(0);
            fn.SerialOffset = fn.SerialSize = 0; Clear(fn); fn.Children = Array.Empty<FPackageIndex>();
            fn.FunctionFlags = EFunctionFlags.FUNC_Public | EFunctionFlags.FUNC_BlueprintCallable
                | EFunctionFlags.FUNC_BlueprintPure | EFunctionFlags.FUNC_HasOutParms | EFunctionFlags.FUNC_HasDefaults;
            asset.Exports.Add(fn); fn.CreateBeforeCreateDependencies.Add(Index(owner));
            FunctionLoadContract.Add(asset, fn);
            owner.Children = owner.Children.Append(Index(fn)).ToArray(); owner.FuncMap.Add(fn.ObjectName, Index(fn));
            owner.SerializationBeforeSerializationDependencies.Add(Index(fn));
            PropertyExport Field(string name, string type, UProperty property, EPropertyFlags flags) {
                var field = (PropertyExport)asset.Exports.OfType<PropertyExport>().First(p => p.Property is UObjectProperty).Clone();
                field.ObjectName = new FName(asset, name); field.OuterIndex = Index(fn); field.ClassIndex = Class("/Script/CoreUObject", type);
                field.SuperIndex = field.TemplateIndex = new FPackageIndex(0); field.SerialOffset = field.SerialSize = 0; Clear(field);
                property.ArrayDim = field.Property.ArrayDim; property.PropertyFlags = flags; property.RepNotifyFunc = new FName(asset, "None");
                field.Property = property;
                var cls = field.ClassIndex.ToImport(asset);
                var archetype = asset.Imports.FindIndex(i => i.ClassPackage.ToString() == "/Script/CoreUObject"
                    && i.ClassName.ToString() == type && i.ObjectName.ToString() == "Default__" + type && i.OuterIndex.Index == cls.OuterIndex.Index);
                field.TemplateIndex = archetype >= 0 ? FPackageIndex.FromImport(archetype)
                    : asset.AddImport(new Import("/Script/CoreUObject", type, cls.OuterIndex, "Default__" + type, false, asset));
                field.CreateBeforeCreateDependencies.Add(Index(fn)); field.SerializationBeforeCreateDependencies.Add(field.ClassIndex);
                field.SerializationBeforeCreateDependencies.Add(field.TemplateIndex);
                if (property is UObjectProperty o) field.CreateBeforeSerializationDependencies.Add(o.PropertyClass);
                if (property is UStructProperty s) field.SerializationBeforeSerializationDependencies.Add(s.Struct);
                asset.Exports.Add(field); fn.Children = fn.Children.Append(Index(field)).ToArray();
                fn.SerializationBeforeSerializationDependencies.Add(Index(field)); return field;
            }
            var nativeOwner = Class("/Script/Dungeons", spec.Owner);
            var input = Field("Selected", "ObjectProperty", new UObjectProperty { PropertyClass = nativeOwner }, EPropertyFlags.CPF_Parm);
            var nativeResult = spec.Struct ? Import("ScriptStruct", spec.Result, Package("/Script/Dungeons")) : Class("/Script/Dungeons", spec.Result);
            var result = Field("ReturnValue", spec.Struct ? "StructProperty" : "ObjectProperty",
                spec.Struct ? new UStructProperty { Struct = nativeResult } : new UObjectProperty { PropertyClass = nativeResult },
                EPropertyFlags.CPF_Parm | EPropertyFlags.CPF_OutParm | EPropertyFlags.CPF_ReturnParm);
            var member = Import(spec.Struct ? "StructProperty" : "ObjectProperty", "Item", nativeOwner);
            var read = new EX_InstanceVariable { Variable = new KismetPropertyPointer(member) };
            using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, asset);
            var context = new EX_Context { ObjectExpression = new EX_LocalVariable { Variable = new KismetPropertyPointer(Index(input)) },
                ContextExpression = read, Offset = (uint)ExpressionSerializer.WriteExpression(read, writer),
                RValuePointer = new KismetPropertyPointer(Index(result)) };
            fn.ScriptBytecode = new KismetExpression[] { new EX_Return { ReturnExpression = context }, new EX_EndOfScript() };
            fn.ScriptBytecodeRaw = null; fn.ScriptBytecodeSize = fn.ScriptBytecode.Sum(e => ExpressionSerializer.WriteExpression(e, writer));
            fn.CreateBeforeSerializationDependencies.Add(member); fn.CreateBeforeSerializationDependencies.Add(Index(input));
            fn.CreateBeforeSerializationDependencies.Add(Index(result));
        }
        asset.DependsMap = asset.Exports.Select(e => e.SerializationBeforeSerializationDependencies.Concat(e.CreateBeforeSerializationDependencies)
            .Concat(e.SerializationBeforeCreateDependencies).Concat(e.CreateBeforeCreateDependencies).Select(i => i.Index).Distinct().ToArray()).ToList();
        Validate(asset);
    }

    public static void Validate(UAsset asset)
    {
        FunctionLoadContract.ValidateOwned(asset);
        var owner = asset.Exports.OfType<ClassExport>().Single();
        foreach (var spec in Specs) {
            var fn = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == spec.Name);
            var index = FPackageIndex.FromExport(asset.Exports.IndexOf(fn));
            if (fn.Children.Length != 2 || fn.OuterIndex.Index != FPackageIndex.FromExport(asset.Exports.IndexOf(owner)).Index
                || owner.FuncMap[fn.ObjectName].Index != index.Index || !owner.Children.Any(p => p.Index == index.Index)
                || !fn.FunctionFlags.HasFlag(EFunctionFlags.FUNC_HasDefaults) || !fn.FunctionFlags.HasFlag(EFunctionFlags.FUNC_HasOutParms))
                throw new InvalidDataException("Invalid selected-item function ownership/layout.");
            var input = (PropertyExport)fn.Children[0].ToExport(asset); var result = (PropertyExport)fn.Children[1].ToExport(asset);
            if (input.Property is not UObjectProperty selected || input.Property.PropertyFlags != EPropertyFlags.CPF_Parm
                || selected.PropertyClass.ToImport(asset).ObjectName.ToString() != spec.Owner
                || selected.PropertyClass.ToImport(asset).OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/Dungeons"
                || result.Property.PropertyFlags != (EPropertyFlags.CPF_Parm | EPropertyFlags.CPF_OutParm | EPropertyFlags.CPF_ReturnParm))
                throw new InvalidDataException("Invalid selected-item signature.");
            var resultType = result.Property switch { UStructProperty s when spec.Struct => s.Struct, UObjectProperty o when !spec.Struct => o.PropertyClass,
                _ => throw new InvalidDataException("Invalid selected-item return type.") };
            if (resultType.ToImport(asset).ObjectName.ToString() != spec.Result
                || resultType.ToImport(asset).OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/Dungeons")
                throw new InvalidDataException("Wrong native item record type.");
            if (fn.ScriptBytecode.Length != 2 || fn.ScriptBytecode[0] is not EX_Return ret || ret.ReturnExpression is not EX_Context context
                || context.ObjectExpression is not EX_LocalVariable local || local.Variable.Old.Index != fn.Children[0].Index
                || context.RValuePointer.Old.Index != fn.Children[1].Index || context.ContextExpression is not EX_InstanceVariable member
                || fn.ScriptBytecode[1] is not EX_EndOfScript)
                throw new InvalidDataException("Invalid selected-item read graph.");
            var field = member.Variable.Old.ToImport(asset); var declared = field.OuterIndex.ToImport(asset);
            using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, asset);
            if (field.ObjectName.ToString() != "Item" || field.ClassName.ToString() != (spec.Struct ? "StructProperty" : "ObjectProperty")
                || declared.ObjectName.ToString() != spec.Owner || declared.OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/Dungeons"
                || context.Offset != ExpressionSerializer.WriteExpression(context.ContextExpression, writer)
                || !fn.CreateBeforeSerializationDependencies.Any(p => p.Index == member.Variable.Old.Index)
                || result.TemplateIndex.IsNull()) throw new InvalidDataException("Unverified native item field/context/preload.");
        }
    }

    static void VerifyContracts()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NativeUpgradeContracts.json")
            ?? throw new InvalidDataException("Missing selected-item contracts.");
        using var document = JsonDocument.Parse(stream);
        if (!document.RootElement.GetProperty("sourceCaptureCompleted").GetBoolean()
            || !document.RootElement.GetProperty("controlContractsPassed").GetBoolean())
            throw new InvalidDataException("Selected-item capture incomplete.");
        VerifyNativeRecord(document.RootElement);
        foreach (var spec in Specs) {
            var owner = document.RootElement.GetProperty("Classes").EnumerateArray().Single(c => c.GetProperty("Name").GetString() == spec.Owner);
            var field = owner.GetProperty("Fields").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == "Item");
            if (field.GetProperty("Type").GetString() != (spec.Struct ? "StructProperty" : "ObjectProperty")
                || field.GetProperty("Target").GetString() != "/Script/Dungeons." + spec.Result
                || field.GetProperty("Size").GetInt32() != spec.Size || field.GetProperty("ArrayDim").GetInt32() != 1)
                throw new InvalidDataException("Captured item field contract changed.");
        }
    }
    // These checks describe the native record read by our graph. They are not a
    // serializer, item constructor, or proof of native transaction persistence.
    internal static void VerifyNativeRecord(JsonElement root)
    {
        void Field(string owner, string name, string type, string? target, int size, int offset, string? innerTarget = null) {
            var declaration = root.GetProperty("Classes").EnumerateArray().Single(c => c.GetProperty("Name").GetString() == owner);
            var field = declaration.GetProperty("Fields").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == name);
            if (field.GetProperty("Type").GetString() != type || field.GetProperty("Target").GetString() != target
                || field.GetProperty("ArrayDim").GetInt32() != 1 || field.GetProperty("Size").GetInt32() != size
                || field.GetProperty("Offset").GetInt32() != offset
                || (innerTarget != null && (field.GetProperty("Inner").GetProperty("Type").GetString() != "StructProperty"
                    || field.GetProperty("Inner").GetProperty("Target").GetString() != innerTarget)))
                throw new InvalidDataException("Native item record contract changed: " + owner + "." + name);
        }
        Field("SerializableItemId", "SerializedId", "NameProperty", null, 8, 12);
        Field("InventoryItemData", "ItemId", "StructProperty", "/Script/Dungeons.SerializableItemId", 20, 0);
        Field("InventoryItemData", "ItemPower", "FloatProperty", null, 4, 20);
        Field("InventoryItemData", "Enchantments", "ArrayProperty", null, 16, 24, "/Script/Dungeons.EnchantmentData");
        Field("InventoryItemData", "ArmorProperties", "ArrayProperty", null, 16, 40, "/Script/Dungeons.ArmorPropertyData");
        Field("InventoryItemData", "Rarity", "EnumProperty", "/Script/Dungeons.EItemRarity", 1, 56);
        Field("InventoryItemData", "bIsUpgraded", "BoolProperty", null, 1, 57);
        Field("InventoryItemData", "bIsGifted", "BoolProperty", null, 1, 58);
        Field("InventoryItemData", "bIsModified", "BoolProperty", null, 1, 59);
        Field("InventoryItemData", "timesModified", "IntProperty", null, 4, 60);
        Field("InventoryItemData", "bHasNetherite", "BoolProperty", null, 1, 96);
        Field("InventoryItemData", "NetheriteEnchantData", "StructProperty", "/Script/Dungeons.EnchantmentData", 16, 100);
        Enum("EItemRarity", new[] { ("Common", 0L), ("Rare", 1L), ("Unique", 2L) });
        Enum("EEnchantmentSource", new[] { ("Unset", 0L), ("Permanent", 1L), ("Generated", 2L), ("Netherite", 3L), ("Dust", 4L) });
        Enum("EEnchantmentCategory", new[] { ("Unset", 0L), ("Melee", 1L), ("Ranged", 2L), ("Aoe", 4L), ("Armor", 8L), ("Permanent", 64L) });
        void Enum(string name, (string Symbol, long Value)[] expected) {
            var values = root.GetProperty("Enums").EnumerateArray().Single(e => e.GetProperty("Name").GetString() == "/Script/Dungeons." + name).GetProperty("Values");
            foreach (var (symbol, value) in expected) {
                var entry = values.EnumerateArray().Single(e => e.GetProperty("Name").GetString() == name + "::" + symbol);
                if (entry.GetProperty("Value").GetInt64() != value || entry.GetProperty("NameNumber").GetInt32() != 0)
                    throw new InvalidDataException("Native enum value changed: " + name + "::" + symbol);
            }
        }
    }
    public static void SelfTest(string source)
    {
        FunctionExport Function(UAsset a) => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Specs[1].Name);
        void Reject(Action<UAsset> mutation) {
            var asset = new UAsset(source, EngineVersion.VER_UE4_22); Add(asset); mutation(asset);
            try { Validate(asset); } catch (InvalidDataException) { return; }
            throw new InvalidDataException("Broken selected-item graph accepted.");
        }
        Reject(a => Function(a).FunctionFlags &= ~EFunctionFlags.FUNC_HasDefaults);
        Reject(a => ((EX_Context)((EX_Return)Function(a).ScriptBytecode[0]).ReturnExpression).Offset++);
        Reject(a => ((EX_Context)((EX_Return)Function(a).ScriptBytecode[0]).ReturnExpression).RValuePointer = new KismetPropertyPointer(Function(a).Children[0]));
        Reject(a => { var c = (EX_Context)((EX_Return)Function(a).ScriptBytecode[0]).ReturnExpression;
            var p = ((EX_InstanceVariable)c.ContextExpression).Variable.Old.ToImport(a); p.ObjectName = new FName(a, "Meta"); });
        Console.WriteLine("Four selected-item graph rejection checks passed; no gameplay calls or asset writes.");
        void RejectContract(Action<System.Text.Json.Nodes.JsonNode> mutation) {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NativeUpgradeContracts.json")!;
            var node = System.Text.Json.Nodes.JsonNode.Parse(stream)!; mutation(node);
            using var document = JsonDocument.Parse(node.ToJsonString());
            try { VerifyNativeRecord(document.RootElement); }
            catch (InvalidDataException) { return; }
            catch (InvalidOperationException) { return; }
            throw new Exception("Changed native record contract accepted.");
        }
        System.Text.Json.Nodes.JsonNode Field(System.Text.Json.Nodes.JsonNode n, string owner, string name)
            => n["Classes"]!.AsArray().Single(c => (string?)c!["Name"] == owner)!["Fields"]!.AsArray().Single(f => (string?)f!["Name"] == name)!;
        RejectContract(n => Field(n, "SerializableItemId", "SerializedId")["Offset"] = 0);
        RejectContract(n => Field(n, "InventoryItemData", "Enchantments")["Inner"]!["Target"] = "/Script/Dungeons.ArmorPropertyData");
        RejectContract(n => n["Enums"]!.AsArray().Single(e => (string?)e!["Name"] == "/Script/Dungeons.EEnchantmentCategory")!["Values"]!
            .AsArray().Single(v => (string?)v!["Name"] == "EEnchantmentCategory::Armor")!["Value"] = 4);
        RejectContract(n => Field(n, "InventoryItemData", "NetheriteEnchantData")["Size"] = 12);
        Console.WriteLine("Four native record contract rejection checks passed.");
    }
    static void Clear(Export e) { e.SerializationBeforeSerializationDependencies.Clear(); e.SerializationBeforeCreateDependencies.Clear();
        e.CreateBeforeSerializationDependencies.Clear(); e.CreateBeforeCreateDependencies.Clear(); }
}
