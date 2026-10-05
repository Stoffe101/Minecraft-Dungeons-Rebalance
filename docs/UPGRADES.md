# Camp upgrades and choosing a Unique

Updated October 5, 2026, after the supplemental collection.

## Implemented source foundation

`src/rebalance/upgrades.py` implements developer-side upgrade policy and choice/confirmation models. **Python is not a Minecraft Dungeons runtime and this module is not loaded by the PAK.** No native adapter is connected. Camp NPC placement, interactive UMG picker, actual item mutations and charges remain unfinished.

The foundation supports Common -> Rare (750 emeralds), Rare -> selected Unique (2,500 emeralds), gilding (150 gold), and gild reroll (250 gold). Powersmith pricing is still TBD and has no fabricated service. Native eligibility must explicitly permit a service; rarity alone cannot make an artifact gildable.

The new Unique requirement is an explicit purchase choice, not an instruction to roll randomly. Candidate outcomes must come from the current authoritative native family/catalog. No mapping is inferred from `_Unique1`/`_Unique2` filenames, and no third-party item names/icons are copied into a runtime database.

## Intended native interface

| Area | Behavior |
| --- | --- |
| Inventory selection | Select one persistent item; capture its physical identity and revision |
| Variant list | Scrollable rows with native localized name, native icon, brief description and innate effects |
| Detail panel | Selected variant, native stat preview where available, and current item comparison |
| Preserved state | Show only verified preservation of power/enchantments/gilding; never claim a preview effect that native mutation cannot guarantee |
| Price | Display the agreed currency/cost before confirming; no payment for browsing |
| Multiple outcomes | Player explicitly chooses one valid native family member |
| One outcome | Show the result and request confirmation |
| Input | Mouse selection and controller/keyboard focus; scrolling keeps the selected row visible |
| Confirm | Exact selected outcome, item and price are bound to one confirmation token |
| Cancel / close | No mutation or charge |
| Unavailable service | Disable purchase with a clear player-facing reason; keep diagnostic implementation details in logs |

`Quote.choice_rows()` supplies the names/icons/descriptions/effects/stat rows for a future native UMG binding. It does **not** implement that UMG screen itself. Actual names/icons/stat computation remain a native presentation-provider dependency; tests use clearly synthetic IDs and assets.

## Transaction boundary

The core checks ownership, inventory location, item revision/state fingerprint, native catalog revision, native eligible services, selected family member, price and wallet balance. Confirmation expires after 120 seconds; a duplicate confirmation returns the same receipt. Changing its choice is rejected. A changed/deleted/replaced item or outcome catalog requires a new confirmation.

The adapter must perform native charge-and-mutation atomically, rechecking state inside that transaction. It must retain the existing item, apply the exact selected Unique type, preserve intended state, save/notify natively and deduplicate its request ID. Failed mutation must not charge. The model does not manufacture a refund or recreate an item from a save record.

Native capability flags default false. Preview data may be modeled, but buying is blocked until selected-result support, persistent inventory support, item-state preservation, atomic charge/mutation and receipt deduplication are verified. Adapter exceptions or mismatched receipts retain a pending token and block a second mutation; recovery needs native receipt reconciliation rather than blind retries.

The core is intended to run on a single serialized native-authority queue. It is not a thread-safe network server or a persistence layer. Tokens/receipts are currently session-memory models; reconnect/restart recovery requires a native implementation before shipping.

## Direct native findings

The Tower Artisan's provided Blueprint opens the native inventory selection view for the owning controlled player. Its merchant slot/transaction behavior points to native UniqueCollectItem. The actual imports include InventoryItem, ItemStashComponent, MerchantTransactionUtil, TowerMerchantUtil and selected-slot transaction classes. The raw cooked assets do not include the native method bodies, current native class property defaults, or a proven selected-variant/atomic-cost contract. An inherited widget and a native class name do not establish that persistent Camp upgrades work.

Native widgets already reference UMG_InventoryItemIcon and UMG_InventoryInspectorItemIcon. Reuse those native presentation paths after tracing their item-type and inspector bindings. Do not substitute a static external icon collection or guessed family database.

## Research tooling

`runtime/NativeContracts/Scripts/main.lua` is a **developer-only, not-installed** UE4SS reflection inventory. It uses documented StaticFindObject, UStruct function/property iteration, function flags, a key callback and ExecuteInGameThread. F8 schedules a bounded names/flags scan of 23 native classes directly observed in the provided imports. No native upgrade/transaction is called; no hook, item/currency setter, actor spawn or save modification is included.

Primary API references:

- https://docs.ue4ss.com/lua-api/global-functions/staticfindobject.html
- https://docs.ue4ss.com/lua-api/classes/ustruct.html
- https://docs.ue4ss.com/lua-api/classes/ufunction.html
- https://docs.ue4ss.com/lua-api/classes/property.html
- https://docs.ue4ss.com/lua-api/global-functions/registerkeybind.html
- https://docs.ue4ss.com/lua-api/global-functions/executeingamethread.html

Two Lua 5.4 API-shim tests pass (missing-loader disablement and bounded read-only inventory). This does not verify UE4SS installation, wrapper behavior or compatibility with this Store build. The script does not produce offsets or parameter-direction metadata and cannot by itself certify an ABI. No new loader dependency is enabled or bundled into the gameplay PAK. Check exact-build loader compatibility before considering a runtime probe.

## Tests and next work

Seventeen upgrade policy tests pass with a synthetic adapter: explicit second-result selection, complete presentation rows, eligibility, family/ownership/item/catalog/balance revalidation, fixed prices, expiration, duplicate confirmation, and ambiguous/failure receipt handling. These tests prove source-model behavior only, not native mutation or payment.

Next: verify native runtime diagnostic compatibility; obtain current selected-result and currency transaction contracts; trace native item-family/name/icon/stat presentation; implement a native bridge and native UMG picker; add Camp placement; then verify real saved items, currency and co-op. Existing v2 remains the latest gameplay test PAK and contains no upgrade service.
