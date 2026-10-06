# Private Camp smith asset stage

Build with .NET 8, then run:

```text
dotnet build tools/CampSmithStager -c Release
dotnet tools/CampSmithStager/bin/Release/net8.0/CampSmithStager.dll <private-PatchSources-root> <fresh-private-output-root>
```

The tool creates separate Rebalance copies of the three native Tower NPC actors and their three native content widgets and three owned root screens. It relocates self and cross-package names into `/Game/Mods/MinecraftDungeonsRebalance/Camp`, preserves native merchant definitions and visuals, binds each actor to its owned root, changes the root GetSoftContentWidget override to dispatch to its corresponding content, preserves all other native decision/input graphs, adds three captured native Unique presentation functions, writes and reopens every package, checks exact parsed semantic equality and verifies original source hashes.

All original inputs are checked before writing; failure removes the newly created stage. Output cannot overlap the input tree or overwrite an existing directory. Raw source/generated game assets must remain private and outside git.

**This is an asset foundation, not a playable Camp smith build.** The original one-transaction flags remain true. Native transaction definitions still use Tower implementations; only actor/screen/content routing is replaced. There is no Camp spawn binding, paid persistent transaction, custom Unique selector or adapter to the Python policy model. The report explicitly marks those features false. The gameplay PAK builder does not include these clones. Do not package them as a completed upgrade feature.

Integration tests require the seven original private packages and their companions:

```text
python tests/test_camp_smiths.py --dotnet <dotnet> --stager <CampSmithStager.dll> --source <private-PatchSources-root> -v
```


The cloned Uniquesmith content widget now contains `RebalanceUniqueName`, `RebalanceUniqueDescription` and `RebalanceUniqueIcon`. Each takes the native SerializableItemId and returns localized FText or Texture2D through the captured ItemFunctionLibrary method. The selected contract manifest is embedded at build time. Function maps, parameters, declaring owners, bytecode and preload dependencies are checked before write and after reopen. These helpers are not yet connected to a visible choice list.

Read-only graph rejection checks against an original private Artisan widget:

```text
dotnet CampSmithStager.dll --self-test-presentation <original-Artisan.uasset>
```

These seven checks deliberately invalidate generated graphs in memory; they execute no native gameplay calls and write no assets.


All three cloned content widgets also contain `RebalanceSelectedSlotItem(InventoryItemSlot)` and `RebalanceSelectedItemData(InventoryItem)`. These read the physical slot's existing object and its full 120-byte native record through captured UProperty names. They do not recreate gear, mutate the record or save anything. Return construction, owning classes, context skip offsets and preload bindings are checked before writing and after reopening. Four deliberate rejection checks run with `--self-test-selection <original-Artisan.uasset>`.

MerchantScreens validates exact self relocation, actor soft-class fields, typed content dispatch and byte-for-byte retention of root function graphs. The stock UMG_MerchantItemDecision reference and owning-player creation/selection return path are retained. These have not been run in retail. `--self-test-screens <private-PatchSources-root>` rejects three broken routes/graphs without gameplay calls or asset writes.
