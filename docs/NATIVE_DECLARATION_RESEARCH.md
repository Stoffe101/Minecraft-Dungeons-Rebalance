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
