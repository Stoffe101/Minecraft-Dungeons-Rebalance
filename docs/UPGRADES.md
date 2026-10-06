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

## Runtime investigation status

The historical UE4SS reflection probe is withdrawn and disabled after the related QoL startup crash. Do not install or re-enable it on the user's game. See NATIVE_RUNTIME.md for the superseding compatibility requirements.

## Native asset implementation — October 6

CampSmithStager now creates isolated copies of all three native NPC actors and their content widgets, with exact parsed write/reopen checks and original source preservation. Four integration checks pass against the real original assets. This supplies reusable native cooked assets for the Camp implementation, but no Camp spawn/dispatch, paid transaction or custom Unique picker is connected. See CAMP_SMITH_IMPLEMENTATION.md for precise native UI bindings and remaining work.

## Tests and next work

Seventeen upgrade policy tests pass with a synthetic adapter: explicit second-result selection, complete presentation rows, eligibility, family/ownership/item/catalog/balance revalidation, fixed prices, expiration, duplicate confirmation, and ambiguous/failure receipt handling. These tests prove source-model behavior only, not native mutation or payment.

Next: establish a compatible native transaction route and current selected-result and currency contracts; trace native item-family/name/icon/stat presentation; implement a native bridge and native UMG picker; add Camp placement; then verify real saved items, currency and co-op. Existing v2 remains the latest gameplay test PAK and contains no upgrade service.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## October 6 successful native capture continuation

Capture native-contracts-20261006-133711.zip validates all six control signatures and contains all 18 requested classes. Selected contracts are preserved in research/native-upgrade-contracts-v1.json. CampSmithStager now adds native item-ID name/description/icon presentation functions to the isolated Uniquesmith widget; these helpers have no visible choice list or native adapter yet. Details, tests and remaining superclass/struct/payment dependencies: [CAMP_SMITH_IMPLEMENTATION.md](CAMP_SMITH_IMPLEMENTATION.md).

WalletComponent.Deduct returns void, and the three smith classes inherit their selection behavior from the uncaptured InventoryItemSlotTransactionBase. OnTransactionDecisionMade accepts a full InventoryItemData, but outcome validation/payment/save behavior is unverified. Do not infer an atomic paid upgrade from these declarations or use TryUpgradeItem's name as a rarity contract. Native services remain disabled until those behaviors are implemented and tested. Existing v2 is the last playable PAK.


## Selected-item native integration — October 6

Added native physical-slot/item and full-record read functions to all three cloned smith screens, validated against captured InventoryItemSlot.Item and InventoryItem.Item declarations. Six private asset tests pass (11 deliberate graph rejection cases); no currency/item/save mutation is enabled. The newer QoL capture succeeded and confirms item-record field names/nested array types, but lacks transaction parent classes and field sizes/offsets.

Prepared collector v4 with 11 additional exact parent/struct targets and bounded nested array declarations; 54 local checks pass. Full paid Camp services still require that added capture and native behavior validation. No repeat v3 or asset collection is required. Detailed findings, source pins and next work: [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md). Gameplay PAK remains v2.

## October 6 v4 capture accepted

All 29 requested native types and 16 enum declarations captured successfully. Camp staging now rejects changed native record shapes and sparse enum values; six real-source integration tests include 15 rejection checks. Native pricing/selection dependencies revealed by this capture are being resolved through bounded parent/struct closure. No paid upgrade, picker, Camp placement or shared-gold runtime is enabled. See CAMP_SMITH_IMPLEMENTATION.md and NATIVE_DECLARATION_RESEARCH.md for exact findings, tests and next work. Retail acceptance still requires actual payment, selected outcome, item-state preservation, repeated upgrades and save/reload tests.

## V5 integration and collector stop

The user supplied the successful v5 capture: 53 types/26 enums, requested dependency closure complete. No further capture requested. Camp actor CDOs now point to owned merchant roots whose GetSoftContentWidget overrides open the matching cloned content; all other native decision/input graphs are retained. Seven asset integration tests pass, including 18 negative graph/record/dispatch checks. This is configuration/graph integration, not runtime payment or save validation. No new gameplay PAK is released. Pricing customization and normal-inventory upgrade behavior need an exposed implementation route or a validated native runtime/toolchain before paid services and world placement can be completed. See CAMP_SMITH_IMPLEMENTATION.md.
