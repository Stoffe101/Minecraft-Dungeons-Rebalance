# Camp smith implementation

**Current milestone (October 6): CampPlacement-Test-v3 packages a host-only, non-interactive Camp spawn preview, retaining v2 economy/Hunt changes. Ten package pairs and nine integration tests pass; all 37 PAK entries integrity-test and unpack-compare exactly. Paid upgrades/picker/persistence/shared gold remain unfinished and no retail placement result exists. See [CAMP_PLACEMENT_TEST.md](CAMP_PLACEMENT_TEST.md). Earlier sections below record earlier passes.**

Updated October 6, 2026. Camp smiths are the user's highest implementation priority.

## Implemented asset foundation

`tools/CampSmithStager` now stages three separate native NPC actor copies and three separate native content widget copies from the already supplied private original packages. All six are relocated into `/Game/Mods/MinecraftDungeonsRebalance/Camp`. Native definitions, meshes, animation references, sounds, widget trees, property values and original script behavior are retained; three captured native presentation functions are added to the Uniquesmith widget; self/cross names are relocated together. Native Tower assets are not overwritten.

| Service | New actor | New content widget | Existing native transaction |
| --- | --- | --- | --- |
| Uniquesmith | BP_RebalanceCampUniquesmith | UMG_RebalanceCampUniquesmithContent | UniqueCollectItem |
| Powersmith | BP_RebalanceCampPowersmith | UMG_RebalanceCampPowersmithContent | UpgradeTowerItem |
| Gildsmith | BP_RebalanceCampGildsmith | UMG_RebalanceCampGildsmithContent | GildItem |

The local private stage contains twelve cooked files (six `.uasset`/`.uexp` pairs). [Validation report](research/camp-smith-stage.json) contains checksums and structural results, not raw game assets. Each package is written/reopened and compared with the expected relocated parsed package. Original input hashes are checked again after staging. All originals must validate before output creation; a failed stage is removed, and existing/nested output is rejected.

Four integration checks passed against the real supplied originals: six-package staging and unchanged originals, missing sources with no output, existing output preservation, and nested output rejection. These are asset-tool tests; no NPC was loaded or transaction executed in a game. The tool compiled through SDK 8.0.415's Roslyn compiler locally because this workspace's process-information restrictions prevent the normal .NET/MSBuild front ends from starting. Windows CI now builds the normal project.

## Exact native bindings traced

The actor `MerchantDef.MerchantDefinition` properties reference TowerArtisanMerchantDef, TowerBlacksmithMerchantDef and TowerGilderMerchantDef. Native merchant enum values are TowerUniquesmith, **TowerBlacksmith** and TowerGildsmith; the Powersmith's internal enum is not named TowerPowersmith. Both original one-transaction flags remain true in the copies until repeatable paid Camp transactions are verified.

The Artisan/Gilder widgets' `OnOpened` graphs get the owning player, cast to BP_PlayerController, obtain GetControlledPlayerCharacter, and pass that character to `UMG_SelectInventoryPlayerView.SetPlayerCharacter`. The Powersmith passes the owning controlled character to `UMG_MerchantEquippedGearWithPlayerView.SetPlayerCharacter`. It therefore needs additional adaptation for the agreed inventory selection flow.

Native action buttons bind **TransactionClassPrio**, not a serialized fixed price or selected-result parameter. In the original Artisan/Gilder metadata, `Upgrade` exports 3409/3410 hold UniqueCollectItem/GildItem respectively. In the Powersmith, `UpgradeItem` exports 3597/3598 hold UpgradeTowerItem. The native class imports do not supply transaction implementations. Showing a custom price in UMG would not establish that it is charged.

Camp's `lobby.json` uses region-based prop groups such as `*.*.blacksmith` and `*.*.enchantermerchant`, with native unlock alternatives. The supplied `thetower_npc/objectgroup.json` contains three complete 59-by-59 floor tiles, not three portable NPC prop objects. Adding those entire floor tiles to Camp is not an NPC placement implementation. Their baked block/resource-pack prefab mapping has not been resolved. New positions and navigation/interaction clearance remain unvalidated.

