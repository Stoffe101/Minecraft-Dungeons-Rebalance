# Camp native upgrade test v6

October 7, 2026. v5 crashed and is withdrawn. v6 corrects that identified loading assertion and enables a guarded native upgrade prototype. Minecraft Dungeons is unavailable in this workspace: loading, interaction, mutation, currency persistence and repeat use are not gameplay verified. The complete Rebalance design remains unfinished.

## Crash finding and correction

The supplied CrashContext(8) reports an Assert after 36 seconds: `Unsupported UBoolProperty ReturnValue size 0`, at PropertyBool.cpp line 89. This is a different failure from v3's child-field loading access violation. The dump's KERNELBASE exception is the assertion reporting mechanism; it does not locate a gameplay transaction fault. Sanitized hashes and assertion: [crash summary](research/camp-v5-crash-summary.json). Raw machine paths, XML and minidump are not committed.

The v5 generator constructed its new affordability Boolean ReturnValue with NativeBool true and ElementSize zero. UAssetAPI writes this size into the BoolProperty contract. UE4.22 SetBoolSize supports 1, 2, 4 and 8, and rejects zero. Retail merchant Boolean declarations and the same-owner QoL property factory use a one-byte native Boolean. This was an implementation error that parsed asset round trips did not detect.

Every new Boolean now explicitly serializes ElementSize 1 and NativeBool true. Validation also checks contiguous function parameters, return/output flags and local construction. These checks adapt QoL's FunctionLayoutContracts and retain v4's native Function/Default__Function creation prerequisites. The actual retained v5 screen is rejected by the new size check; v3's retained chest is still rejected by the archetype check. The specific serialized assertion is corrected; runtime crash resolution remains unconfirmed until v6 loads in game.

## Upgrade implementation

| Service | Controlled action | Native transaction | Temporary fee |
| --- | --- | --- | --- |
| Uniquesmith | Upgrade | UniqueCollectItem | 1 emerald |
| Powersmith | UpgradeItem | UpgradeTowerItem | 1 emerald |
| Powersmith | CollectItem | CollectItem | No additional fee |
| Gildsmith | Upgrade | GildItem | 1 gold |

Eight actual action widget exports, counting live widgets and cooked templates, become native UMG Buttons. Each has a TextBlock child and native ButtonSlot, retains its parent panel slot where present, and clears then binds one OnClicked delegate each time content opens. The original Tower free-action widgets' transaction and nested WidgetTree bindings are removed from those eight reachable controls. Three passive Powersmith views retain their native transaction bindings for presentation. Original Tower packages are not overwritten.

Button callbacks find the owned merchant root by class, visibility and matching valid owning player. The root uses the captured GetTransactionByClass and GetSelectionByClass interfaces; selection comes from SelectInventorySlotItem/SelectInventorySlot and its physical InventoryItemSlot. Full native 120-byte item records are copied without reconstructing their unreflected 32-byte region. Tower-cloned items are rejected.

Execution requires a valid merchant/player, player authority, a valid transaction, native HasPrice false, native CanExecute true, a physical item and a balance of at least one. Existing native prices are rejected, not combined with the test fee. The native TryExecute Boolean result is stored explicitly. The prototype does not override GetPrice, manufacture merchant pricing, force native eligibility or directly write save files.

For actual currency IDs, the root enumerates native BlacksmithMerchant or PiglinMerchant state actors, matches mPlayerCharacterOwner and reads that merchant's mMerchantCurrencyComponent.GetCurrencyItemId. SerializableItemId has unreflected bytes; no hand-built currency struct or guessed MerchantDef map key is used. If the provider is absent, it fails before mutation. The button asks the player to open the ordinary Camp Blacksmith/Piglin shop first. Provider creation timing and unlock requirements remain runtime questions.

Before execution, the root stores the original physical item/slot/full record and pending transaction, wallet and native currency ID. OnTransactionExecuted invokes settlement. A synchronous successful TryExecute also invokes settlement when no decision is pending. Replayed callbacks see a cleared pending transaction and cannot deduct again.

Settlement requires the pending transaction identity, authority, funds and native before/after evidence: Uniquesmith requires a changed item ID that native IsUnique recognizes; Powersmith requires the same item ID and increased power, including an item held in a native UpgraderItemSlot; Gildsmith requires bHasNetherite to change from false to true. Only then does it clear the pending transaction, call WalletComponent.Deduct with amount 1, and verify the balance fell by exactly one. Unchanged results are not charged. The separate controlled Powersmith collection button uses native CollectItem and cannot add another upgrade fee.

