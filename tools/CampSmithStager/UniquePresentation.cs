using System.Reflection;
using System.Text.Json;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.FieldTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.UnrealTypes;

// Graph construction follows the same-owner QoL Graph.cs pattern, reused at
// the user's explicit request. Native call contracts come from the successful
// 2026-10-06 retail capture, not from guessed SDK signatures.
static class UniquePresentation
{
    static readonly (string Wrapper, string Native, string Type, int Size, string? Target)[] Specs = {
        ("RebalanceUniqueName", "GetNameForItemType", "TextProperty", 24, null),
        ("RebalanceUniqueDescription", "GetDescriptionForItemType", "TextProperty", 24, null),
        ("RebalanceUniqueIcon", "GetIconTextureForItemType", "ObjectProperty", 8, "/Script/Engine.Texture2D")
    };

    public static void Add(UAsset asset)
    {
        VerifyCapturedContracts();
        var owner = asset.Exports.OfType<ClassExport>().Single();
        FPackageIndex Index(Export e) => FPackageIndex.FromExport(asset.Exports.IndexOf(e));
        FPackageIndex Import(string type, string name, FPackageIndex outer) {
            var found = asset.Imports.FindIndex(i => i.ClassPackage.ToString() == "/Script/CoreUObject"
                && i.ClassName.ToString() == type && i.ObjectName.ToString() == name && i.OuterIndex.Index == outer.Index);
            return found >= 0 ? FPackageIndex.FromImport(found)
                : asset.AddImport(new Import("/Script/CoreUObject", type, outer, name, false, asset));
        }
        FPackageIndex Package(string name) => Import("Package", name, new FPackageIndex(0));
        FPackageIndex Class(string package, string name) => Import("Class", name, Package(package));
        var library = Class("/Script/Dungeons", "ItemFunctionLibrary");
        var itemId = Import("ScriptStruct", "SerializableItemId", Package("/Script/Dungeons"));
        foreach (var spec in Specs) {
            if (asset.Exports.Any(e => e.ObjectName.ToString() == spec.Wrapper)) throw new InvalidDataException("Presentation already added.");
            var fn = (FunctionExport)asset.Exports.OfType<FunctionExport>().First(f => f.Children.Length == 0).Clone();
            fn.ObjectName = new FName(asset, spec.Wrapper); fn.OuterIndex = Index(owner);
            fn.SuperIndex = fn.SuperStruct = fn.TemplateIndex = new FPackageIndex(0);
            fn.SerialOffset = fn.SerialSize = 0; ClearDependencies(fn);
            fn.Children = Array.Empty<FPackageIndex>();
            fn.FunctionFlags = EFunctionFlags.FUNC_Public | EFunctionFlags.FUNC_BlueprintCallable
                | EFunctionFlags.FUNC_BlueprintPure | EFunctionFlags.FUNC_HasOutParms;
            asset.Exports.Add(fn);
            fn.CreateBeforeCreateDependencies.Add(Index(owner));
            owner.Children = owner.Children.Append(Index(fn)).ToArray(); owner.FuncMap.Add(fn.ObjectName, Index(fn));
            owner.SerializationBeforeSerializationDependencies.Add(Index(fn));
            PropertyExport Field(string name, string type, UProperty property, EPropertyFlags flags) {
                var p = (PropertyExport)asset.Exports.OfType<PropertyExport>().First(p => p.Property is UObjectProperty).Clone();
                p.ObjectName = new FName(asset, name); p.OuterIndex = Index(fn);
                p.ClassIndex = Class("/Script/CoreUObject", type);
                p.SuperIndex = p.TemplateIndex = new FPackageIndex(0); p.SerialOffset = p.SerialSize = 0;
                ClearDependencies(p); property.ArrayDim = p.Property.ArrayDim; property.PropertyFlags = flags;
                property.RepNotifyFunc = new FName(asset, "None");
                p.Property = property;
                var cls = p.ClassIndex.ToImport(asset);
                var archetypeName = "Default__" + type;
                var archetype = asset.Imports.FindIndex(i => i.ClassPackage.ToString() == "/Script/CoreUObject"
                    && i.ClassName.ToString() == type && i.ObjectName.ToString() == archetypeName && i.OuterIndex.Index == cls.OuterIndex.Index);
                p.TemplateIndex = archetype >= 0 ? FPackageIndex.FromImport(archetype)
                    : asset.AddImport(new Import("/Script/CoreUObject", type, cls.OuterIndex, archetypeName, false, asset));
                p.SerializationBeforeCreateDependencies.Add(p.ClassIndex);
                p.SerializationBeforeCreateDependencies.Add(p.TemplateIndex);
                p.CreateBeforeCreateDependencies.Add(Index(fn));
                if (property is UStructProperty s) p.SerializationBeforeSerializationDependencies.Add(s.Struct);
                if (property is UObjectProperty o) p.CreateBeforeSerializationDependencies.Add(o.PropertyClass);
                asset.Exports.Add(p); fn.Children = fn.Children.Append(Index(p)).ToArray();
                fn.SerializationBeforeSerializationDependencies.Add(Index(p)); return p;
            }
            var input = Field("Type", "StructProperty", new UStructProperty { Struct = itemId }, EPropertyFlags.CPF_Parm);
            UProperty resultProperty = spec.Type == "TextProperty" ? new UTextProperty()
                : new UObjectProperty { PropertyClass = Class("/Script/Engine", "Texture2D") };
            Field("ReturnValue", spec.Type, resultProperty,
                EPropertyFlags.CPF_Parm | EPropertyFlags.CPF_OutParm | EPropertyFlags.CPF_ReturnParm);
            fn.ScriptBytecode = new KismetExpression[] {
                new EX_Return { ReturnExpression = new EX_CallMath {
                    StackNode = Import("Function", spec.Native, library),
                    Parameters = new KismetExpression[] { new EX_LocalVariable { Variable = new KismetPropertyPointer(Index(input)) } }
                } }, new EX_EndOfScript()
            };
            fn.ScriptBytecodeRaw = null;
            fn.CreateBeforeSerializationDependencies.Add(((EX_CallMath)((EX_Return)fn.ScriptBytecode[0]).ReturnExpression).StackNode);
            fn.CreateBeforeSerializationDependencies.Add(Index(input));
            using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, asset);
            fn.ScriptBytecodeSize = fn.ScriptBytecode.Sum(e => ExpressionSerializer.WriteExpression(e, writer));
        }
        asset.DependsMap = asset.Exports.Select(e => e.SerializationBeforeSerializationDependencies
            .Concat(e.CreateBeforeSerializationDependencies).Concat(e.SerializationBeforeCreateDependencies)
            .Concat(e.CreateBeforeCreateDependencies).Select(p => p.Index).Distinct().ToArray()).ToList();
        Validate(asset);
    }

    public static void Validate(UAsset asset)
    {
        var owner = asset.Exports.OfType<ClassExport>().Single();
        foreach (var spec in Specs) {
            var fn = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == spec.Wrapper);
            var index = FPackageIndex.FromExport(asset.Exports.IndexOf(fn));
            if (fn.OuterIndex.Index != FPackageIndex.FromExport(asset.Exports.IndexOf(owner)).Index
                || !owner.Children.Any(c => c.Index == index.Index) || owner.FuncMap[fn.ObjectName].Index != index.Index
                || !fn.FunctionFlags.HasFlag(EFunctionFlags.FUNC_HasOutParms) || fn.Children.Length != 2)
                throw new InvalidDataException("Invalid presentation function ownership/signature.");
            var input = (PropertyExport)fn.Children[0].ToExport(asset);
            var result = (PropertyExport)fn.Children[1].ToExport(asset);
            if (input.Property is not UStructProperty structure || input.ObjectName.ToString() != "Type"
                || input.Property.PropertyFlags != EPropertyFlags.CPF_Parm
                || structure.Struct.ToImport(asset).ObjectName.ToString() != "SerializableItemId"
                || structure.Struct.ToImport(asset).OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/Dungeons"
                || result.ObjectName.ToString() != "ReturnValue"
                || result.Property.PropertyFlags != (EPropertyFlags.CPF_Parm | EPropertyFlags.CPF_OutParm | EPropertyFlags.CPF_ReturnParm)
                || (spec.Type == "TextProperty" ? result.Property is not UTextProperty : result.Property is not UObjectProperty))
                throw new InvalidDataException("Invalid presentation parameter types.");
            if (result.Property is UObjectProperty icon && (icon.PropertyClass.ToImport(asset).ObjectName.ToString() != "Texture2D"
                || icon.PropertyClass.ToImport(asset).OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/Engine"))
                throw new InvalidDataException("Invalid native icon class.");
            if (fn.ScriptBytecode.Length != 2 || fn.ScriptBytecode[0] is not EX_Return ret
                || ret.ReturnExpression is not EX_CallMath call || call.Parameters.Length != 1
                || call.Parameters[0] is not EX_LocalVariable variable || variable.Variable.Old.Index != fn.Children[0].Index
                || fn.ScriptBytecode[1] is not EX_EndOfScript)
                throw new InvalidDataException("Invalid presentation call graph.");
            var imported = call.StackNode.ToImport(asset);
            var declared = imported.OuterIndex.ToImport(asset);
            if (imported.ClassName.ToString() != "Function" || imported.ObjectName.ToString() != spec.Native
                || declared.ObjectName.ToString() != "ItemFunctionLibrary"
                || declared.OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/Dungeons")
                throw new InvalidDataException("Wrong native presentation declaring owner.");
            if (!fn.CreateBeforeSerializationDependencies.Any(p => p.Index == call.StackNode.Index)
                || !fn.SerializationBeforeSerializationDependencies.Any(p => p.Index == fn.Children[0].Index)
                || !fn.SerializationBeforeSerializationDependencies.Any(p => p.Index == fn.Children[1].Index)
                || input.TemplateIndex.IsNull() || result.TemplateIndex.IsNull())
                throw new InvalidDataException("Missing native presentation preload dependencies.");
        }
    }

    public static void SelfTest(string source)
    {
        void Reject(Action<UAsset> mutation) {
            var asset = new UAsset(source, EngineVersion.VER_UE4_22); Add(asset); mutation(asset);
            try { Validate(asset); } catch (InvalidDataException) { return; }
            throw new InvalidDataException("Presentation validator accepted a broken graph.");
        }
        FunctionExport First(UAsset a) => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Specs[0].Wrapper);
        Reject(a => First(a).FunctionFlags &= ~EFunctionFlags.FUNC_HasOutParms);
        Reject(a => First(a).OuterIndex = new FPackageIndex(0));
        Reject(a => ((EX_CallMath)((EX_Return)First(a).ScriptBytecode[0]).ReturnExpression).Parameters = Array.Empty<KismetExpression>());
        Reject(a => ((EX_LocalVariable)((EX_CallMath)((EX_Return)First(a).ScriptBytecode[0]).ReturnExpression).Parameters[0]).Variable = new KismetPropertyPointer(First(a).Children[1]));
        Reject(a => { var call = (EX_CallMath)((EX_Return)First(a).ScriptBytecode[0]).ReturnExpression;
            call.StackNode.ToImport(a).OuterIndex = ((UStructProperty)((PropertyExport)First(a).Children[0].ToExport(a)).Property).Struct; });
        Reject(a => First(a).CreateBeforeSerializationDependencies.Clear());
        var duplicate = new UAsset(source, EngineVersion.VER_UE4_22); Add(duplicate);
        try { Add(duplicate); } catch (InvalidDataException) {
            Console.WriteLine("Seven presentation graph rejection checks passed; no gameplay calls or asset writes."); return;
        }
        throw new InvalidDataException("Duplicate presentation patch accepted.");
    }

    static void VerifyCapturedContracts()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NativeUpgradeContracts.json")
            ?? throw new InvalidDataException("Missing captured native contracts.");
        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;
        if (!root.GetProperty("sourceCaptureCompleted").GetBoolean() || !root.GetProperty("controlContractsPassed").GetBoolean())
            throw new InvalidDataException("Capture not validated.");
        var library = root.GetProperty("Classes").EnumerateArray().Single(c => c.GetProperty("Name").GetString() == "ItemFunctionLibrary");
        foreach (var spec in Specs) {
            var fn = library.GetProperty("Functions").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == spec.Native);
            var parameters = fn.GetProperty("Parameters").EnumerateArray().ToArray();
            var input = parameters[0]; var result = parameters[1];
            if (parameters.Length != 2 || (fn.GetProperty("Flags").GetInt32() & 0x2400) != 0x2400
                || fn.GetProperty("ParameterSize").GetInt32() != (spec.Type == "TextProperty" ? 48 : 32)
                || fn.GetProperty("ReturnOffset").GetInt32() != 24
                || input.GetProperty("Name").GetString() != "Type" || input.GetProperty("Type").GetString() != "StructProperty"
                || input.GetProperty("Target").GetString() != "/Script/Dungeons.SerializableItemId"
                || input.GetProperty("Size").GetInt32() != 20 || input.GetProperty("Offset").GetInt32() != 0
                || result.GetProperty("Name").GetString() != "ReturnValue" || result.GetProperty("Type").GetString() != spec.Type
                || result.GetProperty("Size").GetInt32() != spec.Size || result.GetProperty("Offset").GetInt32() != 24
                || result.GetProperty("Target").GetString() != spec.Target)
                throw new InvalidDataException("Native presentation contract changed: " + spec.Native);
        }
    }

    static void ClearDependencies(Export export) {
        export.SerializationBeforeSerializationDependencies.Clear(); export.SerializationBeforeCreateDependencies.Clear();
        export.CreateBeforeSerializationDependencies.Clear(); export.CreateBeforeCreateDependencies.Clear();
    }
}
