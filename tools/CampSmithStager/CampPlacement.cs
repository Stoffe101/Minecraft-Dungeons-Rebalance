using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.UnrealTypes;
using UAssetAPI.PropertyTypes.Objects;

// Experimental placement / read-only UI preview. No upgrade, currency or
// inventory function is included in this spawn graph. UI preview content must
// separately remove the stock native Tower transaction bindings.
static class CampPlacement
{
    internal static UAsset Prepare(string source, bool interactionPreview = false)
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
        var anchors = graph.ActorArray(actor, "GiftWrappers");
        var anchor = graph.Object("GiftWrapperAnchor", actor);
        var right = graph.Struct("AnchorRight", "Vector");
        var forward = graph.Struct("AnchorForward", "Vector");
        var rotation = graph.Struct("LabelRotation", "Rotator");
        var camera = graph.Object("CampCamera", graph.Class("/Script/Engine", "PlayerCameraManager"));
        var label = graph.Object("SmithLabel", graph.Class("/Script/Engine", "TextRenderActor"));
        var textRenderClass = graph.Class("/Script/Engine", "TextRenderComponent");
        var textRender = graph.Object("SmithTextRender", textRenderClass);
        var boundsOrigin = graph.Struct("SmithBoundsOrigin", "Vector");
        var boundsExtent = graph.Struct("SmithBoundsExtent", "Vector");
        var labelPosition = graph.Struct("LabelPosition", "Vector");
        var origin = graph.Struct("CampAnchorLocation", "Vector");
        var transform = graph.Struct("SmithTransform", "Transform");
        var spawned = graph.Object("SpawnedSmith", actor);
        var component = graph.Object("SmithInteractable", graph.Class("/Script/Dungeons", "InteractableComponent"));
        var merchant = graph.Class("/Script/Dungeons", "MerchantActor");
        var member = graph.Import("ObjectProperty", "mInteractableComponent", merchant);
        graph.Branch(new EX_FinalFunction { StackNode = graph.Fn(actor, "HasAuthority"), Parameters = Array.Empty<KismetExpression>() }, "Done");
        // Resolve the actual native Gift Wrapper; never fall back to the reward
        // chest location under the bridge when that merchant is unavailable.
        var gift = graph.Import("BlueprintGeneratedClass", "BP_LobbyVillager_GiftWrapper_C",
            graph.Package("/Game/Decor/Prefabs/Merchants/BP_LobbyVillager_GiftWrapper"), "/Script/Engine");
        graph.Code.Add(graph.Math("GameplayStatics", "GetAllActorsOfClass", new EX_Self(), new EX_ObjectConst { Value = gift }, graph.L(anchors)));
        graph.Branch(graph.Math("KismetMathLibrary", "Greater_IntInt",
            graph.Math("KismetArrayLibrary", "Array_Length", graph.L(anchors)), new EX_IntConst { Value = 0 }), "Done");
        graph.Code.Add(graph.Math("KismetArrayLibrary", "Array_Get", graph.L(anchors), new EX_IntConst { Value = 0 }, graph.L(anchor)));
        graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(anchor)), "Done");
        graph.Set(origin, graph.Context(graph.L(anchor), new EX_FinalFunction { StackNode = graph.Fn(actor, "K2_GetActorLocation"), Parameters = Array.Empty<KismetExpression>() }));
        graph.Set(right, graph.Context(graph.L(anchor), new EX_FinalFunction { StackNode = graph.Fn(actor, "GetActorRightVector"), Parameters = Array.Empty<KismetExpression>() }));
        graph.Set(forward, graph.Context(graph.L(anchor), new EX_FinalFunction { StackNode = graph.Fn(actor, "GetActorForwardVector"), Parameters = Array.Empty<KismetExpression>() }));
        graph.Set(rotation, graph.Context(graph.L(anchor), new EX_FinalFunction { StackNode = graph.Fn(actor, "K2_GetActorRotation"), Parameters = Array.Empty<KismetExpression>() }));
        graph.Set(camera, graph.Math("GameplayStatics", "GetPlayerCameraManager", new EX_Self(), new EX_IntConst { Value = 0 }));
        for (int i = 0; i < MerchantScreens.Services.Length; i++) {
            string service = MerchantScreens.Services[i], next = "Next" + i;
            var npc = graph.Import("BlueprintGeneratedClass", "BP_RebalanceCamp" + service + "_C",
                graph.Package("/Game/" + MerchantScreens.Folder + "/BP_RebalanceCamp" + service), "/Script/Engine");
            graph.Code.Add(graph.Math("GameplayStatics", "GetAllActorsOfClass", new EX_Self(), new EX_ObjectConst { Value = npc }, graph.L(existing)));
            graph.Branch(graph.Math("KismetMathLibrary", "EqualEqual_IntInt",
                graph.Math("KismetArrayLibrary", "Array_Length", graph.L(existing)), new EX_IntConst { Value = 0 }), next);
            graph.Set(rotation, graph.Context(graph.L(anchor), new EX_FinalFunction { StackNode = graph.Fn(actor, "K2_GetActorRotation"), Parameters = Array.Empty<KismetExpression>() }));
            graph.Set(transform, graph.Math("KismetMathLibrary", "MakeTransform",
                graph.Math("KismetMathLibrary", "Add_VectorVector", graph.L(origin),
                    graph.Math("KismetMathLibrary", "Add_VectorVector",
                        graph.Math("KismetMathLibrary", "Multiply_VectorFloat", graph.L(right), new EX_FloatConst { Value = 900f }),
                        graph.Math("KismetMathLibrary", "Multiply_VectorFloat", graph.L(forward), new EX_FloatConst { Value = (i - 1) * 650f }))),
                graph.L(rotation), new EX_VectorConst { Value = new FVector(1f, 1f, 1f) }));
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
            if (!interactionPreview) graph.Code.Add(graph.Context(graph.L(component), new EX_FinalFunction {
                StackNode = graph.Fn(graph.Class("/Script/Dungeons", "InteractableComponent"), "DisableInteraction"), Parameters = Array.Empty<KismetExpression>() }));
            // Independent engine text labels do not depend on the disabled
            // InteractableComponent's balloon UI. TextRenderActor.TextRender
            // is a reflected property; its C++ getter is NOT a UFUNCTION.
            graph.Code.Add(graph.Context(graph.L(spawned), new EX_FinalFunction {
                StackNode = graph.Fn(actor, "GetActorBounds"), Parameters = new KismetExpression[] { new EX_False(), graph.L(boundsOrigin), graph.L(boundsExtent) } }));
            graph.Set(labelPosition, graph.Math("KismetMathLibrary", "Add_VectorVector",
                graph.Math("KismetMathLibrary", "Add_VectorVector", graph.L(boundsOrigin),
                    graph.Math("KismetMathLibrary", "Multiply_VectorVector", graph.L(boundsExtent), new EX_VectorConst { Value = new FVector(0f, 0f, 1f) })),
                new EX_VectorConst { Value = new FVector(0f, 0f, 70f) }));
            graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(camera)), "CameraReady" + i);
            graph.Set(rotation, graph.Math("KismetMathLibrary", "FindLookAtRotation", graph.L(labelPosition),
                graph.Context(graph.L(camera), new EX_FinalFunction { StackNode = graph.Fn(graph.Class("/Script/Engine", "PlayerCameraManager"), "GetCameraLocation"), Parameters = Array.Empty<KismetExpression>() })));
            graph.Label("CameraReady" + i);
            graph.Set(transform, graph.Math("KismetMathLibrary", "MakeTransform", graph.L(labelPosition), graph.L(rotation),
                new EX_VectorConst { Value = new FVector(1f, 1f, 1f) }));
            graph.Set(label, graph.Math("GameplayStatics", "BeginDeferredActorSpawnFromClass", new EX_Self(),
                new EX_ObjectConst { Value = graph.Class("/Script/Engine", "TextRenderActor") }, graph.L(transform), new EX_ByteConst { Value = 2 }, graph.L(spawned)));
            graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(label)), next);
            graph.Code.Add(graph.Context(graph.L(label), new EX_FinalFunction { StackNode = graph.Fn(actor, "SetReplicates"), Parameters = new KismetExpression[] { new EX_False() } }));
            graph.Set(label, graph.Math("GameplayStatics", "FinishSpawningActor", graph.L(label), graph.L(transform)));
            graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(label)), next);
            graph.Set(textRender, graph.Context(graph.L(label), new EX_InstanceVariable {
                Variable = new KismetPropertyPointer(graph.Import("ObjectProperty", "TextRender", graph.Class("/Script/Engine", "TextRenderActor"))) }, textRender));
            graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(textRender)), "DestroyLabel" + i);
            graph.Code.Add(graph.Context(graph.L(textRender), new EX_FinalFunction { StackNode = graph.Fn(textRenderClass, "K2_SetText"),
                Parameters = new[] { graph.Math("KismetTextLibrary", "Conv_StringToText", new EX_StringConst { Value = service }) } }));
            graph.Code.Add(graph.Context(graph.L(textRender), new EX_FinalFunction { StackNode = graph.Fn(textRenderClass, "SetWorldSize"),
                Parameters = new KismetExpression[] { new EX_FloatConst { Value = 40f } } }));
            graph.Branch(new EX_False(), next);
            graph.Label("DestroyLabel" + i);
            graph.Code.Add(graph.Context(graph.L(label), new EX_FinalFunction { StackNode = graph.Fn(actor, "K2_DestroyActor"), Parameters = Array.Empty<KismetExpression>() }));
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
        Validate(asset, interactionPreview); return asset;
    }
    internal static void Validate(UAsset asset, bool interactionPreview = false)
    {
        FunctionLoadContract.ValidateOwned(asset);
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
            if (calls.Count(c => c == call) != (call == "GetAllActorsOfClass" ? 4 : 6)) throw new InvalidDataException("Placement/deduplication call count changed.");
        var native = Flatten(code).OfType<EX_FinalFunction>().Select(c => c.StackNode.ToImport(asset).ObjectName.ToString()).ToArray();
        if (native.Count(n => n == "DisableInteraction") != (interactionPreview ? 0 : 3) || native.Count(n => n == "K2_DestroyActor") != 6 || native.Count(n => n == "SetReplicates") != 6
            || native.Count(n => n == "K2_SetText") != 3 || native.Count(n => n == "SetWorldSize") != 3)
            throw new InvalidDataException("Preview interaction guard missing.");
        var allowed = new HashSet<string> {
            "Actor.HasAuthority", "Actor.K2_GetActorLocation", "Actor.SetReplicates", "Actor.K2_DestroyActor",
            "InteractableComponent.DisableInteraction", "GameplayStatics.GetAllActorsOfClass",
            "GameplayStatics.BeginDeferredActorSpawnFromClass", "GameplayStatics.FinishSpawningActor",
            "KismetMathLibrary.EqualEqual_IntInt", "KismetMathLibrary.MakeTransform", "KismetMathLibrary.Add_VectorVector",
            "KismetArrayLibrary.Array_Length", "KismetArrayLibrary.Array_Get", "KismetSystemLibrary.IsValid",
            "Actor.GetActorRightVector", "Actor.GetActorForwardVector", "Actor.K2_GetActorRotation", "Actor.GetActorBounds",
            "KismetMathLibrary.Greater_IntInt", "KismetMathLibrary.Multiply_VectorFloat", "KismetMathLibrary.Multiply_VectorVector",
            "KismetMathLibrary.FindLookAtRotation", "GameplayStatics.GetPlayerCameraManager", "PlayerCameraManager.GetCameraLocation",
            "TextRenderComponent.K2_SetText", "TextRenderComponent.SetWorldSize", "KismetTextLibrary.Conv_StringToText"
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
        if (classes.Count(c => c.ObjectName.ToString() == "BP_LobbyVillager_GiftWrapper_C"
            && c.ClassPackage.ToString() == "/Script/Engine" && c.ClassName.ToString() == "BlueprintGeneratedClass"
            && c.OuterIndex.ToImport(asset).ObjectName.ToString() == "/Game/Decor/Prefabs/Merchants/BP_LobbyVillager_GiftWrapper") != 1)
            throw new InvalidDataException("Gift Wrapper anchor class changed.");
        var names = Flatten(code).OfType<EX_StringConst>().Select(c => c.Value).ToArray();
        if (!names.SequenceEqual(MerchantScreens.Services)) throw new InvalidDataException("Smith overhead names changed.");
        var spacing = Flatten(code).OfType<EX_CallMath>().Where(c => c.StackNode.ToImport(asset).ObjectName.ToString() == "Multiply_VectorFloat").ToArray();
        if (spacing.Length != 6 || spacing.Where((c, i) => c.Parameters.Length != 2 || c.Parameters[1] is not EX_FloatConst value
            || value.Value != (i % 2 == 0 ? 900f : (i / 2 - 1) * 650f)).Any())
            throw new InvalidDataException("Gift Wrapper-relative smith spacing changed.");
        var textMember = Flatten(code).OfType<EX_InstanceVariable>().Where(v => v.Variable.Old.ToImport(asset).ObjectName.ToString() == "TextRender").ToArray();
        if (textMember.Length != 3 || textMember.Any(v => v.Variable.Old.ToImport(asset).OuterIndex.ToImport(asset).ObjectName.ToString() != "TextRenderActor"))
            throw new InvalidDataException("Reflected text label component changed.");
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
            a => Spawn(a).ScriptBytecode.OfType<EX_JumpIfNot>().Last().CodeOffset = uint.MaxValue,
            a => Flatten(Spawn(a).ScriptBytecode).OfType<EX_StringConst>().First().Value = "",
            a => ((EX_FloatConst)Flatten(Spawn(a).ScriptBytecode).OfType<EX_CallMath>().First(c => c.StackNode.ToImport(a).ObjectName.ToString() == "Multiply_VectorFloat").Parameters[1]).Value = 350f
        };
        foreach (var corrupt in corruptions) {
            var asset = Prepare(source); corrupt(asset);
            bool rejected = false;
            try { Validate(asset); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Corrupted placement graph accepted.");
        }
        Console.WriteLine("Ten Camp placement rejection checks passed.");
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
