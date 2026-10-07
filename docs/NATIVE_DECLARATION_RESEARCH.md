# Native declaration research and capture

October 6, 2026. The latest retail capture succeeded. Native presentation, selected-result and wallet method declarations are available; inherited transaction/struct detail and native behavior remain unresolved. The supplied cooked packages establish callers and native class references, not those native implementations. No additional copy of the existing asset collection is needed.

## Research completed

The same-owner Minecraft-Dungeons-QoL source at `bc996fe` contains `HeroProfileCallContracts.cs` and profile-call tests grounded in its newer retail evidence. These provide six independent caller shapes. A cloud profile Guid identifies a profile in that flow; it is not proof of stable physical item identity or clone-stable hero identity.

| Native owner | Method | Inputs | Result |
| --- | --- | --- | --- |
| PlayerCharacterSaveSlot | GetCloudPlayerId | None | /Script/CoreUObject.Guid |
| PlayerControllerBase | GetRecentSaveDataIndex | None | int32 |
| PlayerControllerBase | GetNumProfiles | None | int32 |
| PlayerControllerBase | GetSaveLocalUserNum | None | int32 |
| PlayerControllerBase | GetAvailableSaveDataByIndex | int32 | /Script/Dungeons.CharacterSaveData object |
| PlayerControllerBase | GetCharacterSlotByIndex | int32, bool | /Script/Dungeons.PlayerCharacterSaveSlot object |

