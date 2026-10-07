using System.Reflection;
using System.Text.Json;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.FieldTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.UnrealTypes;

// Test pricing only. Transactions and full item records stay native. A fee is
// deducted only after the selected/native held item demonstrates the upgrade.
static class PaidUpgradeGateway
{
    const string Game = "/Script/Dungeons";
    const string Engine = "/Script/Engine";
    internal const string Execute = "RebalanceExecuteUpgrade";
    internal const string Collect = "RebalanceCollectPowerItem";
    internal const string Confirm = "RebalanceConfirmUpgradeChoice";
    internal const string Settle = "RebalanceSettleUpgradeFee";
    const string Clear = "RebalanceClearUpgradeState";
    const string Payment = "RebalanceResolveUpgradePayment";
    const string Mark = "RebalanceMarkUpgradeChoice";
    const string Cancel = "RebalanceCancelUpgradeChoice";
    internal static bool Present(UAsset a) => a.Exports.OfType<FunctionExport>().Any(f => f.ObjectName.ToString() == Execute);
    internal static void Add(UAsset a, string service)
    {
        VerifyContracts();
        var names = new[] { Clear, Payment, Execute, Settle, Mark, Cancel, Confirm, Collect, "OnTransactionExecuted" };
        var graphs = names.ToDictionary(n => n, n => new SpawnGraph(a, n));
        var seed = graphs[Clear];
        var pending = seed.Object("RebalancePendingTransaction", seed.Class(Game, "MerchantTransactionBase"), true);
        var wallet = seed.Object("RebalancePendingWallet", seed.Class(Game, "WalletComponent"), true);
        var currency = seed.NativeStruct("RebalancePendingCurrency", "SerializableItemId", true);
        var slot = seed.Object("RebalancePendingSlot", seed.Class(Game, "InventoryItemSlot"), true);
        var item = seed.Object("RebalancePendingItem", seed.Class(Game, "InventoryItem"), true);
        var before = seed.NativeStruct("RebalanceBeforeItemData", "InventoryItemData", true);
        var awaiting = seed.Boolean("RebalanceAwaitingChoice", true);
        var status = seed.String("RebalanceUpgradeStatus", true);
        KismetExpression Merchant(SpawnGraph g) => g.Member(Game, "MerchantBaseWidget", "mMerchant", new EX_Self());
        KismetExpression Player(SpawnGraph g) => g.Member(Game, "MerchantBase", "mPlayerCharacterOwner", Merchant(g));
        KismetExpression Valid(SpawnGraph g, KismetExpression e) => g.Math("KismetSystemLibrary", "IsValid", e);
        KismetExpression Not(SpawnGraph g, KismetExpression e) => g.Math("KismetMathLibrary", "Not_PreBool", e);
        KismetExpression Equal(SpawnGraph g, KismetExpression x, KismetExpression y) => g.Math("KismetMathLibrary", "EqualEqual_ObjectObject", x, y);
        KismetExpression Tx(SpawnGraph g, string fn, KismetExpression receiver, params KismetExpression[] args) => g.Native(Game, "MerchantTransactionBase", fn, receiver, args);
        KismetExpression Balance(SpawnGraph g) => g.Native(Game, "WalletComponent", "Balance", g.L(wallet), g.L(currency));
        KismetExpression Funds(SpawnGraph g) => g.Math("KismetMathLibrary", "GreaterEqual_IntInt", Balance(g), new EX_IntConst { Value = 1 });
        KismetExpression Record(SpawnGraph g, KismetExpression obj) => g.Member(Game, "InventoryItem", "Item", obj, "StructProperty");
        void Message(SpawnGraph g, string text) => g.Set(status, new EX_StringConst { Value = text });
        void Host(SpawnGraph g, string reject) {
            g.Branch(Valid(g, Merchant(g)), reject); g.Branch(Valid(g, Player(g)), reject);
            g.Branch(g.Native(Engine, "Actor", "HasAuthority", Player(g)), reject);
        }
        string money = service == "Gildsmith" ? "gold" : "emerald";
        string txClass = service switch { "Uniquesmith" => "UniqueCollectItem", "Powersmith" => "UpgradeTowerItem", _ => "GildItem" };
        foreach (var field in new[] { pending, wallet, slot, item }) seed.Set(field, new EX_NoObject());
        seed.Set(awaiting, new EX_False()); seed.Finish();

        // Currency IDs contain unreflected bytes. Obtain a complete native ID
        // from this player's real Camp merchant, never manufacture a struct.
        {
            var g = graphs[Payment];
            var result = g.Boolean("ReturnValue"); result.Property.PropertyFlags = EPropertyFlags.CPF_Parm | EPropertyFlags.CPF_OutParm | EPropertyFlags.CPF_ReturnParm;
            g.Function.FunctionFlags |= EFunctionFlags.FUNC_HasOutParms;
            var merchants = g.ActorArray(g.Class(Game, "MerchantBase"), "CurrencyProviders");
            var provider = g.Object("CurrencyProvider", g.Class(Game, "MerchantBase"));
            var component = g.Object("CurrencyComponent", g.Class(Game, "MerchantCurrencyComponent"));
            var index = g.Integer("ProviderIndex");
            Host(g, "Unavailable");
            g.Set(wallet, g.Native(Engine, "Actor", "GetComponentByClass", Player(g), new EX_ObjectConst { Value = g.Class(Game, "WalletComponent") }));
            g.Branch(Valid(g, g.L(wallet)), "Unavailable");
            g.Code.Add(g.Math("GameplayStatics", "GetAllActorsOfClass", new EX_Self(),
                new EX_ObjectConst { Value = g.Class(Game, service == "Gildsmith" ? "PiglinMerchant" : "BlacksmithMerchant") }, g.L(merchants)));
            g.Set(index, new EX_IntConst { Value = 0 }); g.Label("ProviderLoop");
            g.Branch(g.Math("KismetMathLibrary", "Less_IntInt", g.L(index), g.Math("KismetArrayLibrary", "Array_Length", g.L(merchants))), "Unavailable");
            g.Code.Add(g.Math("KismetArrayLibrary", "Array_Get", g.L(merchants), g.L(index), g.L(provider)));
            g.Branch(Valid(g, g.L(provider)), "NextProvider");
            g.Branch(Equal(g, g.Member(Game, "MerchantBase", "mPlayerCharacterOwner", g.L(provider)), Player(g)), "NextProvider");
            g.Set(component, g.Member(Game, "MerchantBase", "mMerchantCurrencyComponent", g.L(provider)));
            g.Branch(Valid(g, g.L(component)), "NextProvider");
            g.Set(currency, g.Native(Game, "MerchantCurrencyComponent", "GetCurrencyItemId", g.L(component)));
            g.Branch(g.Static(Game, "ItemFunctionLibrary", "IsItemIdValid", g.L(currency)), "NextProvider");
            g.Code.Add(new EX_Return { ReturnExpression = new EX_True() });
            g.Label("NextProvider"); g.Set(index, g.Math("KismetMathLibrary", "Add_IntInt", g.L(index), new EX_IntConst { Value = 1 }));
            g.Branch(new EX_False(), "ProviderLoop");
            g.Label("Unavailable"); Message(g, "Open the Camp " + (service == "Gildsmith" ? "Piglin" : "Blacksmith") + " shop first; host only"); g.Finish(new EX_False());
        }
        {
            var g = graphs[Execute];
            var transaction = g.Object("Transaction", g.Class(Game, "MerchantTransactionBase"));
            var selection = g.Object("Selection", g.Class(Game, "SelectInventorySlot"));
            var executed = g.Boolean("Executed");
            g.Branch(Not(g, Valid(g, g.L(pending))), "Done");
            Host(g, "Done");
            Message(g, "Select an eligible inventory item");
            g.Set(transaction, g.Native(Game, "MerchantBaseWidget", "GetTransactionByClass", new EX_Self(), new EX_ObjectConst { Value = g.Class(Game, txClass) }));
            Message(g, "Native transaction unavailable in Camp");
            g.Branch(Valid(g, g.L(transaction)), "Done");
            Message(g, "Native price unsupported; no upgrade requested");
            g.Branch(Not(g, Tx(g, "HasPrice", g.L(transaction))), "Done");
            Message(g, "Native smith rejected this Camp item; no fee charged");
            g.Branch(Tx(g, "CanExecute", g.L(transaction)), "Done");
            Message(g, "Select an eligible inventory item");
            g.Set(selection, g.Native(Game, "MerchantBaseWidget", "GetSelectionByClass", new EX_Self(), new EX_ObjectConst { Value = g.Class(Game, "SelectInventorySlotItem") }));
            g.Branch(Valid(g, g.L(selection)), "SelectionFallback"); g.Branch(new EX_False(), "SelectionReady");
            g.Label("SelectionFallback");
            g.Set(selection, g.Native(Game, "MerchantBaseWidget", "GetSelectionByClass", new EX_Self(), new EX_ObjectConst { Value = g.Class(Game, "SelectInventorySlot") }));
            g.Label("SelectionReady"); g.Branch(Valid(g, g.L(selection)), "Done");
            g.Set(slot, g.Native(Game, "SelectInventorySlot", "GetInventorySlot", g.L(selection)));
            g.Branch(Valid(g, g.L(slot)), "Done");
            g.Set(item, g.Member(Game, "InventoryItemSlot", "Item", g.L(slot)));
            g.Branch(Valid(g, g.L(item)), "Done");
            Message(g, "Tower clones cannot be upgraded in Camp");
            g.Branch(Not(g, g.Native(Game, "InventoryItem", "IsCloned", g.L(item))), "Done");
            g.Set(before, Record(g, g.L(item)));
            g.Branch(g.Own(Payment), "Done");
            Message(g, "Need 1 " + money);
            g.Branch(g.Own(TestUpgradeAffordability.Name, Player(g), g.L(currency)), "Done");
            g.Set(pending, g.L(transaction)); g.Set(awaiting, new EX_False());
            Message(g, "Waiting for native upgrade");
            g.Set(executed, Tx(g, "TryExecute", g.L(transaction)));
            g.Branch(Not(g, g.L(awaiting)), "Done");
            g.Branch(g.L(executed), "Failed");
            g.Code.Add(g.Own(Settle, g.L(transaction))); g.Branch(new EX_False(), "Done");
            g.Label("Failed"); g.Code.Add(g.Own(Clear)); Message(g, "Native upgrade unavailable; no fee charged");
            g.Label("Done"); g.Finish();
        }
        {
            var g = graphs[Mark];
            g.Branch(Valid(g, g.L(pending)), "Done"); g.Set(awaiting, new EX_True());
            Message(g, "Choose a Unique outcome - 1 " + money); g.Label("Done"); g.Finish();
            g = graphs[Cancel]; g.Branch(g.L(awaiting), "Done"); g.Code.Add(g.Own(Clear));
            Message(g, "Choice cancelled; no fee charged"); g.Label("Done"); g.Finish();
        }
        {
            var g = graphs[Confirm];
            var choice = g.NativeStruct("choosenItem", "InventoryItemData"); choice.Property.PropertyFlags = EPropertyFlags.CPF_Parm;
            var transaction = g.Object("Transaction", g.Class(Game, "MerchantTransactionBase"));
            g.Branch(g.L(awaiting), "Done"); g.Branch(Valid(g, g.L(pending)), "Done"); Host(g, "Cancel");
            g.Branch(Valid(g, g.L(wallet)), "Cancel"); g.Branch(Funds(g), "Cancel");
            g.Set(transaction, g.L(pending)); g.Set(awaiting, new EX_False());
            g.Code.Add(g.Native(Game, "MerchantBaseWidget", "OnDecisionMade", new EX_Self(), g.L(choice)));
            g.Code.Add(g.Own(Settle, g.L(transaction))); g.Branch(new EX_False(), "Done");
            g.Label("Cancel"); g.Code.Add(g.Own(Clear)); Message(g, "Choice cancelled; need 1 " + money);
            g.Label("Done"); g.Finish();
        }
        {
            var g = graphs[Settle];
            var transaction = g.Object("transaction", g.Class(Game, "MerchantTransactionBase")); transaction.Property.PropertyFlags = EPropertyFlags.CPF_Parm;
            var afterItem = g.Object("AfterItem", g.Class(Game, "InventoryItem"));
            var after = g.NativeStruct("AfterItemData", "InventoryItemData");
            var balance = g.Integer("BalanceBeforeCharge");
            var heldSlots = g.ActorArray(g.Class(Game, "MerchantItemSlotBase"), "HeldItemSlots");
            var held = g.Object("HeldItemSlot", g.Class(Game, "MerchantItemSlotBase")); var index = g.Integer("HeldSlotIndex");
            g.Branch(Valid(g, g.L(pending)), "Done"); g.Branch(Equal(g, g.L(transaction), g.L(pending)), "Done");
            g.Branch(Not(g, g.L(awaiting)), "Done"); Host(g, "Done");
            Message(g, "No changed native result yet; no fee charged");
            g.Set(afterItem, g.L(item));
            g.Branch(Valid(g, g.L(slot)), "ReadAfter");
            g.Set(afterItem, g.Member(Game, "InventoryItemSlot", "Item", g.L(slot)));
            g.Branch(Valid(g, g.L(afterItem)), "ReadAfter"); g.Branch(new EX_False(), "HasAfter");
            g.Label("ReadAfter"); g.Set(afterItem, g.L(item));
            g.Label("HasAfter"); g.Branch(Valid(g, g.L(afterItem)), service == "Powersmith" ? "HeldItems" : "Done");
            g.Set(after, Record(g, g.L(afterItem)));
            KismetExpression Changed() => service switch {
                "Uniquesmith" => g.Math("KismetMathLibrary", "BooleanAND",
                    g.Static(Game, "ItemFunctionLibrary", "NotEqual_ItemTypeID",
                        g.StructMember("InventoryItemData", "ItemId", "StructProperty", g.L(before)), g.StructMember("InventoryItemData", "ItemId", "StructProperty", g.L(after))),
                    g.Static(Game, "ItemFunctionLibrary", "GetIsUniqueForItemType", g.StructMember("InventoryItemData", "ItemId", "StructProperty", g.L(after)))),
                "Powersmith" => g.Math("KismetMathLibrary", "BooleanAND",
                    Not(g, g.Static(Game, "ItemFunctionLibrary", "NotEqual_ItemTypeID", g.StructMember("InventoryItemData", "ItemId", "StructProperty", g.L(before)),
                        g.StructMember("InventoryItemData", "ItemId", "StructProperty", g.L(after)))),
                    g.Math("KismetMathLibrary", "Greater_FloatFloat", g.StructMember("InventoryItemData", "ItemPower", "FloatProperty", g.L(after)),
                        g.StructMember("InventoryItemData", "ItemPower", "FloatProperty", g.L(before)))) ,
                _ => g.Math("KismetMathLibrary", "BooleanAND", Not(g, g.StructMember("InventoryItemData", "bHasNetherite", "BoolProperty", g.L(before))),
                    g.StructMember("InventoryItemData", "bHasNetherite", "BoolProperty", g.L(after)))
            };
            g.Branch(Changed(), service == "Powersmith" ? "HeldItems" : "Done");
            g.Branch(new EX_False(), "Charge");
            if (service == "Powersmith") {
                g.Label("HeldItems");
                g.Set(heldSlots, g.Native(Game, "MerchantBaseWidget", "GetSlotsByClass", new EX_Self(), new EX_ObjectConst { Value = g.Class(Game, "UpgraderItemSlot") }));
                g.Set(index, new EX_IntConst { Value = 0 }); g.Label("HeldLoop");
                g.Branch(g.Math("KismetMathLibrary", "Less_IntInt", g.L(index), g.Math("KismetArrayLibrary", "Array_Length", g.L(heldSlots))), "Done");
                g.Code.Add(g.Math("KismetArrayLibrary", "Array_Get", g.L(heldSlots), g.L(index), g.L(held)));
                g.Branch(Valid(g, g.L(held)), "NextHeld");
                g.Branch(g.Native(Game, "MerchantItemSlotBase", "HasItem", g.L(held)), "NextHeld");
                g.Set(after, g.Native(Game, "MerchantItemSlotBase", "GetItem", g.L(held)));
                g.Branch(Changed(), "NextHeld"); g.Branch(new EX_False(), "Charge");
                g.Label("NextHeld"); g.Set(index, g.Math("KismetMathLibrary", "Add_IntInt", g.L(index), new EX_IntConst { Value = 1 })); g.Branch(new EX_False(), "HeldLoop");
            }
            g.Label("Charge"); g.Branch(Valid(g, g.L(wallet)), "Done"); g.Branch(Funds(g), "Done");
            g.Set(balance, Balance(g));
            // Clear the transaction first, retain wallet/currency until the
            // deduction completes. Reentrant/replayed success cannot charge twice.
            g.Set(pending, new EX_NoObject()); g.Set(awaiting, new EX_False());
            g.Code.Add(g.Native(Game, "WalletComponent", "Deduct", g.L(wallet), g.L(currency), new EX_IntConst { Value = 1 }));
            g.Branch(g.Math("KismetMathLibrary", "EqualEqual_IntInt", Balance(g), g.Math("KismetMathLibrary", "Subtract_IntInt", g.L(balance), new EX_IntConst { Value = 1 })), "PaymentFailed");
            Message(g, "Upgraded - charged 1 " + money); g.Code.Add(g.Own(Clear)); g.Branch(new EX_False(), "Done");
            g.Label("PaymentFailed"); Message(g, "Upgrade returned; payment not verified"); g.Code.Add(g.Own(Clear));
            g.Label("Done"); g.Finish();
        }
        {
            var g = graphs["OnTransactionExecuted"];
            var transaction = g.Object("transaction", g.Class(Game, "MerchantTransactionBase")); transaction.Property.PropertyFlags = EPropertyFlags.CPF_Parm | EPropertyFlags.CPF_ConstParm;
            var parent = g.Fn(g.Class(Game, "MerchantBaseWidget"), "OnTransactionExecuted");
            g.Function.SuperIndex = g.Function.SuperStruct = parent;
            g.Function.FunctionFlags = (EFunctionFlags)134744064; // captured native Blueprint event
            g.Function.SerializationBeforeSerializationDependencies.Add(parent);
            g.Code.Add(g.Own(Settle, g.L(transaction))); g.Finish();
            FunctionLoadContract.Validate(a, g.Function);
        }
        {
            var g = graphs[Collect];
            if (service == "Powersmith") {
                var transaction = g.Object("CollectionTransaction", g.Class(Game, "MerchantTransactionBase"));
                var upgrade = g.Object("UpgradeTransaction", g.Class(Game, "MerchantTransactionBase"));
                var collected = g.Boolean("Collected");
                Host(g, "Done");
                g.Set(transaction, g.Native(Game, "MerchantBaseWidget", "GetTransactionByClass", new EX_Self(), new EX_ObjectConst { Value = g.Class(Game, "CollectItem") }));
                g.Branch(Valid(g, g.L(transaction)), "Done"); g.Branch(Not(g, Tx(g, "HasPrice", g.L(transaction))), "Done");
                g.Branch(Tx(g, "CanExecute", g.L(transaction)), "Done");
                g.Branch(Valid(g, g.L(pending)), "Collect"); g.Branch(Valid(g, g.L(wallet)), "Done"); g.Branch(Funds(g), "Done");
                g.Label("Collect"); g.Set(upgrade, g.L(pending)); g.Set(collected, Tx(g, "TryExecute", g.L(transaction)));
                g.Branch(g.L(collected), "Done");
                g.Code.Add(g.Own(Settle, g.L(upgrade))); Message(g, "Native collection requested"); g.Label("Done");
            }
            g.Finish();
        }
        Prefix(a, "OnDecisionToBeMade", graphs[Mark].Function, true);
        Prefix(a, "CloseDecision", graphs[Cancel].Function, true);
        Prefix(a, "OnBoundMerchant", graphs[Clear].Function, true);
        var decision = a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "InstDecisionContent");
        var bindings = CampPlacement.Flatten(decision.ScriptBytecode).OfType<EX_BindDelegate>().Where(d => d.FunctionName.ToString() == "OnDecisionMade").ToArray();
        if (bindings.Length != 1) throw new InvalidDataException("Unexpected native decision confirmation binding.");
        bindings[0].FunctionName = new FName(a, Confirm);
        decision.ScriptBytecodeRaw = null;
        decision.CreateBeforeSerializationDependencies.Add(seed.Index(graphs[Confirm].Function));
        var cdo = a.Exports.OfType<NormalExport>().Single(e => e.ObjectName.ToString().StartsWith("Default__"));
        // Repeatable test services; native Camp screen remains open for collect.
        cdo.Data.RemoveAll(p => p.Name.ToString() == "bCloseAfterTransaction");
        cdo.Data.Add(new UAssetAPI.PropertyTypes.Objects.BoolPropertyData(new FName(a, "bCloseAfterTransaction")) { Value = false });
        Rebuild(a); Validate(a, service);
    }
    internal static void Prefix(UAsset a, string name, FunctionExport target, bool before)
    {
        var f = a.Exports.OfType<FunctionExport>().Single(e => e.ObjectName.ToString() == name);
        // Event stubs hand execution to the unchanged ubergraph. Refuse an
        // unexpected direct jump layout instead of guessing offset fixups.
        if (CampPlacement.Flatten(f.ScriptBytecode).Any(e => e is EX_Jump or EX_JumpIfNot or EX_Skip))
            throw new InvalidDataException("Hook stub has direct jumps: " + name);
        var call = new EX_LocalFinalFunction { StackNode = FPackageIndex.FromExport(a.Exports.IndexOf(target)), Parameters = Array.Empty<KismetExpression>() };
        var code = f.ScriptBytecode.ToList();
        if (before) code.Insert(0, call); else code.Insert(code.FindIndex(e => e is EX_Return), call);
        f.ScriptBytecode = code.ToArray(); f.ScriptBytecodeRaw = null;
        using var stream = new MemoryStream(); using var writer = new AssetBinaryWriter(stream, a);
        f.ScriptBytecodeSize = code.Sum(e => ExpressionSerializer.WriteExpression(e, writer));
        f.CreateBeforeSerializationDependencies.Add(call.StackNode);
    }
    internal static void Rebuild(UAsset a) => a.DependsMap = a.Exports.Select(e => e.SerializationBeforeSerializationDependencies.Concat(e.CreateBeforeSerializationDependencies)
        .Concat(e.SerializationBeforeCreateDependencies).Concat(e.CreateBeforeCreateDependencies).Select(p => p.Index).Distinct().ToArray()).ToList();
    internal static void Validate(UAsset a, string service)
    {
        FunctionLoadContract.ValidateOwned(a);
        var functions = a.Exports.OfType<FunctionExport>().ToDictionary(f => f.ObjectName.ToString());
        foreach (string name in new[] { Execute, Collect, Confirm, Settle, Clear, Payment, Mark, Cancel, "OnTransactionExecuted" })
            if (!functions.ContainsKey(name)) throw new InvalidDataException("Missing upgrade gateway function.");
        FunctionLoadContract.Validate(a, functions["OnTransactionExecuted"]);
        var all = functions.Where(f => f.Key.StartsWith("Rebalance") || f.Key == "OnTransactionExecuted").SelectMany(f => CampPlacement.Flatten(f.Value.ScriptBytecode)).ToArray();
        var charges = all.OfType<EX_FinalFunction>().Where(c => c.StackNode.IsImport() && c.StackNode.ToImport(a).ObjectName.ToString() == "Deduct").ToArray();
        if (charges.Length != 1 || charges[0].Parameters.Length != 2 || charges[0].Parameters[1] is not EX_IntConst cost || cost.Value != 1
            || !CampPlacement.Flatten(functions[Settle].ScriptBytecode).Contains(charges[0])) throw new InvalidDataException("Fee must be one native currency unit in settlement only.");
        string[] NativeCalls(string name) => CampPlacement.Flatten(functions[name].ScriptBytecode).OfType<EX_FinalFunction>().Where(c => c.StackNode.IsImport()).Select(c => c.StackNode.ToImport(a).ObjectName.ToString()).ToArray();
        if (!NativeCalls(Execute).Contains("HasPrice") || !NativeCalls(Execute).Contains("CanExecute") || !NativeCalls(Execute).Contains("IsCloned")
            || !NativeCalls(Execute).Contains("TryExecute") || !NativeCalls(Confirm).Contains("OnDecisionMade")) throw new InvalidDataException("Native upgrade preflight/decision path missing.");
        var confirmed = CampPlacement.Flatten(functions["InstDecisionContent"].ScriptBytecode).OfType<EX_BindDelegate>().Where(d => d.FunctionName.ToString() == Confirm).ToArray();
        if (confirmed.Length != 1 || CampPlacement.Flatten(functions["InstDecisionContent"].ScriptBytecode).OfType<EX_BindDelegate>().Any(d => d.FunctionName.ToString() == "OnDecisionMade"))
            throw new InvalidDataException("Unique decision can bypass payment preflight.");
        foreach (var p in a.Exports.OfType<PropertyExport>().Where(p => p.OuterIndex.ToExport(a) is ClassExport && p.ObjectName.ToString().StartsWith("Rebalance")))
            if (p.Property is UBoolProperty b && (b.ElementSize != 1 || !b.NativeBool)) throw new InvalidDataException("State Boolean must serialize a one-byte native size.");
        var proof = CampPlacement.Flatten(functions[Settle].ScriptBytecode).OfType<EX_StructMemberContext>().Select(p => p.StructMemberExpression.Old.ToImport(a).ObjectName.ToString()).ToArray();
        string member = service == "Uniquesmith" ? "ItemId" : service == "Powersmith" ? "ItemPower" : "bHasNetherite";
        if (proof.Count(p => p == member) < 2) throw new InvalidDataException("Fee lacks before/after native item proof.");
        string CallName(KismetExpression e) => e is EX_Context c ? CallName(c.ContextExpression)
            : e is EX_FinalFunction f && f.StackNode.IsImport() ? f.StackNode.ToImport(a).ObjectName.ToString() : "";
        bool HasGuard(string fn, string name) => functions[fn].ScriptBytecode.OfType<EX_JumpIfNot>().Any(j =>
            CampPlacement.Flatten(new[] { j.BooleanExpression }).Any(e => CallName(e) == name));
        foreach (string fn in new[] { Execute, Confirm, Settle, Payment })
            if (!HasGuard(fn, "HasAuthority")) throw new InvalidDataException("Upgrade/payment must be authority guarded.");
        if (!functions[Execute].ScriptBytecode.OfType<EX_JumpIfNot>().Any(j => j.BooleanExpression is EX_LocalFinalFunction f
                && f.StackNode.ToExport(a) == functions[TestUpgradeAffordability.Name])
            || !HasGuard(Confirm, "Balance") || !HasGuard(Settle, "Balance")) throw new InvalidDataException("Funds preflight/confirmation/settlement guard missing.");
        if (!functions[Execute].ScriptBytecode.OfType<EX_JumpIfNot>().Any(j => j.BooleanExpression is EX_CallMath n
            && n.StackNode.ToImport(a).ObjectName.ToString() == "Not_PreBool" && n.Parameters.Length == 1 && CallName(n.Parameters[0]) == "HasPrice"))
            throw new InvalidDataException("Unknown native price must reject execution.");
        var settlement = functions[Settle].ScriptBytecode;
        int deduction = Array.FindIndex(settlement, e => e is EX_Context c && c.ContextExpression == charges[0]);
        bool Cleared(KismetExpression e) => e is EX_LetObj l && l.VariableExpression is EX_InstanceVariable v
            && v.Variable.Old.ToExport(a).ObjectName.ToString() == "RebalancePendingTransaction" && l.AssignmentExpression is EX_NoObject;
        if (deduction < 2 || !Cleared(settlement[deduction - 2])) throw new InvalidDataException("Pending transaction must clear before deduction to prevent replay.");
        // The proof branch's success jumps to the charge block; its false path
        // returns or tries a native held Powersmith slot. No direct free charge.
        var proofBranches = settlement.OfType<EX_JumpIfNot>().Where(j => CampPlacement.Flatten(new[] { j.BooleanExpression }).OfType<EX_StructMemberContext>().Any()).ToArray();
        if (proofBranches.Length != (service == "Powersmith" ? 2 : 1)) throw new InvalidDataException("Mutation proof must guard fee execution.");
        foreach (string hook in new[] { "OnDecisionToBeMade", "CloseDecision", "OnBoundMerchant" })
            if (functions[hook].ScriptBytecode[0] is not EX_LocalFinalFunction) throw new InvalidDataException("Native decision lifecycle hook missing.");
    }
    internal static void SelfTest(string source)
    {
        Action<UAsset>[] corruptions = {
            a => CampPlacement.Flatten(a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Settle).ScriptBytecode)
                .OfType<EX_FinalFunction>().Where(c => c.StackNode.IsImport()).Single(c => c.StackNode.ToImport(a).ObjectName.ToString() == "Deduct").Parameters[1] = new EX_IntConst { Value = 0 },
            a => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Execute).ScriptBytecode.OfType<EX_JumpIfNot>()
                .Single(j => j.BooleanExpression is EX_LocalFinalFunction f && f.StackNode.ToExport(a).ObjectName.ToString() == TestUpgradeAffordability.Name).BooleanExpression = new EX_True(),
            a => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Confirm).ScriptBytecode.OfType<EX_JumpIfNot>()
                .Single(j => CampPlacement.Flatten(new[] { j.BooleanExpression }).OfType<EX_FinalFunction>().Any(f => f.StackNode.IsImport() && f.StackNode.ToImport(a).ObjectName.ToString() == "Balance")).BooleanExpression = new EX_True(),
            a => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Settle).ScriptBytecode.OfType<EX_LetObj>()
                .Single(l => l.AssignmentExpression is EX_NoObject && l.VariableExpression is EX_InstanceVariable v && v.Variable.Old.ToExport(a).ObjectName.ToString() == "RebalancePendingTransaction").AssignmentExpression = new EX_Self(),
            a => a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Settle).ScriptBytecode.OfType<EX_JumpIfNot>()
                .Single(j => CampPlacement.Flatten(new[] { j.BooleanExpression }).OfType<EX_StructMemberContext>().Any()).BooleanExpression = new EX_True(),
            a => CampPlacement.Flatten(a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "InstDecisionContent").ScriptBytecode)
                .OfType<EX_BindDelegate>().Single(d => d.FunctionName.ToString() == Confirm).FunctionName = new FName(a, "OnDecisionMade"),
            a => ((UBoolProperty)a.Exports.OfType<PropertyExport>().Single(p => p.ObjectName.ToString() == "RebalanceAwaitingChoice").Property).ElementSize = 0
        };
        foreach (var corrupt in corruptions) {
            var screen = MerchantScreens.Prepare(source, true)[0]; corrupt(screen.Asset);
            bool rejected = false; try { Validate(screen.Asset, screen.Service); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Unsafe upgrade/payment graph accepted.");
        }
        Console.WriteLine("Seven native upgrade/payment rejection checks passed; no gameplay functions invoked.");
    }
    static void VerifyContracts()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NativeUpgradeContracts.json")!;
        using var document = JsonDocument.Parse(stream);
        var classes = document.RootElement.GetProperty("Classes").EnumerateArray().ToDictionary(c => c.GetProperty("Name").GetString()!);
        void Field(string owner, string name, string type, int size) {
            var f = classes[owner].GetProperty("Fields").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == name);
            if (f.GetProperty("Type").GetString() != type || f.GetProperty("Size").GetInt32() != size) throw new InvalidDataException("Native field contract changed: " + owner + "." + name);
        }
        Field("MerchantBase", "mPlayerCharacterOwner", "ObjectProperty", 8); Field("MerchantBase", "mMerchantCurrencyComponent", "ObjectProperty", 8);
        Field("MerchantBaseWidget", "mMerchant", "ObjectProperty", 8); Field("InventoryItem", "Item", "StructProperty", 120);
        Field("InventoryItemSlot", "Item", "ObjectProperty", 8); Field("InventoryItemData", "ItemPower", "FloatProperty", 4);
        foreach (var pair in new[] { ("MerchantTransactionBase", "TryExecute", 1), ("MerchantTransactionBase", "HasPrice", 1), ("InventoryItem", "IsCloned", 1),
            ("MerchantCurrencyComponent", "GetCurrencyItemId", 20), ("WalletComponent", "Deduct", 24), ("MerchantBaseWidget", "OnDecisionMade", 120) }) {
            var f = classes[pair.Item1].GetProperty("Functions").EnumerateArray().Single(f => f.GetProperty("Name").GetString() == pair.Item2);
            if (f.GetProperty("ParameterSize").GetInt32() != pair.Item3 || (f.GetProperty("Flags").GetUInt32() & 0x40) != 0) throw new InvalidDataException("Native call contract changed: " + pair.Item1 + "." + pair.Item2);
        }
    }
}