## Remaining implementation

The isolated assets are **not bound to a Camp spawner or controller dispatch**. Their native merchant types still refer to native Tower dispatch; the cloned screens are not automatically selected. There is no paid persistent-item transaction, Common-to-Rare mutation, gild reroll or native Unique variant picker yet. The Python policy model remains a separate reference implementation and is not loaded by the PAK.

The next concrete work is to establish a compatible native transaction route, including the exact selected-result item operation and currency writer, then bind Camp interactions to the isolated screens. Native family/presentation data must feed the names/icons/details picker and the explicit choice must reach the authoritative mutation. Payment, preservation, repeatability and failure handling must be verified together. Common-to-Rare costs 750 emeralds; selected Unique conversion 2,500 emeralds; gilding 150 gold; gild reroll 250 gold. Powersmith price/power rules remain undecided.

Current assets cannot establish that native Tower transactions work on persistent Camp items. No live service or new gameplay PAK is enabled by this pass; v2 remains unchanged. The withdrawn UE4SS probe remains disabled. No new collection of previously supplied sources is requested.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## Successful retail declarations and native presentation — October 6

Received native-contracts-20261006-133711.zip. Completed=true, all six controls passed, all 18 allowlisted classes present, no runner issues. Accepted layout: name capacity 256, characters +12, children +0x48, property target +0x70, function header +0x98, parameter-count delta 4. Read 51,841,974 bytes in 1,292,194 calls and 1,045 region queries. Capture invoked no gameplay function and wrote no game memory. All 50 Windows self-tests passed. Sanitized summary: research/native-capture-20261006-v3.json; selected declarations and input SHA-256: research/native-upgrade-contracts-v1.json. Historical failed-capture sections above describe earlier attempts. A repeat of the same v3 capture is not needed.

Native ItemFunctionLibrary provides SerializableItemId-based name, description and Texture2D icon calls. MerchantTransactionBase exposes OnTransactionDecisionMade(InventoryItemData), CanExecute() and TryExecute(). WalletComponent.Balance returns int32; Deduct has no return value. GildItem, UniqueCollectItem and UpgradeTowerItem declare no direct functions and inherit InventoryItemSlotTransactionBase, which this capture did not collect. The selected-result declaration does not establish native outcome generation, transaction lifetime, payment override, mutation or persistent Camp behavior. InventoryItem.TryUpgradeItem must not be assumed to implement the agreed rarity/power policy from its name.

CampSmithStager now embeds the selected captured contract manifest and adds RebalanceUniqueName, RebalanceUniqueDescription and RebalanceUniqueIcon to the cloned Uniquesmith content widget. Each accepts the native item ID and calls the exact captured static native owner. Added function fields, ownership/function maps, native property archetypes and preload dependencies are validated before writing and after reopening. Original graphs and six source package pairs are retained; the three functions do not execute paid upgrades. Full choice list, native family enumeration, confirmation, Camp spawning/dispatch, persistent payment and shared gold remain unfinished.

Five private-source integration tests pass, including seven deliberate graph/signature/preload rejection checks. All six packages write/reopen with exact expected parsed equality; exactly three functions are added. Initial compilation caught an ArrayDim enum assignment; first round-trip caught nonserialized ElementSize/Next fields. Corrected both and reran successfully. SDK Roslyn compiled locally; one existing Newtonsoft net6/net8 reference-unification warning remains in this direct-compiler path. Report: research/camp-smith-presentation-stage.json. No Unreal/gameplay execution occurred.

Reviewed related QoL native-favorites-20261006-133827-9942ba.zip: incomplete at the 2M call budget, empty declarations. It supplies no missing superclass/struct contracts. Reused the same-owner QoL graph/property/preload construction approach at the user's explicit request; no third-party unlicensed source or game-owned assets published.

