# Native integration gate

**Latest runtime result: v3 crashes and is withdrawn. v4 restores generated Function archetypes/creation prerequisites and passes eleven private-source checks plus 37-entry PAK verification; this correction is retail-unverified. Prices and paid upgrades remain unfinished. Use v2 as the known reward baseline. See [CAMP_PLACEMENT_TEST.md](CAMP_PLACEMENT_TEST.md). Earlier milestone notes below are historical.**

**Current milestone (October 6): CampPlacement-Test-v3 packages a host-only, non-interactive Camp spawn preview, retaining v2 economy/Hunt changes. Ten package pairs and nine integration tests pass; all 37 PAK entries integrity-test and unpack-compare exactly. Paid upgrades/picker/persistence/shared gold remain unfinished and no retail placement result exists. See [CAMP_PLACEMENT_TEST.md](CAMP_PLACEMENT_TEST.md). Earlier sections below record earlier passes.**

## What this pass established

`scripts/index_native_calls.py` indexes all 39 received cooked-package metadata reports: 247 observed native calls. The generated [table](research/native-calls.md) and [JSON](research/native-calls.json) retain import candidates, enclosing export/statement locations, receiver expressions, serialized arguments, return-property references and source metadata SHA-256 values. It rejects collector errors, broken reference chains and conflicting duplicate packages. It retains ambiguous function-name imports rather than choosing a class by guesswork.

Twenty-two calls contain serializer pointer/name error markers. Their JSON entries set `hasUnresolvedMetadata=true`. None of the entries certifies a native signature. Argument counts include serialized caller expressions, potentially including out parameters.

## Concrete call paths

| Integration | Actual evidence | Still required |
| --- | --- | --- |
| Tower merchant UI | BP_PlayerController calls TowerMerchantUtil.OpenTowerMerchant with a controller expression and byte value 0 or 1 | Native enum meaning, Camp support, actor association and persistent inventory semantics |
| Transaction feedback | Mission offering problem widget calls MerchantTransactionBase.QueryProblemStatus with a local hasProblem expression | Parameter direction; atomic payment and item mutation implementation; selected Unique result support |
| Mission request | UMG_TransactionAncientMobChances.RefreshAncientMobs calls MissionRequestUtil.CreateMissionRequest with player, byte 0, MissionSelection, integer 0 and offerings expressions | Current native types/flags and authoritative launch/generation handoff; this caller is a preview refresh |
| Probability UI | MissionChancesUtil.GetMissionProbabilities consumes MissionState | Actual authoritative encounter reservation/selection, not display probabilities |
| Currency display | BPL_Currency uses ItemFunctionLibrary.BreakItemId | Wallet writer and server authority; this is display data, not a payment API |
| Pickup presentation | Gold storable graphs include WalkPickupComponent.ResetPickup | Award owner, persistence and duplicate protection for host/client gold grants |

Do not use the byte/integer literals above to infer undocumented enum values or costs. Neither CreateMissionRequest nor OpenTowerMerchant is invoked by the Rebalance runtime source.

## Runtime evidence needed

The source-only 32-class UE4SS probe is **withdrawn and disabled by default** after the related QoL startup crash. The supplied reflection ZIP has no generated headers or completed capture. Three local Lua shim checks pass, including default disable; none proves game compatibility or fixes native loader startup. No loader DLL or installer is provided. Do not install/re-enable UE4SS for this probe on the user's game.

Current native contracts remain a dependency. The Drive copy gives direct asset access, but has no game executable, game-specific native DLL, symbols or runtime reflection output. Eight archived source mounts and six targeted reads succeeded; the two additional Ancient door Blueprints contain no functions. See [DRIVE_GAME_RESEARCH.md](DRIVE_GAME_RESEARCH.md) for exact inspected scope and transfer limits.

A future diagnostic/bridge must first establish compatibility in a developer environment. Imported names, generated offsets from another build and synthetic models cannot certify the user's native ABI or transaction semantics. The prior F8/Ctrl+H installation/test recommendation is withdrawn. Do not repeat loader installation or guess engine/signature settings.

