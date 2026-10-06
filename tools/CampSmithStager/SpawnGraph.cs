using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.FieldTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.UnrealTypes;

// Adapted from the same-owner QoL graph-building pattern at the user's request.
// Engine/game operations are selected separately in CampPlacement.
sealed class SpawnGraph
{
    internal readonly UAsset Asset;
    internal readonly ClassExport Owner;
    internal readonly FunctionExport Function;
    internal readonly List<KismetExpression> Code = new();
    readonly Dictionary<string, int> labels = new();
    readonly List<(EX_JumpIfNot Jump, string Label)> branches = new();
    internal SpawnGraph(UAsset asset)
    {
        Asset = asset; Owner = asset.Exports.OfType<ClassExport>().Single();
        const string name = "RebalanceSpawnCampSmiths";
        if (asset.Exports.Any(e => e.ObjectName.ToString() == name)) throw new InvalidDataException("Placement already added.");
        Function = (FunctionExport)asset.Exports.OfType<FunctionExport>().First(f => f.Children.Length == 0).Clone();
        Function.ObjectName = new FName(asset, name); Function.OuterIndex = Index(Owner);
        Function.SuperIndex = Function.SuperStruct = Function.TemplateIndex = new FPackageIndex(0);
        Function.SerialOffset = Function.SerialSize = 0; Clear(Function);
        Function.Children = Array.Empty<FPackageIndex>();
        Function.FunctionFlags = EFunctionFlags.FUNC_Public | EFunctionFlags.FUNC_BlueprintCallable | EFunctionFlags.FUNC_HasDefaults;
        asset.Exports.Add(Function); Function.CreateBeforeCreateDependencies.Add(Index(Owner));
        Owner.Children = Owner.Children.Append(Index(Function)).ToArray(); Owner.FuncMap.Add(Function.ObjectName, Index(Function));
        Owner.SerializationBeforeSerializationDependencies.Add(Index(Function));
    }
    internal FPackageIndex Index(Export e) => FPackageIndex.FromExport(Asset.Exports.IndexOf(e));
    internal FPackageIndex Import(string type, string name, FPackageIndex outer, string package = "/Script/CoreUObject")
    {
        int i = Asset.Imports.FindIndex(p => p.ClassName.ToString() == type && p.ObjectName.ToString() == name && p.OuterIndex.Index == outer.Index && p.ClassPackage.ToString() == package);
        var index = i >= 0 ? FPackageIndex.FromImport(i) : Asset.AddImport(new Import(package, type, outer, name, false, Asset));
        Function.CreateBeforeSerializationDependencies.Add(index); return index;
    }
    internal FPackageIndex Package(string name) => Import("Package", name, new FPackageIndex(0));
    internal FPackageIndex Class(string package, string name) => Import("Class", name, Package(package));
    internal FPackageIndex Fn(FPackageIndex owner, string name) => Import("Function", name, owner);
    internal PropertyExport Property(string name, string type, UProperty value)
    {
        var field = (PropertyExport)Asset.Exports.OfType<PropertyExport>().First(p => p.Property is UObjectProperty).Clone();
        field.ObjectName = new FName(Asset, name); field.OuterIndex = Index(Function);
        field.ClassIndex = Class("/Script/CoreUObject", type); field.SuperIndex = new FPackageIndex(0);
        field.SerialSize = field.SerialOffset = 0; Clear(field);
        field.TemplateIndex = Import(type, "Default__" + type, Package("/Script/CoreUObject"));
        value.ArrayDim = field.Property.ArrayDim; value.PropertyFlags = EPropertyFlags.CPF_None;
        value.RepNotifyFunc = new FName(Asset, "None"); field.Property = value;
        field.SerializationBeforeCreateDependencies.Add(field.ClassIndex); field.SerializationBeforeCreateDependencies.Add(field.TemplateIndex);
        field.CreateBeforeCreateDependencies.Add(Index(Function));
        if (value is UStructProperty s) field.SerializationBeforeSerializationDependencies.Add(s.Struct);
        if (value is UObjectProperty o) field.CreateBeforeSerializationDependencies.Add(o.PropertyClass);
        Asset.Exports.Add(field); Function.Children = Function.Children.Append(Index(field)).ToArray();
        Function.SerializationBeforeSerializationDependencies.Add(Index(field)); return field;
    }
    internal PropertyExport Object(string name, FPackageIndex type) => Property(name, "ObjectProperty", new UObjectProperty { PropertyClass = type });
    internal PropertyExport Struct(string name, string type) => Property(name, "StructProperty", new UStructProperty { Struct = Import("ScriptStruct", type, Package("/Script/CoreUObject")) });
    internal PropertyExport ActorArray(FPackageIndex actor)
    {
        var field = Property("ExistingSmiths", "ArrayProperty", new UArrayProperty());
        var inner = Object("ExistingSmiths_Inner", actor); inner.OuterIndex = Index(field);
        inner.CreateBeforeCreateDependencies.Clear(); inner.CreateBeforeCreateDependencies.Add(Index(field));
        Function.Children = Function.Children.Where(p => p.Index != Index(inner).Index).ToArray();
        ((UArrayProperty)field.Property).Inner = Index(inner); field.SerializationBeforeSerializationDependencies.Add(Index(inner));
        return field;
    }
    internal KismetExpression L(PropertyExport field) => new EX_LocalVariable { Variable = new KismetPropertyPointer(Index(field)) };
    internal KismetExpression Math(string owner, string name, params KismetExpression[] args) => new EX_CallMath { StackNode = Fn(Class("/Script/Engine", owner), name), Parameters = args };
    internal KismetExpression Context(KismetExpression receiver, KismetExpression expr, PropertyExport? result = null) => new EX_Context {
        ObjectExpression = receiver, ContextExpression = expr, Offset = (uint)Size(expr), RValuePointer = new KismetPropertyPointer(result == null ? new FPackageIndex(0) : Index(result))
    };
    internal void Set(PropertyExport field, KismetExpression value)
    {
        if (value is EX_Context c) c.RValuePointer = new KismetPropertyPointer(Index(field));
        Code.Add(field.Property is UObjectProperty ? new EX_LetObj { VariableExpression = L(field), AssignmentExpression = value }
            : new EX_Let { Value = new KismetPropertyPointer(Index(field)), Variable = L(field), Expression = value });
    }
    internal void Branch(KismetExpression condition, string target)
    {
        var jump = new EX_JumpIfNot { BooleanExpression = condition }; Code.Add(jump); branches.Add((jump, target));
    }
    internal void Label(string name) => labels.Add(name, Code.Count);
    internal int Size(KismetExpression expr) { using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, Asset); return ExpressionSerializer.WriteExpression(expr, writer); }
    internal void Finish()
    {
        Code.Add(new EX_Return { ReturnExpression = new EX_Nothing() }); Code.Add(new EX_EndOfScript());
        uint offset = 0; var offsets = Code.Select(e => { uint value = offset; offset += (uint)Size(e); return value; }).ToArray();
        foreach (var (jump, label) in branches) jump.CodeOffset = offsets[labels[label]];
        Function.ScriptBytecode = Code.ToArray(); Function.ScriptBytecodeRaw = null; Function.ScriptBytecodeSize = (int)offset;
        Asset.DependsMap = Asset.Exports.Select(e => e.SerializationBeforeSerializationDependencies.Concat(e.CreateBeforeSerializationDependencies)
            .Concat(e.SerializationBeforeCreateDependencies).Concat(e.CreateBeforeCreateDependencies).Select(p => p.Index).Distinct().ToArray()).ToList();
    }
    static void Clear(Export e) { e.SerializationBeforeSerializationDependencies.Clear(); e.SerializationBeforeCreateDependencies.Clear(); e.CreateBeforeSerializationDependencies.Clear(); e.CreateBeforeCreateDependencies.Clear(); }
}