The stock item decision widget is retained. Its OnDecisionMade delegate is rebound to an owned wrapper that rechecks authority/funds, passes the complete selected native record to MerchantBaseWidget.OnDecisionMade, and settles only after the native result changes. Decision opening marks a pending choice; closing a pending choice cancels without a fee. The native callback order must be tested. This is not yet the agreed custom catalog picker with guaranteed complete names, icons and information for every possible outcome.

The three owned actors have both one-transaction flags disabled in upgrade-test mode; the owned root defaults bCloseAfterTransaction false. This permits repeat-use/collection testing but does not establish that native definitions permit repeated Camp transactions.

## Test procedure

1. Close Dungeons and remove all earlier Rebalance PAKs, especially the crashed v5. Install only `MinecraftDungeonsRebalance-CampUpgrade-Test-v6.pak` in `C:\XboxGames\Minecraft Dungeons\Content\Dungeons\Content\Paks\~mods`. Keep other mods that do not override these same packages. v2's accepted reward/Hunt subset is included.
2. Enter Camp in single-player. Confirm loading, all three smiths near the Gift Wrapper area, overhead names, spacing and interaction. Placement coordinates and labels are authored but unconfirmed because v5 failed before acceptance.
3. Open the ordinary Blacksmith shop once and, for gilding, the Piglin shop once. Close it, then interact with the appropriate smith. Select a spare eligible item and click its labeled action. The button displays a specific preflight failure if native Camp eligibility or selection is missing.
4. Note both currencies and item before/after. A completed upgrade should cost exactly 1 emerald, or 1 gold for gilding. Powersmith collection should add no fee. Test another item without restarting Camp, then save/reload to verify the item and fee persist together.
5. If Uniquesmith opens an outcome decision, cancel once and confirm once; cancellation must leave the item and currencies unchanged. Verify native outcomes and their presentation. Custom picker completeness is still pending.
6. If an action is rejected, record the exact text on its button. `Native smith rejected this Camp item` means native CanExecute refused the request; displaying a button does not prove native eligibility. `No changed native result yet` means the charge guard found no completed change. No new collector or command run is required for this test.

## Validation and remaining work

Fifteen private-source integration tests pass, including ten-package semantic round trips/source hash preservation and ten deliberate upgrade/payment/button corruption rejections. Selection, presentation, load, placement and affordability checks remain covered. Seventeen developer policy tests and five native-call indexing tests pass; Python policy code is not a Dungeons runtime. PAK integrity and exact unpack comparison cover all 37 entries, with 17 baseline entries retained byte-for-byte. Build/stage reports are committed separately.

The synchronous item-change guard is a prototype, not a proven atomic native purchase. Native callbacks, held-item timing, UI closure, asynchronous mutation, unexpected balance changes, and save/reload need retail acceptance; no native rollback or asynchronous settlement recovery is implemented. A balance or persistence failure must be treated as a failed test, not a complete paid service. Closing/rebinding clears pending screen state and is not a durable transaction ledger.

Next work: establish each native transaction's Camp eligibility, selection and result lifecycle from v6 gameplay; adapt native held-item/decision ordering if rejected; verify success/cancel/repeat/save/reload; then implement production prices, Common-to-Rare, gild reroll and the complete explicit Unique picker. Host-only NPCs, client UI/RPC support, shared gold, higher native Ancient encounter selection chance, mission completion gold and remaining global income coverage are still unfinished. Guaranteed Ancient spawning remains dropped by the user's decision.

PAK SHA-256: `d5dc63ec49bec17765b940c5662e3e2b97868b83d6377ece2a1b1191a78dd92b`; bytes: 6209121. Reports: [build](research/camp-upgrade-build-v6.json), [stage](research/camp-upgrade-stage-v6.json).

## Published source and CI

Source commit: `e5aec223a2956d25487309484c040f079cc69f2d`. All three workflows passed: [Upgrade policy models](https://github.com/Stoffe101/Minecraft-Dungeons-Rebalance/actions/runs/37600331384), [Native declaration reader](https://github.com/Stoffe101/Minecraft-Dungeons-Rebalance/actions/runs/37600331495), [Rebalance evidence collector](https://github.com/Stoffe101/Minecraft-Dungeons-Rebalance/actions/runs/37600331421). The evidence collector workflow's Windows `Compile isolated Camp smith asset stager` step passed. Private retail source tests ran locally; CI uses permitted fixtures and does not launch Dungeons. Full job/step metadata: [validation report](research/camp-upgrade-validation-v6.json).
