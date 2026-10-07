using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.FieldTypes;
using UAssetAPI.Kismet.Bytecode;
using UAssetAPI.Kismet.Bytecode.Expressions;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;

// Replace each actual Tower action widget (live + cooked template) with a
// native UMG Button. Passive native item presentation keeps its own bindings.
static class PaidUpgradeButtons
{
    const string UMG = "/Script/UMG";
    const string Initialize = "RebalanceBindUpgradeButtons";
    internal static int Add(UAsset a, string service)
    {
        var g = new SpawnGraph(a, Initialize);
        var buttonClass = g.Class(UMG, "Button");
        var textClass = g.Class(UMG, "TextBlock");
        var slotClass = g.Class(UMG, "ButtonSlot");
        var buttonDonor = a.Exports.OfType<NormalExport>().First(e => e.ClassIndex.IsImport() && e.ClassIndex.ToImport(a).ObjectName.ToString() == "Button");
        var textDonor = a.Exports.OfType<NormalExport>().First(e => e.ClassIndex.IsImport() && e.ClassIndex.ToImport(a).ObjectName.ToString() == "TextBlock");
        var slotDonor = a.Exports.OfType<NormalExport>().First(e => e.ClassIndex.IsImport() && e.ClassIndex.ToImport(a).ObjectName.ToString() == "OverlaySlot");
        var actions = service == "Powersmith" ? new[] { "UpgradeItem", "CollectItem" } : new[] { "Upgrade" };
        int count = 0;
        foreach (string action in actions) {
            var property = a.Exports.OfType<PropertyExport>().Single(e => e.OuterIndex.Index == g.Index(g.Owner).Index && e.ObjectName.ToString() == action);
            if (property.Property is not UObjectProperty type) throw new InvalidDataException("Native action class field missing.");
            type.PropertyClass = buttonClass; property.CreateBeforeSerializationDependencies.Clear(); property.CreateBeforeSerializationDependencies.Add(buttonClass);
            var widgets = a.Exports.OfType<NormalExport>().Where(e => e.ObjectName.ToString() == action && e.Data.Any(p => p.Name.ToString() == "TransactionClassPrio")).ToArray();
            if (widgets.Length != 2) throw new InvalidDataException("Expected live and cooked action widgets: " + action);
            foreach (var original in widgets) {
                var panelSlot = original.Data.SingleOrDefault(p => p.Name.ToString() == "Slot")?.Clone();
                var button = (NormalExport)buttonDonor.Clone();
                button.ObjectName = original.ObjectName; button.OuterIndex = original.OuterIndex; button.ObjectFlags = original.ObjectFlags;
                button.ClassIndex = buttonClass; button.TemplateIndex = buttonDonor.TemplateIndex;
                button.SerialSize = button.SerialOffset = 0;
                button.Data = button.Data.Where(p => p.Name.ToString() is "WidgetStyle" or "BackgroundColor").Select(p => (PropertyData)p.Clone()).ToList();
                if (panelSlot != null) button.Data.Add((PropertyData)panelSlot); button.Data.Add(new BoolPropertyData(new FName(a, "IsFocusable")) { Value = true });
                button.Data.Add(new BoolPropertyData(new FName(a, "bIsEnabled")) { Value = true });
                int at = a.Exports.IndexOf(original); a.Exports[at] = button;
                var labelWidget = (NormalExport)textDonor.Clone(); labelWidget.ObjectName = new FName(a, "Rebalance" + action + "Label" + count); labelWidget.OuterIndex = button.OuterIndex;
                labelWidget.SerialSize = labelWidget.SerialOffset = 0;
                labelWidget.Data = labelWidget.Data.Where(p => p.Name.ToString() is "Font" or "ColorAndOpacity" or "ShadowOffset" or "ShadowColorAndOpacity").Select(p => (PropertyData)p.Clone()).ToList();
                labelWidget.Data.Add(new BoolPropertyData(new FName(a, "bIsVariable")) { Value = false }); a.Exports.Add(labelWidget);
                var childSlot = (NormalExport)slotDonor.Clone(); childSlot.ObjectName = new FName(a, "Rebalance" + action + "ButtonSlot" + count);
                childSlot.OuterIndex = g.Index(button); childSlot.ClassIndex = slotClass;
                childSlot.TemplateIndex = g.Import("ButtonSlot", "Default__ButtonSlot", g.Package(UMG), UMG);
                childSlot.SerialSize = childSlot.SerialOffset = 0;
                childSlot.Data = new List<PropertyData> {
                    new ObjectPropertyData(new FName(a, "Parent")) { Value = g.Index(button) },
                    new ObjectPropertyData(new FName(a, "Content")) { Value = g.Index(labelWidget) }
                }; a.Exports.Add(childSlot);
                labelWidget.Data.Add(new ObjectPropertyData(new FName(a, "Slot")) { Value = g.Index(childSlot) });
                button.Data.Add(new ArrayPropertyData(new FName(a, "Slots")) { ArrayType = new FName(a, "ObjectProperty"),
                    Value = new PropertyData[] { new ObjectPropertyData(new FName(a, "0")) { Value = g.Index(childSlot) } } });
                Dependencies(button); Dependencies(labelWidget); Dependencies(childSlot);
                button.SerializationBeforeSerializationDependencies.Add(g.Index(childSlot));
                childSlot.CreateBeforeSerializationDependencies.Add(g.Index(labelWidget));
                labelWidget.CreateBeforeSerializationDependencies.Add(g.Index(childSlot));
                count++;
            }
            string handler = "RebalanceClick" + action;
            AddClick(a, service, action, handler, action == "CollectItem" ? PaidUpgradeGateway.Collect : PaidUpgradeGateway.Execute);
            KismetExpression Button() => g.L(property);
            g.Branch(g.Math("KismetSystemLibrary", "IsValid", Button()), "Next" + action);
            KismetExpression Delegate() => g.Member(UMG, "Button", "OnClicked", Button(), "MulticastDelegateProperty");
            g.Code.Add(new EX_ClearMulticastDelegate { DelegateToClear = Delegate() });
            g.Code.Add(new EX_AddMulticastDelegate { Delegate = Delegate(), DelegateToAdd = new EX_InstanceDelegate { FunctionName = new FName(a, handler) } });
            var text = g.Object("Label" + action, textClass);
            g.Set(text, new EX_DynamicCast { ClassPtr = textClass, Target = g.Native(UMG, "ContentWidget", "GetContent", Button()) });
            g.Branch(g.Math("KismetSystemLibrary", "IsValid", g.L(text)), "Next" + action);
            string label = action == "CollectItem" ? "Collect upgraded item" : service == "Gildsmith" ? "Gild - 1 gold" : "Upgrade - 1 emerald";
            g.Code.Add(g.Native(UMG, "TextBlock", "SetText", g.L(text), g.Math("KismetTextLibrary", "Conv_StringToText", new EX_StringConst { Value = label })));
            g.Label("Next" + action);
        }
        g.Finish(); PaidUpgradeGateway.Prefix(a, "OnOpened", g.Function, false);
        PaidUpgradeGateway.Rebuild(a); Validate(a, service); return count;
        void Dependencies(NormalExport e) {
            e.SerializationBeforeSerializationDependencies.Clear(); e.SerializationBeforeCreateDependencies.Clear(); e.CreateBeforeSerializationDependencies.Clear(); e.CreateBeforeCreateDependencies.Clear();
            e.SerializationBeforeCreateDependencies.Add(e.ClassIndex); e.SerializationBeforeCreateDependencies.Add(e.TemplateIndex); e.CreateBeforeCreateDependencies.Add(e.OuterIndex);
        }
    }
    static void AddClick(UAsset a, string service, string action, string name, string execute)
    {
        var g = new SpawnGraph(a, name);
        var rootClass = g.Import("BlueprintGeneratedClass", MerchantScreens.Screen(service) + "_C", g.Package("/Game/" + MerchantScreens.Folder + "/" + MerchantScreens.Screen(service)), "/Script/Engine");
        var screens = g.ActorArray(rootClass, "MerchantScreens"); var screen = g.Object("MerchantScreen", rootClass); var index = g.Integer("ScreenIndex");
        var textClass = g.Class(UMG, "TextBlock"); var text = g.Object("ActionLabel", textClass);
        var button = a.Exports.OfType<PropertyExport>().Single(p => p.OuterIndex.Index == g.Index(g.Owner).Index && p.ObjectName.ToString() == action);
        g.Branch(g.Math("KismetSystemLibrary", "IsValid", g.Native(UMG, "Widget", "GetOwningPlayer", new EX_Self())), "Done");
        g.Code.Add(g.Static(UMG, "WidgetBlueprintLibrary", "GetAllWidgetsOfClass", new EX_Self(), g.L(screens), new EX_ObjectConst { Value = rootClass }, new EX_False()));
        g.Set(index, new EX_IntConst { Value = 0 }); g.Label("ScreenLoop");
        g.Branch(g.Math("KismetMathLibrary", "Less_IntInt", g.L(index), g.Math("KismetArrayLibrary", "Array_Length", g.L(screens))), "Done");
        g.Code.Add(g.Math("KismetArrayLibrary", "Array_Get", g.L(screens), g.L(index), g.L(screen)));
        g.Branch(g.Math("KismetSystemLibrary", "IsValid", g.L(screen)), "NextScreen");
        g.Branch(g.Native(UMG, "Widget", "IsVisible", g.L(screen)), "NextScreen");
        g.Branch(g.Math("KismetMathLibrary", "EqualEqual_ObjectObject", g.Native(UMG, "Widget", "GetOwningPlayer", g.L(screen)),
            g.Native(UMG, "Widget", "GetOwningPlayer", new EX_Self())), "NextScreen");
        g.Code.Add(g.Context(g.L(screen), new EX_FinalFunction { StackNode = g.Fn(rootClass, execute), Parameters = Array.Empty<KismetExpression>() }));
        g.Set(text, new EX_DynamicCast { ClassPtr = textClass, Target = g.Native(UMG, "ContentWidget", "GetContent", g.L(button)) });
        g.Branch(g.Math("KismetSystemLibrary", "IsValid", g.L(text)), "Done");
        g.Code.Add(g.Native(UMG, "TextBlock", "SetText", g.L(text), g.Math("KismetTextLibrary", "Conv_StringToText",
            g.Context(g.L(screen), new EX_InstanceVariable { Variable = new KismetPropertyPointer(g.Import("StrProperty", "RebalanceUpgradeStatus", rootClass)) }))));
        g.Branch(new EX_False(), "Done");
        g.Label("NextScreen"); g.Set(index, g.Math("KismetMathLibrary", "Add_IntInt", g.L(index), new EX_IntConst { Value = 1 })); g.Branch(new EX_False(), "ScreenLoop");
        g.Label("Done"); g.Finish();
    }
    internal static void Validate(UAsset a, string service)
    {
        FunctionLoadContract.ValidateOwned(a);
        string[] names = service == "Powersmith" ? new[] { "UpgradeItem", "CollectItem" } : new[] { "Upgrade" };
        foreach (string name in names) {
            var widgets = a.Exports.OfType<NormalExport>().Where(w => w.GetType() == typeof(NormalExport) && w.ObjectName.ToString() == name).ToArray();
            if (widgets.Length != 2 || widgets.Any(w => !w.ClassIndex.IsImport() || w.ClassIndex.ToImport(a).ObjectName.ToString() != "Button"
                || w.Data.Any(p => p.Name.ToString() is "TransactionClassPrio" or "WidgetTree") || !w.Data.OfType<BoolPropertyData>().Single(p => p.Name.ToString() == "bIsEnabled").Value))
                throw new InvalidDataException("Stock free Tower action remains reachable.");
            foreach (var w in widgets) {
                var slots = w.Data.OfType<ArrayPropertyData>().Single(p => p.Name.ToString() == "Slots");
                if (slots.Value.Length != 1 || ((ObjectPropertyData)slots.Value[0]).Value.ToExport(a) is not NormalExport child || child.ClassIndex.ToImport(a).ObjectName.ToString() != "ButtonSlot"
                    || child.Data.OfType<ObjectPropertyData>().Single(p => p.Name.ToString() == "Parent").Value.ToExport(a) != w
                    || child.Data.OfType<ObjectPropertyData>().Single(p => p.Name.ToString() == "Content").Value.ToExport(a).ClassIndex.ToImport(a).ObjectName.ToString() != "TextBlock")
                    throw new InvalidDataException("Controlled Button content/slot contract invalid.");
            }
            var handler = a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "RebalanceClick" + name);
            var call = CampPlacement.Flatten(handler.ScriptBytecode).OfType<EX_FinalFunction>().SingleOrDefault(c => c.StackNode.ToImport(a).ObjectName.ToString() == (name == "CollectItem" ? PaidUpgradeGateway.Collect : PaidUpgradeGateway.Execute));
            if (call == null || call.Parameters.Length != 0) throw new InvalidDataException("Button does not call owned upgrade gateway.");
        }
        var init = a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Initialize);
        if (init.ScriptBytecode.OfType<EX_ClearMulticastDelegate>().Count() != names.Length || init.ScriptBytecode.OfType<EX_AddMulticastDelegate>().Count() != names.Length)
            throw new InvalidDataException("Button delegate must clear before binding once per open.");
        var opened = a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == "OnOpened");
        if (!opened.ScriptBytecode.OfType<EX_LocalFinalFunction>().Any(c => c.StackNode.ToExport(a) == init)) throw new InvalidDataException("Upgrade buttons not initialized when opened.");
    }
    internal static void SelfTest(string source)
    {
        var path = Path.Combine(source, "Dungeons/Content/Content_Season1/UI/Merchant/UMG_TowerMerchantArtisanContent.uasset");
        for (int mode = 0; mode < 3; mode++) {
            var a = new UAsset(path, EngineVersion.VER_UE4_22); Add(a, "Uniquesmith");
            var button = a.Exports.OfType<NormalExport>().First(w => w.GetType() == typeof(NormalExport) && w.ObjectName.ToString() == "Upgrade");
            var bind = a.Exports.OfType<FunctionExport>().Single(f => f.ObjectName.ToString() == Initialize);
            if (mode == 0) button.ClassIndex = a.Imports.Select((v, i) => (v, i)).Where(x => x.v.ObjectName.ToString() == "UMG_MerchantDelayedTransactionButton_C").Select(x => FPackageIndex.FromImport(x.i)).First();
            else if (mode == 1) button.Data.OfType<ArrayPropertyData>().Single(p => p.Name.ToString() == "Slots").Value = Array.Empty<PropertyData>();
            else bind.ScriptBytecode = bind.ScriptBytecode.Where(e => e is not EX_ClearMulticastDelegate).ToArray();
            bool rejected = false; try { Validate(a, "Uniquesmith"); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Unsafe or disconnected native action button accepted.");
        }
        Console.WriteLine("Three controlled upgrade button rejection checks passed.");
    }
}