Next work: obtain the missing InventoryItemSlotTransactionBase/MerchantSubobjectBase/MerchantDef and InventoryItemData/SerializableItemId/MerchantDisplayPrice/EnchantmentData declarations, including nested array/enum types; trace native outcome enumeration and chosen-result handling; wire the presentation functions into an owned picker and Camp screen dispatch; establish persistent item/currency transaction behavior before gameplay acceptance. No new collector or NPC PAK is delivered in this increment.

Existing upgrade policy tests (17) and native call indexing tests (5) pass after this increment. Windows normal compilation subsequently passed at implementation 5f5b0a3 (workflow 37474277976, zero warnings/errors); no retail presentation/upgrade test is claimed.


## Native presentation Windows validation — October 6

Implementation 5f5b0a33b9109e7f5ba0c495cd5296ced58c6818 passed evidence tooling workflow 37474277976, including normal Windows CampSmithStager compilation with its embedded captured manifest, the existing collector fixtures, economy compilation and source syntax checks. Upgrade policy workflow 37474278027 and native reader workflow 37474278115 also passed. Private game assets are not available to CI; the five Camp asset tests and seven negative graph checks were run locally against the supplied originals. These results do not certify in-game names/icons, choice UI, payment or persistent upgrades.


## Selected-item native integration — October 6

Added native physical-slot/item and full-record read functions to all three cloned smith screens, validated against captured InventoryItemSlot.Item and InventoryItem.Item declarations. Six private asset tests pass (11 deliberate graph rejection cases); no currency/item/save mutation is enabled. The newer QoL capture succeeded and confirms item-record field names/nested array types, but lacks transaction parent classes and field sizes/offsets.

Prepared collector v4 with 11 additional exact parent/struct targets and bounded nested array declarations; 54 local checks pass. Full paid Camp services still require that added capture and native behavior validation. No repeat v3 or asset collection is required. Detailed findings, source pins and next work: [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md). Gameplay PAK remains v2.

## October 6 — successful v4 retail contracts and automatic dependency closure

Input: native-contracts-20261006-142012.zip, SHA-256 23f8c58f7c46b7df653281ee9942fb445fe40de71fee3a4a17ba965fffba8d99. Completed=true, controls pass, 29/29 requested types and 16 enums; missing lists empty. Read 52,016,905 bytes in 1,352,914 calls, 1,046 region queries. Accepted function header 0x98, NumParms delta 4, children 0x48, property target 0x70, name characters 12, capacity 256. No gameplay function or game-memory write occurred. Public review: research/native-capture-20261006-v4.json; selected declarations: research/native-upgrade-contracts-v1.json. Private source capture is not committed.

Findings and decisions:

- InventoryItemData is a native 120-byte parameter record. Reflected fields leave bytes 64–95 unaccounted for; SerializableItemId.SerializedId is at offset 12 in a 20-byte record. Do not construct item/currency IDs or replace records by assigning guessed bytes. Preserve native state through verified native operations. A struct read graph does not certify preservation during mutation/save.
- Rarity Common/Rare/Unique values are 0/1/2. EnchantmentCategory is sparse (Aoe=4, Armor=8, Permanent=64), so enum indices cannot substitute for values. Native Netherite source is 3. This establishes declarations, not a chosen gilding effect or persistence.
- InventoryItemSlotTransactionBase inherits MerchantSlotTransactionBase, absent from the earlier fixed request. MerchantDisplayPrice.Pricing targets MerchantPricing (8 bytes); MerchantSubobjectBase.mMerchant targets MerchantBase; WalletComponent currency slots contain ItemSlot objects. These are unresolved declarations, not missing existing assets.
- TowerMerchantUtil.OpenTowerMerchant takes ETowerUIMerchantType, whose values are TowerComplete=0 and TowerFloorComplete=1. It is not the actor merchant enum and cannot be treated as a direct smith opener.
- WalletComponent.Deduct returns void; ClientAdd is a client RPC. No atomic payment, rollback or authoritative party-wide gold contract follows from these signatures. MerchantTransactionBase.GetPrice is a native callable, not an observed Blueprint override event; a custom price label would not establish actual charging.

