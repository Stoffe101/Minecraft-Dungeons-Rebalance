# Native declaration research and capture

October 6, 2026. The paid Camp smith implementation remains blocked by current native transaction/currency/selected-result contracts. The supplied cooked packages establish callers and native class references, not those native implementations. No additional copy of the existing asset collection is needed.

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

The external process handle is exactly 0x0410 (query + read); imported native APIs are OpenProcess, ReadProcessMemory and CloseHandle. No game/save functions are invoked. Read work is bounded by time, byte count, call count, section size, name length, object counts and field-chain length/cycle checks. Only non-executable initialized writable/readable image data is scanned for candidates; PE/data bytes and process addresses are never exported.

Only 18 allowlisted native classes are collected, covering the profile controls, inventory/slots/stash/wallet, Tower definitions/utilities and transaction classes. Instance wallet/item/save values are not collected. Direct class properties and functions are recorded with type/target, flags, sizes and offsets; inheritance is named but not flattened. Container subtype decoding and native bodies are not implemented. Missing classes are explicitly reported.

Name tables must pass entry/index/name controls. Object tables must pass index/class/package checks. Four getter shapes infer a unique header position and relative NumParms location; all six contracts then validate qualified return types, sizes, counts and input direction. Unsupported, inaccessible, inconsistent or ambiguous data yields an incomplete report. It does not substitute offsets when discovery fails.

## Tests and actual status

Eighteen local checks passed through the official checksum-verified SDK 8.0.415 Roslyn compiler and .NET 8.0.21 runtime. The normal Linux SDK command fails in this workspace while retrieving process-start information; direct compiler execution works. Fixtures include complete discovery at two flag-header positions and two relative layouts, and deliberate ambiguous headers, cycles, wrong return targets, wrong owners/counts/input directions, name/object identity corruption and read bounds.

The dedicated Windows workflow builds the normal project, adds a self-process WinAPI marker read, checks PowerShell syntax, checks no-game incomplete reporting and confirms an existing report is preserved. Its result must be recorded after the run. CI has no Dungeons process and cannot validate native upgrade behavior.

**No Dungeons capture has been performed.** `Completed=true` means declaration collection and independent control validation only. UpgradeSemanticsVerified remains false. Native services, Camp spawn/dispatch, paid persistent mutation, Common-to-Rare, gild reroll, Powersmith rules and the in-game Unique picker are still unfinished. The latest gameplay PAK remains v2, unchanged.

## Next work and real dependency

Run the separately compiled collector against the user's running game at Camp and inspect its report, including an incomplete report if access/layout discovery fails. This environment has no Windows Dungeons runtime; it cannot obtain those live declarations itself. This is a new native-declaration capture, not a repeat cooked asset upload. The withdrawn UE4SS loader must remain disabled.

Then resolve exact selected-result/currency operations and native item preservation/commit behavior. Declarations alone may be insufficient: native transaction behavior still needs controlled saved-item and balance verification. Implement the native adapter and names/icons/details picker, connect Camp interaction/dispatch, and test repeatability, cancellation, insufficient funds, stale items, successful charges and reload persistence before packing enabled services. Shared gold and increased Ancient encounter selection remain separate unfinished native-authority work. No guaranteed Ancient minimum or unagreed chance multiplier is introduced.
