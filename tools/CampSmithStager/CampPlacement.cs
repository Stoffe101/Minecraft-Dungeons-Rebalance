using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.UnrealTypes;
using UAssetAPI.PropertyTypes.Objects;

// Experimental placement build only. Native interactions are disabled; no
// upgrade, currency or inventory function is included in this spawn graph.
static class CampPlacement
{
    internal static UAsset Prepare(string source)
    {
        var path = Path.Combine(source, "Dungeons/Content/Decor/Prefabs/RewardChest/BP_LobbyChest.uasset");
        var asset = new UAsset(path, EngineVersion.VER_UE4_22);
        // This package also carries v2's accepted Camp emerald reward. Preserve
        // that feature when replacing the baseline chest pair in the preview.
        var reward = asset.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString() == "Default__BP_LobbyChest_C")
            .Data.OfType<IntPropertyData>().Single(p => p.Name.ToString() == "EmeraldsReward");
        if (reward.Value != 50) throw new InvalidDataException("Unexpected original Camp emerald reward.");
        reward.Value = 100;
        var graph = new SpawnGraph(asset);
        var actor = graph.Class("/Script/Engine", "Actor");
        var existing = graph.ActorArray(actor);
        var origin = graph.Struct("CampAnchorLocation", "Vector");
        var transform = graph.Struct("SmithTransform", "Transform");
        var spawned = graph.Object("SpawnedSmith", actor);
        var component = graph.Object("SmithInteractable", graph.Class("/Script/Dungeons", "InteractableComponent"));
        var merchant = graph.Class("/Script/Dungeons", "MerchantActor");
        var member = graph.Import("ObjectProperty", "mInteractableComponent", merchant);
        graph.Branch(new EX_FinalFunction { StackNode = graph.Fn(actor, "HasAuthority"), Parameters = Array.Empty<KismetExpression>() }, "Done");
        graph.Set(origin, new EX_FinalFunction { StackNode = graph.Fn(actor, "K2_GetActorLocation"), Parameters = Array.Empty<KismetExpression>() });
        for (int i = 0; i < MerchantScreens.Services.Length; i++) {
            string service = MerchantScreens.Services[i], next = "Next" + i;
            var npc = graph.Import("BlueprintGeneratedClass", "BP_RebalanceCamp" + service + "_C",
                graph.Package("/Game/" + MerchantScreens.Folder + "/BP_RebalanceCamp" + service), "/Script/Engine");
            graph.Code.Add(graph.Math("GameplayStatics", "GetAllActorsOfClass", new EX_Self(), new EX_ObjectConst { Value = npc }, graph.L(existing)));
            graph.Branch(graph.Math("KismetMathLibrary", "EqualEqual_IntInt",
                graph.Math("KismetArrayLibrary", "Array_Length", graph.L(existing)), new EX_IntConst { Value = 0 }), next);
            graph.Set(transform, graph.Math("KismetMathLibrary", "MakeTransform",
                graph.Math("KismetMathLibrary", "Add_VectorVector", graph.L(origin), new EX_VectorConst { Value = new FVector(350f, (i - 1) * 220f, 0f) }),
                new EX_RotationConst { Value = new FRotator(0f, 180f, 0f) }, new EX_VectorConst { Value = new FVector(1f, 1f, 1f) }));
            graph.Set(spawned, graph.Math("GameplayStatics", "BeginDeferredActorSpawnFromClass", new EX_Self(),
                new EX_ObjectConst { Value = npc }, graph.L(transform), new EX_ByteConst { Value = 2 }, new EX_NoObject()));
            graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(spawned)), next);
            graph.Code.Add(graph.Context(graph.L(spawned), new EX_FinalFunction { StackNode = graph.Fn(actor, "SetReplicates"), Parameters = new KismetExpression[] { new EX_False() } }));
            graph.Set(spawned, graph.Math("GameplayStatics", "FinishSpawningActor", graph.L(spawned), graph.L(transform)));
            graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(spawned)), next);
            graph.Set(component, graph.Context(graph.L(spawned), new EX_InstanceVariable { Variable = new KismetPropertyPointer(member) }, component));
            // If the expected native component is missing, discard the actor
            // rather than leave an interactive/free Tower service in Camp.
            string destroy = "Destroy" + i;
            graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(component)), destroy);
            graph.Code.Add(graph.Context(graph.L(component), new EX_FinalFunction {
                StackNode = graph.Fn(graph.Class("/Script/Dungeons", "InteractableComponent"), "DisableInteraction"), Parameters = Array.Empty<KismetExpression>() }));
            // Jump using a constant-false branch; all branch targets are fixed
            // from serialized instruction lengths before output.
            graph.Branch(new EX_False(), next);
            graph.Label(destroy);
            graph.Code.Add(graph.Context(graph.L(spawned), new EX_FinalFunction { StackNode = graph.Fn(actor, "K2_DestroyActor"), Parameters = Array.Empty<KismetExpression>() }));
            graph.Label(next);
        }
        graph.Label("Done"); graph.Finish();
        var begin = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "ReceiveBeginPlay");
        if (begin.ScriptBytecode.Length != 3 || begin.ScriptBytecode[0] is not EX_LocalFinalFunction || begin.ScriptBytecode[1] is not EX_Return)
            throw new InvalidDataException("Unexpected Camp chest begin-play graph.");
        // Run before the original chest graph: it may remove an already-claimed
        // chest, so its transform/world must be used while the anchor is live.
        begin.ScriptBytecode = new[] { new EX_LocalFinalFunction { StackNode = graph.Index(graph.Function), Parameters = Array.Empty<KismetExpression>() }, begin.ScriptBytecode[0], begin.ScriptBytecode[1], begin.ScriptBytecode[2] };
        begin.ScriptBytecodeRaw = null; begin.ScriptBytecodeSize = begin.ScriptBytecode.Sum(graph.Size);
        begin.CreateBeforeSerializationDependencies.Add(graph.Index(graph.Function));
        asset.DependsMap = asset.Exports.Select(e => e.SerializationBeforeSerializationDependencies.Concat(e.CreateBeforeSerializationDependencies)
            .Concat(e.SerializationBeforeCreateDependencies).Concat(e.CreateBeforeCreateDependencies).Select(p => p.Index).Distinct().ToArray()).ToList();
        Validate(asset); return asset;
    }
    internal static void Validate(UAsset asset)
    {
        if (asset.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString() == "Default__BP_LobbyChest_C")
            .Data.OfType<IntPropertyData>().Single(p => p.Name.ToString() == "EmeraldsReward").Value != 100)
            throw new InvalidDataException("Accepted Camp reward must be preserved.");
        var function = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "RebalanceSpawnCampSmiths");
        if (!function.FunctionFlags.HasFlag(EFunctionFlags.FUNC_HasDefaults)) throw new InvalidDataException("Native local struct construction missing.");
        var code = function.ScriptBytecode;
        if (code[0] is not EX_JumpIfNot host || host.BooleanExpression is not EX_FinalFunction authority
            || authority.StackNode.ToImport(asset).ObjectName.ToString() != "HasAuthority") throw new InvalidDataException("Host-only spawn guard missing.");
        var calls = Flatten(code).OfType<EX_CallMath>().Select(c => c.StackNode.ToImport(asset).ObjectName.ToString()).ToArray();
        foreach (string call in new[] { "GetAllActorsOfClass", "BeginDeferredActorSpawnFromClass", "FinishSpawningActor" })
            if (calls.Count(c => c == call) != 3) throw new InvalidDataException("Placement/deduplication call count changed.");
        var native = Flatten(code).OfType<EX_FinalFunction>().Select(c => c.StackNode.ToImport(asset).ObjectName.ToString()).ToArray();
        if (native.Count(n => n == "DisableInteraction") != 3 || native.Count(n => n == "K2_DestroyActor") != 3 || native.Count(n => n == "SetReplicates") != 3)
            throw new InvalidDataException("Preview interaction guard missing.");
        var allowed = new HashSet<string> {
            "Actor.HasAuthority", "Actor.K2_GetActorLocation", "Actor.SetReplicates", "Actor.K2_DestroyActor",
            "InteractableComponent.DisableInteraction", "GameplayStatics.GetAllActorsOfClass",
            "GameplayStatics.BeginDeferredActorSpawnFromClass", "GameplayStatics.FinishSpawningActor",
            "KismetMathLibrary.EqualEqual_IntInt", "KismetMathLibrary.MakeTransform", "KismetMathLibrary.Add_VectorVector",
            "KismetArrayLibrary.Array_Length", "KismetSystemLibrary.IsValid"
        };
        foreach (var node in Flatten(code)) {
            var reference = node switch { EX_CallMath m => m.StackNode, EX_FinalFunction f => f.StackNode, _ => null };
            if (reference == null) continue;
            var imported = reference.ToImport(asset);
            var parent = imported.OuterIndex.ToImport(asset);
            var package = parent.OuterIndex.ToImport(asset).ObjectName.ToString();
            if (!allowed.Contains(parent.ObjectName + "." + imported.ObjectName)
                || package != (parent.ObjectName.ToString() == "InteractableComponent" ? "/Script/Dungeons" : "/Script/Engine"))
                throw new InvalidDataException("Unexpected native operation in placement preview.");
            if (node is EX_FinalFunction replicas && imported.ObjectName.ToString() == "SetReplicates"
                && (replicas.Parameters.Length != 1 || replicas.Parameters[0] is not EX_False))
                throw new InvalidDataException("Preview NPC replication must remain disabled.");
            if (node is EX_CallMath spawn && imported.ObjectName.ToString() == "BeginDeferredActorSpawnFromClass"
                && (spawn.Parameters.Length != 5 || spawn.Parameters[3] is not EX_ByteConst collision || collision.Value != 2))
                throw new InvalidDataException("Preview collision handling changed.");
        }
        var classes = Flatten(code).OfType<EX_ObjectConst>().Select(o => o.Value.ToImport(asset)).ToArray();
        foreach (string service in MerchantScreens.Services) {
            string name = "BP_RebalanceCamp" + service;
            if (classes.Count(c => c.ObjectName.ToString() == name + "_C"
                && c.ClassName.ToString() == "BlueprintGeneratedClass" && c.ClassPackage.ToString() == "/Script/Engine"
                && c.OuterIndex.ToImport(asset).ObjectName.ToString() == "/Game/" + MerchantScreens.Folder + "/" + name) != 2)
                throw new InvalidDataException("Owned preview actor class binding changed.");
        }
        var begin = asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "ReceiveBeginPlay");
        if (begin.ScriptBytecode.Length != 4 || begin.ScriptBytecode[0] is not EX_LocalFinalFunction hook
            || hook.StackNode.Index != FPackageIndex.FromExport(asset.Exports.IndexOf(function)).Index) throw new InvalidDataException("Camp spawn hook missing.");
        var owner = asset.Exports.OfType<ClassExport>().Single();
        if (owner.FuncMap[function.ObjectName].Index != hook.StackNode.Index || function.Children.Any(c => c.ToExport(asset).OuterIndex.Index != hook.StackNode.Index))
            throw new InvalidDataException("Placement owner/locals invalid.");
        uint cursor = 0; var offsets = new HashSet<uint>();
        using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, asset);
        foreach (var expr in code) { offsets.Add(cursor); cursor += (uint)ExpressionSerializer.WriteExpression(expr, writer); }
        if (code.OfType<EX_JumpIfNot>().Any(j => !offsets.Contains(j.CodeOffset))) throw new InvalidDataException("Invalid spawn branch target.");
        uint returnOffset = cursor;
        using var sizeStream = new MemoryStream(); using var sizeWriter = new AssetBinaryWriter(sizeStream, asset);
        returnOffset -= (uint)ExpressionSerializer.WriteExpression(code[^1], sizeWriter);
        returnOffset -= (uint)ExpressionSerializer.WriteExpression(code[^2], sizeWriter);
        if (host.CodeOffset != returnOffset) throw new InvalidDataException("Non-host must skip all placement operations.");
    }
    internal static void SelfTest(string source)
    {
        Action<UAsset>[] corruptions = {
            a => a.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString() == "Default__BP_LobbyChest_C").Data.OfType<IntPropertyData>().Single(p => p.Name.ToString() == "EmeraldsReward").Value = 50,
            a => Spawn(a).FunctionFlags &= ~EFunctionFlags.FUNC_HasDefaults,
            a => ((EX_JumpIfNot)Spawn(a).ScriptBytecode[0]).CodeOffset = 0,
            a => Flatten(Spawn(a).ScriptBytecode).OfType<EX_FinalFunction>().First(f => f.StackNode.ToImport(a).ObjectName.ToString() == "SetReplicates").Parameters[0] = new EX_True(),
            a => Flatten(Spawn(a).ScriptBytecode).OfType<EX_CallMath>().First(f => f.StackNode.ToImport(a).ObjectName.ToString() == "BeginDeferredActorSpawnFromClass").Parameters[3] = new EX_ByteConst { Value = 1 },
            a => { var f = Flatten(Spawn(a).ScriptBytecode).OfType<EX_FinalFunction>().First(f => f.StackNode.ToImport(a).ObjectName.ToString() == "DisableInteraction"); f.StackNode = Flatten(Spawn(a).ScriptBytecode).OfType<EX_FinalFunction>().First(f => f.StackNode.ToImport(a).ObjectName.ToString() == "K2_DestroyActor").StackNode; },
            a => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "ReceiveBeginPlay").ScriptBytecode[0] = new EX_Nothing(),
            a => ((EX_JumpIfNot)Spawn(a).ScriptBytecode[3]).CodeOffset = uint.MaxValue
        };
        foreach (var corrupt in corruptions) {
            var asset = Prepare(source); corrupt(asset);
            bool rejected = false;
            try { Validate(asset); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Corrupted placement graph accepted.");
        }
        Console.WriteLine("Eight Camp placement rejection checks passed.");
    }
    static FunctionExport Spawn(UAsset asset) => asset.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "RebalanceSpawnCampSmiths");
    static IEnumerable<KismetExpression> Flatten(IEnumerable<KismetExpression> expressions)
    {
        foreach (var e in expressions) {
            yield return e;
            var children = e switch {
                EX_CallMath c => c.Parameters, EX_FinalFunction f => f.Parameters,
                EX_JumpIfNot j => new[] { j.BooleanExpression }, EX_Let l => new[] { l.Variable, l.Expression },
                EX_LetObj l => new[] { l.VariableExpression, l.AssignmentExpression }, EX_Context c => new[] { c.ObjectExpression, c.ContextExpression },
                _ => Array.Empty<KismetExpression>()
            };
            foreach (var child in Flatten(children)) yield return child;
        }
    }
}