Implementation: update embedded selected native contracts from this capture; gate selected-record asset generation on exact record fields, array elements and enum values. Four corruption checks reject an ItemId name offset of zero, wrong enchantment array element, enum index replacing sparse value, and truncated gilded enchantment state. Existing native read/presentation graphs remain unchanged; no fabricated records or gameplay mutation are enabled.

Collector improvement: index at most 4,096 native declaration identities during the existing object-table scan; emit only the explicit roots plus recursively referenced native structs and superclass declarations (128 maximum). Nine roots come from retail package imports: MerchantActor, MerchantBase, MerchantBaseWidget, MerchantCurrencyComponent, MerchantDefComponent, SelectInventorySlotItem, SelectMerchantSlot, UpgraderItemSlot, ItemSlot. Object instance values and unrelated declaration bodies are not exported. External engine references remain qualified without exporting their bodies. Missing roots have a separate report field; DependencyClosureComplete requires original roots, selected dependencies and merchant roots. Native function bodies and map inner types remain undecoded.

Validation: direct Roslyn collector compilation and 62 synthetic checks pass. Five new checks cover recursive parent/nested pricing records, unrelated type exclusion, inheritance cycles, wrong parent/struct kinds and declaration bound. Camp stager compilation and six private real-source integration tests pass, including 15 graph/contract rejection checks; originals preserved and six package pairs reopened. The local direct Roslyn Camp build retains its known Newtonsoft net6/net8 reference-unification warning. No retail upgrade test has occurred.

Next: validate the compiled closure collector on Windows, resolve merchant factory/outcome selection and native pricing behavior, then integrate paid Camp services and an owned Unique choice list. Continue toward a PAK after those paths are wired; the current usable PAK remains v2. Do not ask for another copy of the successful v4 capture or unchanged game assets.

## Transitive collector Windows validation and v5 bundle

At implementation commit de006824189a335033958f084ac1d98737ece46f, native reader workflow 37481190026 passed (job 112329420111): normal Windows build with zero warnings/errors, 63 checks including self-process reads, PowerShell parsing, no-game incomplete reporting, existing-report preservation, bundle preparation and packaged runner execution. Evidence workflow 37481190032 also passed (job 112329420366), including normal CampSmithStager build with zero warnings/errors. Policy workflow 37481190004 passed. These are tooling/source results; retail merchant transactions are not tested.

Artifact 11421416403 outer digest d79a12cc53980ce0170cf88e4adba8061bf9be29d9aa67d71dc8ebc51942c611 matched before extraction. NativeCollector-v5.zip: 127,660 bytes, nine entries, SHA-256 4e76ac087cff0ce2fee3175e64bc8ee5a93425aa9f2c21eea1be3127236a6ef5; DLL SHA-256 8bab5ecd7b6f1df493fdccce2badf786920d78e9ef8b8cb4c9817d6c1b92fb02. Both ZIP integrity checks pass. Bundle made available for the newly exposed merchant dependency capture; no game assets, loader or upgrades included.

Required next input: run the capture command from a fresh extracted v5 folder with Dungeons at Camp and return its output ZIP. The accepted v4 result is retained; this targets merchant/selection roots and their transitive native parent/struct dependencies. This is the concrete declaration blocker before native paid service integration. No new gameplay PAK or full-project completion is claimed.

## October 6 — v5 accepted; actor/screen/content integration

Accepted native-contracts-20261006-145714.zip: controls and dependency closure pass; all requested roots/dependencies present, 53 types and 26 enums. Read 52,772,581 bytes / 1,379,749 calls / 1,051 queries. No native gameplay calls, game writes or upgrade semantics verified. Public review: research/native-capture-20261006-v5.json. The earlier request for a v5 capture is fulfilled. **No further collector requested.**

Research resolves actual paths:

