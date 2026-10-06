# Drive game-copy audit — October 6, 2026

## Available source and inspection scope

Authenticated folder traversal reached 51 descendant folders and listed 215 descendant files with no listing errors or 100-item truncation. Root manifests were read separately. Both appxmanifest.xml and MicrosoftGame.config identify Microsoft.Lovika version **1.17.0.0**, x64; the latter names Dungeons/Binaries/Win64/Dungeons.exe as the game entry point. That executable is absent from the copied binary folder.

The game Paks folder lists 47 top-level game archives totaling 4,836,730,663 bytes. The separate mods folder contains the user's QoL v7, Boss Tower, Unlock All Cosmetics and Blueprint Loader PAKs. This is the uploaded snapshot, not proof of what is currently installed. Mod source/assets were not adopted or merged during this audit.

No game-specific native DLL, PDB, generated headers, object dump or native reflection output is in the traversed copy. XGamingRuntimeThunks.dll and engine/third-party libraries do not provide the missing Dungeons native merchant/selection implementations.

**The file inventory is audited; all archive contents are not.** Eight selected small archives were downloaded, size checked, hashed and mounted by the existing UE4.22/CUE4Parse collector. They expose 622 distinct file paths. Four packages and two JSON files were read successfully; all ten preserved source hashes were verified. [Sanitized audit report](research/drive-audit.json) records archive hashes and scope, without Drive IDs, sharing links, account data or game source contents.

## What the source reads establish

| Input | Finding |
| --- | --- |
| BP_GoldChest_Rare / BP_GoldChest_Small | All four uasset/uexp files match the earlier retail collection byte-for-byte. This validates their source provenance for the existing reward patch. |
| BP_AH_DoorLarge / BP_AH_DoorLarge_Inner | Zero exported Blueprint functions; component/default definitions only. No encounter roll/count control exposed. |
| Spider Cave JSON files | Successful raw JSON reads demonstrate direct data extraction. These ordinary level files were not patched or treated as Hunt chance controls. |
| Remaining mounted contents | Mostly textures, Ancient icons, audio, maps and decorative packages. Catalog presence alone does not establish item-family mappings or probability semantics. |

The vanilla gold ranges and existing v2 changes remain as documented. No newly inspected file supplies a verified paid Camp transaction, selected-Unique mutation, shared gold award or higher Ancient selection probability implementation.

## Transfer limits

The Drive connector rejected pakchunk0 (1,191,543,250 bytes) with HTTP 413 and a stated 268,435,456-byte limit. Pakchunk4 (1,324,896,477 bytes) also exceeds that limit; no successful content read is claimed for either. Three medium archives were fetched to authenticated file references, but local transfer was blocked with HTTP 403; they were not mounted or counted as inspected contents. The eight successfully materialized small archives are the entire mounted scope of this pass.

The older u4pak reader rejected the original version-8 footers; the existing CUE4Parse collector mounted them successfully without rewriting the originals. No browser/login fallback or protection change was attempted. The direct Drive source will support further asset extraction when a supported transfer route for the necessary archive is available. Merely listing a large archive is not an integrity/content verification.

## Related runtime evidence and withdrawn diagnostic

Inspected the supplied QoL reflection-evidence-20261006-011355-8ef156.zip: SHA-256 e9afa27ac62c30f45d1aa98778e929085288b4c6d39ee7408e9e2db96ea4c51d. It contains REPORT.json and a 180-byte UE4SS.log only. The report says captureCompleted=false; all four expected headers, the object dump and all five named native class declarations are missing. The log reports console creation, UE4SS 3.0.1 and its build configuration, then ends. It does not identify the faulting native function.

The related QoL project withdrew that diagnostic after the user's immediate startup crash. Rebalance now also withdraws its UE4SS recommendation and disables its source-only Lua probe by default. No loader was bundled or installed by Rebalance. The disabled Lua flag cannot prevent UE4SS's own earlier native startup crash; it is not a compatibility repair. Do not repeat installation or guess signatures/settings on the user's retail game.

Three Lua source/API-shim checks pass, including a new assertion that the withdrawn default does not register keys, query objects or queue work. Historical enabled-script behavior is tested only in a local shim. No real game or native loader test occurred.

## Current result and next work

The Drive copy is useful for independent source extraction and source comparison. It has not yet resolved native payment/item mutation, item-family choice, authoritative Ancient probability/count or party gold authority. The existing v2 PAK is unchanged; the user has not yet tested it.

Continue identifying required archive ownership and inspecting relevant cooked graphs/defaults through a supported transfer route. For native-only behavior, establish a compatible developer diagnostic/bridge before invoking functions. Do not use an imported function name, broad rune eligibility or a UI label as proof of a working native service. Keep the higher-chance goal (no guaranteed minimum), paid Camp upgrade prices and explicit Unique choice requirements unchanged.