Inspected [MCD-PE](https://github.com/Minecraforever/MCD-PE/tree/be646dcd82a689e24709b7abd4cff30fb60b7c9f), commit `be646dcd82a689e24709b7abd4cff30fb60b7c9f`, root Apache-2.0 license. Its README describes Steam final-build reverse-engineering and an older behavior-source mirror. `Restored_Tower.h` supplies names/save/config research, not current native smith transaction bodies. `Restored_ItemStashComponent.h` describes inventory slots, SalvageItemInSlot and a void SerializeSaveState write operation; it does not supply a current selected-Unique/payment API or permanent item-ID guarantee. These source declarations cannot certify this Store build's ABI. No reconstructed game source, executable, protection tooling or addresses were copied or used.

Inspected [UEDumper](https://github.com/Spuckwaffel/UEDumper/tree/5b2b5264a66aa9edb28619c5ff654b16d3b9e038), commit `5b2b5264a66aa9edb28619c5ff654b16d3b9e038`, root MIT license, as a layout reference only. Its legacy name path uses 16,384-entry chunks and a UE4.22 ANSI string at entry offset 0xC. UObject/UField/property and chunked object-table layouts inform candidates. Its generic UStruct model has version/chain/tail ambiguity; a fixed UFunction header is not established for Dungeons. The new reader considers two relative header shapes and only accepts one uniquely selected by independent controls. Stock ALL_ACCESS/live-editor behavior and placeholder globals are not reused. No third-party dumper source was copied, built, run or bundled.

Microsoft's primary [ReadProcessMemory documentation](https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory) establishes exact readable-range behavior and the VM_READ requirement. [Process access rights](https://learn.microsoft.com/en-us/windows/win32/procthread/process-security-and-access-rights) distinguishes query/read rights from write, operation, thread and protected-process rights. Access denial is a stopping condition; there is no elevation, driver or protection workaround.

## Implemented research tool

`tools/NativeContractReader` is project-authored C#/.NET 8 with no package dependencies. `scripts/Capture-NativeContracts.ps1` runs its checks, captures a fresh report and packages logs plus reports. The compiled bundle does not install anything into the game.

The external process handle is exactly 0x0410 (query + read); imported native APIs are OpenProcess, ReadProcessMemory, VirtualQueryEx and CloseHandle. No game/save functions are invoked. Read work is bounded by time, byte count, call count, section size, name length, object counts and field-chain length/cycle checks. Only non-executable initialized writable/readable image data is scanned for candidates; PE/data bytes and process addresses are never exported.

Only 18 allowlisted native classes are collected, covering the profile controls, inventory/slots/stash/wallet, Tower definitions/utilities and transaction classes. Instance wallet/item/save values are not collected. Direct class properties and functions are recorded with type/target, flags, sizes and offsets; inheritance is named but not flattened. Container subtype decoding and native bodies are not implemented. Missing classes are explicitly reported.

Name tables must pass entry/index/name controls. Object tables must pass index/class/package checks. Four getter shapes infer a unique header position and relative NumParms location; all six contracts then validate qualified return types, sizes, counts and input direction. Unsupported, inaccessible, inconsistent or ambiguous data yields an incomplete report. It does not substitute offsets when discovery fails.

## Tests and actual status

Eighteen local checks passed through the official checksum-verified SDK 8.0.415 Roslyn compiler and .NET 8.0.21 runtime. The normal Linux SDK command fails in this workspace while retrieving process-start information; direct compiler execution works. Fixtures include complete discovery at two flag-header positions and two relative layouts, and deliberate ambiguous headers, cycles, wrong return targets, wrong owners/counts/input directions, name/object identity corruption and read bounds.

The dedicated Windows workflow builds the normal project, adds a self-process WinAPI marker read, checks PowerShell syntax, checks no-game incomplete reporting and confirms an existing report is preserved. Run 37468028504 passed at 2630d8732cbcc25828b18e87e7c16a5946525032, including execution of the unpacked runner and verification of its incomplete capture ZIP. The build had zero warnings/errors and all 19 Windows checks passed. The initial run 37467815494 passed those checks but failed the workflow epilogue due to a retained intentional negative-test exit code; that CI handling was corrected. CI has no Dungeons process and cannot validate native upgrade behavior.

**Two Dungeons captures have been received; neither produced accepted native declarations.** `Completed=true` means declaration collection and independent control validation only. UpgradeSemanticsVerified remains false. Native services, Camp spawn/dispatch, paid persistent mutation, Common-to-Rare, gild reroll, Powersmith rules and the in-game Unique picker are still unfinished. The latest gameplay PAK remains v2, unchanged.

## Next work and real dependency

Run the separately compiled collector against the user's running game at Camp and inspect its report, including an incomplete report if access/layout discovery fails. This environment has no Windows Dungeons runtime; it cannot obtain those live declarations itself. This is a new native-declaration capture, not a repeat cooked asset upload. The withdrawn UE4SS loader must remain disabled.

Then resolve exact selected-result/currency operations and native item preservation/commit behavior. Declarations alone may be insufficient: native transaction behavior still needs controlled saved-item and balance verification. Implement the native adapter and names/icons/details picker, connect Camp interaction/dispatch, and test repeatability, cancellation, insufficient funds, stale items, successful charges and reload persistence before packing enabled services. Shared gold and increased Ancient encounter selection remain separate unfinished native-authority work. No guaranteed Ancient minimum or unagreed chance multiplier is introduced.


## Native collector Windows validation — October 6, 2026

At source commit 2630d8732cbcc25828b18e87e7c16a5946525032, Native declaration reader run 37468028504 passed: normal Windows .NET build (zero warnings/errors), all 19 synthetic/self-process checks, PowerShell parsing, no-game incomplete-report behavior, existing-report preservation, compiled ZIP preparation and execution of the unpacked capture runner from paths containing spaces. The runner's failure ZIP contained NativeContracts.json, REPORT.json, Capture.log and SelfTest.log. Upgrade policy run 37468028478 and existing evidence tooling run 37468028467 also passed. None has a Dungeons runtime.

The downloadable compiled research bundle is Minecraft-Dungeons-Rebalance-NativeCollector.zip, 109,031 bytes, SHA-256 f6e3803ee5e9f8564274dbc7bb18b1b790ad795fed6681b2577f1a3fdc11e642. DLL SHA-256 bc5ab21c4b76a580e0a0b50ccc809beeca970c3edf6c6171a36b02c8ad14b74e. Downloaded Actions wrapper digest matched GitHub's advertised checksum; the nested bundle includes the runner, configuration and compiled tools, with no game-owned assets/PAK. The capture bundle is research tooling, not a new mod release. Dungeons compatibility and native transaction semantics remain unverified.

Next required input: one new declaration capture with Dungeons running at Camp, returned even if incomplete. Instructions are in tools/NativeContractReader/README.md. Do not repeat the existing asset collection or re-enable the withdrawn UE4SS loader. Paid Camp services and Unique choice remain unfinished; delivered gameplay v2 and its hash are unchanged.


## First retail native capture and discovery correction — October 6

Received native-contracts-20261006-131039.zip. All 19 original self-tests passed. The game query/read handle and PE/data reads succeeded: 6,345,728 bytes, nine reads. Collection stopped at “Too many name-table candidates” before validating any name table; zero classes/control contracts were collected and no game functions/writes occurred. This is a collector discovery-filter failure, not evidence of denied process access. Sanitized result: research/native-capture-20261006.json.

Corrected discovery to use cached, sorted VirtualQueryEx region queries and filter committed readable non-executable data before the existing 200,000 candidate bound. Complete candidate headers must fit their region. Retained byte/read/time bounds; added a 4,096-query bound and 1,000,000 raw-entry workspace bound. No new process rights or protection fallback. Reports include counts and at most eight distinct rejected object-layout reasons, without process addresses or game values. Twenty-one local checks now pass, including >210,000 numeric-noise candidates, memory protection states and boundary rejection; Windows adds its own-process read/query check. Native upgrade work remains blocked on an actual declaration capture. A new corrected capture is needed; no repeat cooked asset collection is requested.


## Corrected collector Windows result — October 6

At d55fe531e12a84a39d87407d4942b89656ba17a5, Windows native-reader workflow 37469426624 passed normal compilation, all 22 checks (including own-process VirtualQueryEx/ReadProcessMemory), PowerShell parsing, report preservation and execution/archive checks for the unpacked bundle. This validates the filter implementation; the corrected collector has not yet run on Dungeons.

NativeCollector-v2.zip: 113,119 bytes; SHA-256 8068ef6a006f39f0dd389bcc531eda4e3a86bce43fce988588e0dc3019c592f0. DLL SHA-256 16b03d99f63ddc0cbe22f15a9c88faad7cbdd4e40cde8e7d7efd497c3d80d36c. Verified the downloaded Actions wrapper against its advertised digest before extracting the new bundle. No gameplay PAK or paid upgrade is enabled. Next: run the corrected collector from its separate extracted folder with Dungeons at Camp and inspect the new report.


## Second retail capture and source-grounded layout correction — October 6

Received native-contracts-20261006-132200.zip. v2 filtering succeeded: 211,959 raw candidates reduced to 153,213 mapped-data candidates; 7,571,532 bytes, 153,233 reads, 1,054 region queries. No name table validated and no class declarations were collected. Sanitized report: research/native-capture-20261006-v2.json.

Read the related QoL native-favorites-20261006-132111-71d697.zip privately: revision legacy-names-256-v2 progressed past name validation but reported zero object-array shape candidates. It contains no accepted declarations. Reviewed latest same-owner QoL e535cab78a11999329ff3ae5d5c5608dd7986f07, including reserved-object capacity correction, as a reuse reference. Reviewed Epic-authored UE4.22.3 NameTypes.h and UObjectArray.h at 99a530d4ccbe6bea1e8f49df20acfeb294006962 through the GitHub connector. The engine name-array maximum/chunk size implies 256 pointer slots. Object preallocation rounds its reservation to an extra chunk; 2 Mi elements can reserve 33 chunks (2,162,688 slots), which the original max-capacity guard rejected. Reserved capacity is independent of live-object work.

Implemented inline and pointer-backed name tables, 128/256 capacities, reserved name chunks, bounded ANSI/UTF-16 text, and independently verified child/target layouts. Native function header selection remains controlled by four getters, and all six profile contracts must match. Object capacity now permits up to 4 Mi reserved slots/64 chunks while actual traversal remains bounded by the unchanged 1M live-object limit. The old 500K call ceiling was insufficient for two header candidates plus that live walk; now 2M calls, with the original 128 MiB/60-second and 4,096-query limits retained. No process rights, game calls, writes or protection fallback added.

Forty-nine local synthetic checks pass, including 24 inline/pointer/name/child/target combinations, distinct valid table ambiguity, 256-capacity tables with 33 reserved object chunks, and inconsistent-capacity rejection. The first compile caught a local variable/constructor-parameter name collision; renamed the parameter before testing. These fixtures validate declaration consistency and rejection behavior, not retail upgrade ABI or payment semantics. Windows validation and a new compiled capture bundle follow. Native Camp upgrade integration remains unfinished; v2 gameplay PAK unchanged.


## Native collector v3 Windows validation — October 6

At 6ab37c672d1cde522ce1997fcbddaa0e5aab10b0, native-reader Windows run 37471659764 passed normal compilation (zero warnings/errors), all 50 checks, PowerShell parsing, fresh/incomplete/report-preservation behavior and execution of the unpacked ZIP runner. Upgrade policy run 37471659806 and existing tooling run 37471659602 also passed. Retail v3 declaration discovery remains untested.

NativeCollector-v3.zip: 118,244 bytes; SHA-256 9b21fc20fd416a76939662a9da381b99be6afba9f589be549b5b0b76fe3fb252. DLL SHA-256 a78ea33df90e54cc41a6daa935a0d39bb2b951312d7c5fb5a6fb4b7a476e6783. The downloaded Actions wrapper matched its advertised digest before extraction. Compiled bundle saved for private testing. Next: capture once with this version at Camp, then use accepted declarations (or precise failure stage) to continue native transaction work. No native paid upgrade or new gameplay PAK is claimed.


## Successful retail declarations and native presentation — October 6

Received native-contracts-20261006-133711.zip. Completed=true, all six controls passed, all 18 allowlisted classes present, no runner issues. Accepted layout: name capacity 256, characters +12, children +0x48, property target +0x70, function header +0x98, parameter-count delta 4. Read 51,841,974 bytes in 1,292,194 calls and 1,045 region queries. Capture invoked no gameplay function and wrote no game memory. All 50 Windows self-tests passed. Sanitized summary: research/native-capture-20261006-v3.json; selected declarations and input SHA-256: research/native-upgrade-contracts-v1.json. Historical failed-capture sections above describe earlier attempts. A repeat of the same v3 capture is not needed.

Native ItemFunctionLibrary provides SerializableItemId-based name, description and Texture2D icon calls. MerchantTransactionBase exposes OnTransactionDecisionMade(InventoryItemData), CanExecute() and TryExecute(). WalletComponent.Balance returns int32; Deduct has no return value. GildItem, UniqueCollectItem and UpgradeTowerItem declare no direct functions and inherit InventoryItemSlotTransactionBase, which this capture did not collect. The selected-result declaration does not establish native outcome generation, transaction lifetime, payment override, mutation or persistent Camp behavior. InventoryItem.TryUpgradeItem must not be assumed to implement the agreed rarity/power policy from its name.

CampSmithStager now embeds the selected captured contract manifest and adds RebalanceUniqueName, RebalanceUniqueDescription and RebalanceUniqueIcon to the cloned Uniquesmith content widget. Each accepts the native item ID and calls the exact captured static native owner. Added function fields, ownership/function maps, native property archetypes and preload dependencies are validated before writing and after reopening. Original graphs and six source package pairs are retained; the three functions do not execute paid upgrades. Full choice list, native family enumeration, confirmation, Camp spawning/dispatch, persistent payment and shared gold remain unfinished.

Five private-source integration tests pass, including seven deliberate graph/signature/preload rejection checks. All six packages write/reopen with exact expected parsed equality; exactly three functions are added. Initial compilation caught an ArrayDim enum assignment; first round-trip caught nonserialized ElementSize/Next fields. Corrected both and reran successfully. SDK Roslyn compiled locally; one existing Newtonsoft net6/net8 reference-unification warning remains in this direct-compiler path. Report: research/camp-smith-presentation-stage.json. No Unreal/gameplay execution occurred.

Reviewed related QoL native-favorites-20261006-133827-9942ba.zip: incomplete at the 2M call budget, empty declarations. It supplies no missing superclass/struct contracts. Reused the same-owner QoL graph/property/preload construction approach at the user's explicit request; no third-party unlicensed source or game-owned assets published.

Next work: obtain the missing InventoryItemSlotTransactionBase/MerchantSubobjectBase/MerchantDef and InventoryItemData/SerializableItemId/MerchantDisplayPrice/EnchantmentData declarations, including nested array/enum types; trace native outcome enumeration and chosen-result handling; wire the presentation functions into an owned picker and Camp screen dispatch; establish persistent item/currency transaction behavior before gameplay acceptance. No new collector or NPC PAK is delivered in this increment.

Existing upgrade policy tests (17) and native call indexing tests (5) pass after this increment. Windows normal compilation subsequently passed at implementation 5f5b0a3 (workflow 37474277976, zero warnings/errors); no retail presentation/upgrade test is claimed.


## Native selected-item readers and targeted dependencies — October 6

Privately read related QoL native-favorites-20261006-134806-1f6922.zip: reader legacy-batched-slots-v5 completed, no issues, 11 declarations, 1,667,667 reads/49,019,979 bytes/3,345 ms. Native InventoryItemData fields include ItemId, ItemPower, Enchantments (EnchantmentData array), ArmorProperties (ArmorPropertyData array), Rarity, upgrade/gift/modified flags, modification count and Netherite fields. SerializableItemId has a SerializedId FName; this is not evidence of a permanent physical item identifier. Inventory slots contain native InventoryItemSlot objects. Selected sanitized records: research/qol-item-record-declarations.json. The capture lacks field sizes/offsets, enum identities and merchant transaction parent classes. Reviewed QoL main 24d3d39c1c58e1f28ee5b806a48c1b291e3c18a6, including bounded nested array declaration decoding.

Added two native read functions to each of the three isolated smith content widgets: RebalanceSelectedSlotItem and RebalanceSelectedItemData. They read the accepted InventoryItemSlot.Item and InventoryItem.Item fields through exact declaring-owner UProperty imports. The second reads the full native 120-byte item record, preserving unknown fields rather than reconstructing gear from a display name. Native callers/transaction lifecycle and persistence remain unimplemented. No mutation, currency change or save call added.

Six private asset integration tests pass, including the prior seven presentation rejection checks and four new read-graph rejection checks (missing construction flag, invalid skip offset, wrong result pointer, wrong native field). All six package pairs write/reopen exactly; six selection readers and three presentation functions are added, original functions and inputs retained. The normal Windows build follows publication. Existing 17 upgrade-model and five call-index tests also pass. No game execution is available in this workspace.

Expanded NativeContractReader's bounded target set by 11 exact dependencies: three parent classes (InventoryItemSlotTransactionBase, MerchantSubobjectBase, MerchantDef) and eight structs (InventoryItemData, SerializableItemId, MerchantDisplayPrice, EnchantmentData, ArmorPropertyData, ProblemStatus, InventoryItemMetaData, TowerFloorItemUpgrades). ScriptStruct identity is reported explicitly, and array elements decode recursively with a 16-node bound. Native array-inner owning-object equality is not assumed; that relation was not established by accepted retail evidence. New missing-dependency/coverage fields are separate from control-validation success. No process rights or byte/call/time/query bounds changed. Fifty-four local collector checks pass; Windows adds its own-process check. New cases cover target-kind separation, inherited merchant class names, nested native structs, null/non-property/cyclic array targets.

Dependency capture v4 is needed because the successful original capture deliberately omitted these classes/structs. Do not rerun v3 or repeat the cooked asset collection. Next: inspect these added declarations, implement native outcome enumeration/selected-result and verified paid transaction lifecycle, then connect the picker and Camp dispatch. A new gameplay PAK is not ready; economy/Hunts v2 remains the latest playable build.


## Enum dependencies in collector v4

Read Epic-authored UE4.22.3 Class.h, UnrealType.h and EnumProperty.h at folgerwang/UnrealEngine 99a530d4ccbe6bea1e8f49df20acfeb294006962 through GitHub. UEnumProperty stores its underlying numeric UProperty followed by the UEnum pointer; UEnum stores FString CppType followed by the Names array of FName/int64 pairs. Using the previously accepted UField/base-property layouts, v4 now records enum target identities/underlying numeric types and referenced Dungeons enum symbol/value pairs. Values are retained directly; indices are not treated as values. FName number is retained separately. Names arrays are bounded (512 live entries, 4096 capacity), symbols must be distinct, and the header is checked again after reading. No engine source is copied or bundled. This extended layout remains a source-grounded candidate until the new retail capture validates its metadata.

Fifty-seven local collector checks pass, adding sparse-value preservation, duplicate-symbol and oversized-array rejection. Windows adds its own-process test (expected 58). The full native record/enum dependency capture is intended to avoid another follow-up merely to obtain rarity/currency enum identities. Existing original controls and process rights/budgets are unchanged.

Primary source links: https://github.com/folgerwang/UnrealEngine/blob/99a530d4ccbe6bea1e8f49df20acfeb294006962/Engine/Source/Runtime/CoreUObject/Public/UObject/EnumProperty.h and https://github.com/folgerwang/UnrealEngine/blob/99a530d4ccbe6bea1e8f49df20acfeb294006962/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h .


## Dependency collector v4 Windows and bundle validation

Source ebecf590f3fd055db8697e4dc7444441d7e16c9a: native reader workflow 37476889616 passed normal Windows compilation (zero warnings/errors), all 58 synthetic/self-process checks, PowerShell parse/incomplete/report preservation and unpacked runner/archive checks. Upgrade policy workflow 37476889740 and evidence tooling workflow 37476889618 passed. Prior selected-item implementation ff74774352f72e5c8ed1e6fea8521962565b8ab1 also passed all three workflows (37476198325, 37476198286, 37476198223), including normal CampSmithStager compilation. Six private asset integration tests and their eleven deliberate graph-rejection cases passed locally; CI lacks retail input assets.

Downloaded artifact 11419715783 and matched its outer SHA-256 6d3df97af71a5b8464bd3887cd737dafdd9e3f7b04f79924f501c13434e7dc05 before extraction. NativeCollector-v4.zip is 124,625 bytes, nine entries, SHA-256 696d52d3eeff3ad42bff8a3a984a2fc2ed48204051c16a2069f20b5e226a5a4d; DLL SHA-256 7c3b00513caec5c8c49f341be5bc57f03eb15775812674e6865404448c77155a. Both ZIP integrity checks passed, and bundle README includes the new dependency and enum scope. Compiled research bundle made available for private testing; no game assets or loader included. This is not a gameplay PAK.

Required next input: one v4 dependency capture with Dungeons at Camp, using the same PowerShell command from a fresh extracted v4 folder. The original successful v3 capture is retained/accepted. Newly targeted inherited transaction APIs, full record field dimensions and enum identities/values are the remaining declaration input; native payment/mutation/save behavior then still requires implementation and gameplay acceptance. No paid upgrade or shared-gold completion is claimed.

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

## October 7 continuation: existing capture used for v6

No new capture required. Extended the selected manifest from the already complete 145714 archive with native HasPrice/IsCloned, item ID validation/comparison, MerchantBase player/currency fields and MerchantItemSlotBase HasItem/GetItem. PaidUpgradeGateway now uses native transactions and complete item/ID records; PaidUpgradeButtons connects controlled UMG actions. v5's crash came from an authored size-zero Boolean, independently identified by the supplied assertion and retail/QoL size-one declarations. Full research, design decisions and runtime limits: [CAMP_UPGRADE_TEST_V6.md](CAMP_UPGRADE_TEST_V6.md).
