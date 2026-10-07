using System.Reflection;
using System.Text.Json;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.FieldTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.UnrealTypes;

// Native preflight foundation only: no UI binding, charge or item mutation.
// Caller supplies the merchant's real currency ID, not a guessed serialized ID.
static class TestUpgradeAffordability
{
    internal const string Name = "RebalanceCanAffordTestUpgrade";
    internal const int Amount = 1;
    internal static void Add(UAsset asset)
    {
        VerifyContract();
        var graph = new SpawnGraph(asset, Name);
        graph.Function.FunctionFlags |= EFunctionFlags.FUNC_BlueprintPure | EFunctionFlags.FUNC_HasOutParms;
        var actor = graph.Class("/Script/Engine", "Actor");
        var player = graph.Object("Player", actor);
        player.Property.PropertyFlags = EPropertyFlags.CPF_Parm;
        var currency = graph.Property("Currency", "StructProperty", new UStructProperty {
            Struct = graph.Import("ScriptStruct", "SerializableItemId", graph.Package("/Script/Dungeons")) });
        currency.Property.PropertyFlags = EPropertyFlags.CPF_Parm;
        var result = graph.Property("ReturnValue", "BoolProperty", new UBoolProperty { ElementSize = 1, NativeBool = true });
        result.Property.PropertyFlags = EPropertyFlags.CPF_Parm | EPropertyFlags.CPF_OutParm | EPropertyFlags.CPF_ReturnParm;
        var walletClass = graph.Class("/Script/Dungeons", "WalletComponent");
        var wallet = graph.Object("PlayerWallet", walletClass);
        graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(player)), "Unavailable");
        graph.Branch(graph.Context(graph.L(player), new EX_FinalFunction { StackNode = graph.Fn(actor, "HasAuthority"),
            Parameters = Array.Empty<KismetExpression>() }), "Unavailable");
        graph.Set(wallet, graph.Context(graph.L(player), new EX_FinalFunction { StackNode = graph.Fn(actor, "GetComponentByClass"),
            Parameters = new KismetExpression[] { new EX_ObjectConst { Value = walletClass } } }));
        graph.Branch(graph.Math("KismetSystemLibrary", "IsValid", graph.L(wallet)), "Unavailable");
        graph.Code.Add(new EX_Return { ReturnExpression = graph.Math("KismetMathLibrary", "GreaterEqual_IntInt",
            graph.Context(graph.L(wallet), new EX_FinalFunction { StackNode = graph.Fn(walletClass, "Balance"),
                Parameters = new[] { graph.L(currency) } }), new EX_IntConst { Value = Amount }) });
        graph.Label("Unavailable");
        graph.Finish(new EX_False());
        Validate(asset);
    }
    internal static void Validate(UAsset asset)
    {
        FunctionLoadContract.ValidateOwned(asset);
        var f = asset.Exports.OfType<FunctionExport>().Single(e => e.ObjectName.ToString() == Name);
        var fields = f.Children.Select(p => (PropertyExport)p.ToExport(asset)).ToArray();
        if (fields.Length != 4 || fields[0].ObjectName.ToString() != "Player" || fields[1].ObjectName.ToString() != "Currency"
            || fields[2].Property is not UBoolProperty b || !b.NativeBool || b.ElementSize != 1
            || !fields[2].Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm)
            || !f.FunctionFlags.HasFlag(EFunctionFlags.FUNC_BlueprintPure)
            || fields[1].Property is not UStructProperty currencyType
            || currencyType.Struct.ToImport(asset).ObjectName.ToString() != "SerializableItemId"
            || currencyType.Struct.ToImport(asset).OuterIndex.ToImport(asset).ObjectName.ToString() != "/Script/Dungeons") throw new InvalidDataException("Invalid affordability fields.");
        var code = f.ScriptBytecode;
        if (code.Length != 7 || code[0] is not EX_JumpIfNot player || code[1] is not EX_JumpIfNot host
            || code[2] is not EX_LetObj lookup || code[3] is not EX_JumpIfNot wallet
            || code[4] is not EX_Return success || success.ReturnExpression is not EX_CallMath compare
            || compare.StackNode.ToImport(asset).ObjectName.ToString() != "GreaterEqual_IntInt"
            || compare.Parameters.Length != 2 || compare.Parameters[1] is not EX_IntConst amount || amount.Value != Amount
            || code[5] is not EX_Return unavailable || unavailable.ReturnExpression is not EX_False
            || code[6] is not EX_EndOfScript) throw new InvalidDataException("Invalid one-currency preflight.");
        void Variable(KismetExpression e, PropertyExport field) {
            if (e is not EX_LocalVariable v || v.Variable.Old.Index != FPackageIndex.FromExport(asset.Exports.IndexOf(field)).Index)
                throw new InvalidDataException("Wrong preflight native input.");
        }
        void Call(KismetExpression e, string owner, string name, params PropertyExport[] inputs) {
            if (e is not EX_CallMath m || m.StackNode.ToImport(asset).ObjectName.ToString() != name
                || m.StackNode.ToImport(asset).OuterIndex.ToImport(asset).ObjectName.ToString() != owner || m.Parameters.Length != inputs.Length)
                throw new InvalidDataException("Wrong preflight engine function.");
            for (int i = 0; i < inputs.Length; i++) Variable(m.Parameters[i], inputs[i]);
        }
        void Native(KismetExpression e, PropertyExport receiver, string owner, string name, int count) {
            if (e is not EX_Context c || c.ContextExpression is not EX_FinalFunction n || n.Parameters.Length != count)
                throw new InvalidDataException("Wrong preflight native function.");
            Variable(c.ObjectExpression, receiver);
            var imp = n.StackNode.ToImport(asset); var cls = imp.OuterIndex.ToImport(asset);
            if (imp.ObjectName.ToString() != name || cls.ObjectName.ToString() != owner
                || cls.OuterIndex.ToImport(asset).ObjectName.ToString() != (owner == "WalletComponent" ? "/Script/Dungeons" : "/Script/Engine"))
                throw new InvalidDataException("Wrong qualified preflight native function.");
        }
        Call(player.BooleanExpression, "KismetSystemLibrary", "IsValid", fields[0]);
        Native(host.BooleanExpression, fields[0], "Actor", "HasAuthority", 0);
        Variable(lookup.VariableExpression, fields[3]);
        Native(lookup.AssignmentExpression, fields[0], "Actor", "GetComponentByClass", 1);
        var get = (EX_FinalFunction)((EX_Context)lookup.AssignmentExpression).ContextExpression;
        if (get.Parameters[0] is not EX_ObjectConst clsConst || clsConst.Value.ToImport(asset).ObjectName.ToString() != "WalletComponent")
            throw new InvalidDataException("Wrong wallet component class.");
        Call(wallet.BooleanExpression, "KismetSystemLibrary", "IsValid", fields[3]);
        Native(compare.Parameters[0], fields[3], "WalletComponent", "Balance", 1);
        Variable(((EX_FinalFunction)((EX_Context)compare.Parameters[0]).ContextExpression).Parameters[0], fields[1]);
        uint offset = 0; using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, asset);
        for (int i = 0; i < 5; i++) offset += (uint)ExpressionSerializer.WriteExpression(code[i], writer);
        if (new[] { player, host, wallet }.Any(j => j.CodeOffset != offset)) throw new InvalidDataException("Preflight rejection must return false.");
    }
    internal static void SelfTest(string source)
    {
        Action<UAsset>[] corruptions = {
            a => ((UBoolProperty)((PropertyExport)Function(a).Children[2].ToExport(a)).Property).ElementSize = 0,
            a => ((EX_IntConst)((EX_CallMath)((EX_Return)Function(a).ScriptBytecode[4]).ReturnExpression).Parameters[1]).Value = 0,
            a => ((EX_JumpIfNot)Function(a).ScriptBytecode[1]).BooleanExpression = new EX_True(),
            a => ((EX_JumpIfNot)Function(a).ScriptBytecode[3]).CodeOffset = 0,
            a => ((EX_FinalFunction)((EX_Context)((EX_CallMath)((EX_Return)Function(a).ScriptBytecode[4]).ReturnExpression).Parameters[0]).ContextExpression).Parameters[0] = new EX_NoObject()
        };
        foreach (var corrupt in corruptions) {
            var asset = new UAsset(Path.Combine(source, "Dungeons/Content/UI/Merchant/UMG_Merchant.uasset"), EngineVersion.VER_UE4_22);
            Add(asset); corrupt(asset); bool rejected = false;
            try { Validate(asset); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Corrupted native affordability guard accepted.");
        }
        Console.WriteLine("Five native affordability rejection checks passed; no charges or upgrades executed.");
    }
    static FunctionExport Function(UAsset a) => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Name);
    static void VerifyContract()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NativeUpgradeContracts.json")!;
        using var document = JsonDocument.Parse(stream);
        var balance = document.RootElement.GetProperty("Classes").EnumerateArray().Single(c => c.GetProperty("Name").GetString() == "WalletComponent")
            .GetProperty("Functions").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == "Balance");
        var p = balance.GetProperty("Parameters");
        if (balance.GetProperty("ParameterSize").GetInt32() != 24 || balance.GetProperty("ReturnOffset").GetInt32() != 20
            || p.GetArrayLength() != 2 || p[0].GetProperty("Target").GetString() != "/Script/Dungeons.SerializableItemId"
            || p[0].GetProperty("Size").GetInt32() != 20 || p[1].GetProperty("Type").GetString() != "IntProperty")
            throw new InvalidDataException("Captured native balance contract changed.");
    }
}