Official references inspected: [dumpers](https://docs.ue4ss.com/feature-overview/dumpers.html), [UE4SS](https://docs.ue4ss.com/). The release documentation warns that generated memory layout is not accurate, that force-loading can crash the game, and that game-specific compatibility work may be necessary. Development-only jmap features are not assumed available in a release.

## Implementation after reflection

1. Establish native item family/presentation and selected-result conversion contracts, with persistent inventory ownership and preservation of item state.
2. Implement a native authority adapter with payment and mutation committed atomically, or a demonstrated recoverable transaction. Validate failure, duplicate confirmation and reconnect cases before exposing paid Camp services.
3. Build the native UMG names/icons/details picker and bind its explicit selection to that adapter; place the three native NPCs in Camp once their services work there.
4. Identify and tune authoritative native encounter probability/count to increase the chance of one or more Ancients. The user withdrew the guaranteed minimum on October 6. Compare matched offerings across runs and verify multiplayer agreement; retain gold rooms.
5. Establish native gold award authority and idempotency; implement party distribution and completion reward through it.
6. Measure income, Prospector, salvage, loot variant coverage and Gold Room generation in retail; tune against the agreed targets and complete the full test matrix.

The current v2 PAK remains a partial Hunts/Economy test build. None of the outstanding services is marked implemented by this research pass.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## First retail native capture and discovery correction — October 6

Received native-contracts-20261006-131039.zip. All 19 original self-tests passed. The game query/read handle and PE/data reads succeeded: 6,345,728 bytes, nine reads. Collection stopped at “Too many name-table candidates” before validating any name table; zero classes/control contracts were collected and no game functions/writes occurred. This is a collector discovery-filter failure, not evidence of denied process access. Sanitized result: research/native-capture-20261006.json.

Corrected discovery to use cached, sorted VirtualQueryEx region queries and filter committed readable non-executable data before the existing 200,000 candidate bound. Complete candidate headers must fit their region. Retained byte/read/time bounds; added a 4,096-query bound and 1,000,000 raw-entry workspace bound. No new process rights or protection fallback. Reports include counts and at most eight distinct rejected object-layout reasons, without process addresses or game values. Twenty-one local checks now pass, including >210,000 numeric-noise candidates, memory protection states and boundary rejection; Windows adds its own-process read/query check. Native upgrade work remains blocked on an actual declaration capture. A new corrected capture is needed; no repeat cooked asset collection is requested.


## October 6 successful native capture continuation

Capture native-contracts-20261006-133711.zip validates all six control signatures and contains all 18 requested classes. Selected contracts are preserved in research/native-upgrade-contracts-v1.json. CampSmithStager now adds native item-ID name/description/icon presentation functions to the isolated Uniquesmith widget; these helpers have no visible choice list or native adapter yet. Details, tests and remaining superclass/struct/payment dependencies: [CAMP_SMITH_IMPLEMENTATION.md](CAMP_SMITH_IMPLEMENTATION.md).

WalletComponent.Deduct returns void, and the three smith classes inherit their selection behavior from the uncaptured InventoryItemSlotTransactionBase. OnTransactionDecisionMade accepts a full InventoryItemData, but outcome validation/payment/save behavior is unverified. Do not infer an atomic paid upgrade from these declarations or use TryUpgradeItem's name as a rarity contract. Native services remain disabled until those behaviors are implemented and tested. Existing v2 is the last playable PAK.


## Selected-item native integration — October 6

Added native physical-slot/item and full-record read functions to all three cloned smith screens, validated against captured InventoryItemSlot.Item and InventoryItem.Item declarations. Six private asset tests pass (11 deliberate graph rejection cases); no currency/item/save mutation is enabled. The newer QoL capture succeeded and confirms item-record field names/nested array types, but lacks transaction parent classes and field sizes/offsets.

Prepared collector v4 with 11 additional exact parent/struct targets and bounded nested array declarations; 54 local checks pass. Full paid Camp services still require that added capture and native behavior validation. No repeat v3 or asset collection is required. Detailed findings, source pins and next work: [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md). Gameplay PAK remains v2.

## October 6 v4 capture accepted

All 29 requested native types and 16 enum declarations captured successfully. Camp staging now rejects changed native record shapes and sparse enum values; six real-source integration tests include 15 rejection checks. Native pricing/selection dependencies revealed by this capture are being resolved through bounded parent/struct closure. No paid upgrade, picker, Camp placement or shared-gold runtime is enabled. See CAMP_SMITH_IMPLEMENTATION.md and NATIVE_DECLARATION_RESEARCH.md for exact findings, tests and next work. Retail acceptance still requires actual payment, selected outcome, item-state preservation, repeated upgrades and save/reload tests.

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