- MerchantActor.mMerchantWidgetClass is a 40-byte native SoftClassProperty. Override it in each isolated actor CDO to its owned Camp root screen.
- MerchantBaseWidget.GetSoftContentWidget is a BlueprintNativeEvent. The existing UMG_Merchant override can return the corresponding cloned smith content as a soft class, avoiding the shared BPL_Merchants dispatch.
- MerchantBaseWidget.OnDecisionToBeMade receives an array of native InventoryItemData. The original UMG_Merchant event routes this to the existing Content_Season1 UMG_MerchantItemDecision.SetItemsToDecide. Its InstDecisionContent function creates the widget with GetOwningPlayer and binds OnDecisionMade to the merchant root's inherited native handler. This is an existing item-choice path, not a need to fabricate an SDK or reconstruct item records.
- SelectInventorySlot exposes SelectInventorySlot, GetSelectableInventorySlots and GetInventorySlot; MerchantBaseWidget exposes GetSelectionByClass and GetTransactionByClass. MerchantSelectionBase exposes confirm/cancel state and actions. These declarations establish available APIs, not Tower-versus-normal-inventory behavior.
- MerchantPricing has Price (int32 offset 0) and RebateApplied (float offset 4). It is returned inside a price value, not an observed mutable transaction pricing field. MerchantTransactionBase.GetPrice/TryExecute are native callables; GetPrice is not a Blueprint override event. No supported custom pricing setter or persistent normal-inventory upgrade contract was identified in this capture. MerchantBase refers to a MerchantPricingComponent whose body was not part of this capture; absence of a setter here is not proof that all possible native extension paths are impossible.

Implementation: MerchantScreens.cs clones UMG_Merchant separately for each smith, relocates only exact self identities (avoids renaming foreign UMG_MerchantItemDecision imports), replaces only GetSoftContentWidget with a typed soft-class return, binds each actor to its own root, and validates the native event/array/screen ABI from the v5 manifest. All remaining root function bytecode is checked byte-for-byte before and after serialization; the stock choice-widget reference is required. Nine private package pairs now connect actor -> root -> content, retaining the native item-choice/input/owning-player graphs. Camp world placement, paid execution, repeatability and retail behavior are still not implemented/verified. Existing native one-transaction flags remain true.

Tests: seven real-source integration tests pass, including nine-pair semantic round-trip, original hashes, three correct content routes and three new rejection checks (wrong content route, erased decision graph, mismatched actor screen). Existing 15 graph/record rejection checks continue to pass. First direct graph JSON serialization failed because UAssetAPI FName converters need asset context; replaced it with actual serialized bytecode comparison. First actor write then caught the absent SoftObjectProperty type name; registered it before serialization. Final tests and a retained private stage pass. Direct Roslyn compilation retains the known Newtonsoft net6/net8 reference-unification warning; Windows CI is checked separately. Report: research/camp-merchant-wired-stage.json.

Decision: stop iterative declaration collection. The capture alone cannot provide native C++ behavior or a working runtime extension. A complete paid Camp-upgrade PAK needs a verified custom pricing/execution path and normal-inventory/save behavior; none is established for the current Store setup. Do not silently ship free Tower services, guessed mutations or another collector. The old loader remains withdrawn. This pass implements concrete screen integration without claiming runtime upgrade completion.

Next work: seek an existing exposed customization path or a validated native mod runtime/toolchain that can implement the agreed prices and normal-inventory preservation; then wire execution and Camp placement and run real item/balance/reload/party tests. If neither route is available, report this limitation rather than continuing capture versions. HuntsEconomy-Test-v2 remains the usable gameplay PAK.

## Native screen integration CI verification

Implementation commit 0285e2b9c8199ad4bab0fc96bd74c86c4a7e98c3 is published to main. Evidence workflow 37485292520, native reader workflow 37485292521 and policy workflow 37485292512 all succeeded. Windows evidence builds CampSmithStager normally; private retail source integration tests are the seven locally passing tests, not part of public CI. No new collector or playable PAK is delivered. The uploaded v5 capture is accepted and no further command/capture request is pending.

Additional public searches for existing Camp Tower-merchant mods and MerchantPricingComponent source did not produce an inspectable primary implementation in the returned results. Results included unrelated projects and general game pages; this search does not prove absence of an existing mod or pricing API. No unverified source was reused or recommended.
