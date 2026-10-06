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
            fn.SuperIndex = fn.SuperStruct = fn.TemplateIndex = new FPackageIndex(0);
            fn.SerialOffset = fn.SerialSize = 0; Clear(fn); fn.Children = Array.Empty<FPackageIndex>();
            fn.FunctionFlags = EFunctionFlags.FUNC_Public | EFunctionFlags.FUNC_BlueprintCallable
                | EFunctionFlags.FUNC_BlueprintPure | EFunctionFlags.FUNC_HasOutParms | EFunctionFlags.FUNC_HasDefaults;
            asset.Exports.Add(fn); fn.CreateBeforeCreateDependencies.Add(Index(owner));
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
        foreach (var spec in Specs) {
            var owner = document.RootElement.GetProperty("Classes").EnumerateArray().Single(c => c.GetProperty("Name").GetString() == spec.Owner);
            var field = owner.GetProperty("Fields").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == "Item");
            if (field.GetProperty("Type").GetString() != (spec.Struct ? "StructProperty" : "ObjectProperty")
                || field.GetProperty("Target").GetString() != "/Script/Dungeons." + spec.Result
                || field.GetProperty("Size").GetInt32() != spec.Size || field.GetProperty("ArrayDim").GetInt32() != 1)
                throw new InvalidDataException("Captured item field contract changed.");
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
    }
    static void Clear(Export e) { e.SerializationBeforeSerializationDependencies.Clear(); e.SerializationBeforeCreateDependencies.Clear();
        e.CreateBeforeSerializationDependencies.Clear(); e.CreateBeforeCreateDependencies.Clear(); }
}
