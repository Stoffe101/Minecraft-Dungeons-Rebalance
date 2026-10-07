# Reuse and credits

This file is the reuse ledger for code, assets, logic and research imported from sibling or third-party projects.

Nothing should be redistributed from another project until its permission/license status is documented here.

---

## Minecraft-Dungeons-QoL

Repository: https://github.com/Stoffe101/Minecraft-Dungeons-QoL

Owner: Stoffe101

Planned reuse candidates:

- cooked package tooling
- graph/bytecode builders
- package validation
- inventory item resolution
- UI helper patterns
- structural regression tests

Status:

- Same owner/project family.
- Exact copied files/functions must be listed here when reuse begins.
- Rebalance must remain independently installable.

---

## Better Ancient Hunt

Author: **Onetoeisenough**

Nexus page:
https://www.nexusmods.com/minecraftdungeons/mods/173

Version observed during project bootstrap: **1.0**

Last updated on Nexus: **2024-10-03**

### Current permission text observed on Nexus

The page currently states:

- **Upload permission:** file may be uploaded to other sites if the creator is credited.
- **Modification permission:** files may be modified and improvements/bug fixes released.
- **Conversion permission:** conversion is allowed with creator credit.
- **Asset use permission:** assets may be used without permission or credit.
- Commercial/Donation Point asset use is also allowed by the page.

Author note:

> Usage must not result in harm of thinking things and must be appropriate for all audiences that Minecraft Dungeons is.

### Project policy

Even where credit is not explicitly required for asset reuse, this project will credit **Onetoeisenough**.

### Documented Better Ancient Hunt features

- longer side paths
- harder mobs including minibosses
- more mobs
- a second wave after the first Ancient containing two Ancients
- raid captains after gold and Ancient battles

### Bootstrap reuse plan (superseded by actual adaptation below)

At bootstrap this was not yet imported.

Before importing:

1. obtain the actual distributed mod file;
2. archive/checksum the source file used for research;
3. inspect changed cooked assets;
4. identify which changes are safe/relevant;
5. document exact copied/modified assets here;
6. avoid blindly inheriting excessive spawn density.

---

## Minecraft Wiki / community documentation

Used for factual research only, not copied as project code/assets.

Key topics:

- Ancient Hunt structure and gold values
- Tower merchant floor behavior
- Uniquesmith ID/behavior
- Gildsmith ID/behavior
- gilded gear rules
- rarity behavior
- Loot Urn and Camp Emerald Chest values

See SOURCES.md.


## Actual reuse in 2026-10-05 pass

Minecraft-Dungeons-QoL at `ce7002779f0b47db8986a2a230628399027f5586`: adapted LegacyEvidenceExporter Program/csproj, evidence-tool pin configuration, collector/bootstrap script and the path/process helpers from Common.ps1. Same owner requested reuse. No project-wide third-party license is inferred from that authorization. No QoL runtime assets or retail assets copied into this git repository.

Pinned UAssetAPI 1.1.0 and CUE4Parse libraries from UeBlueprintDumper 1.2.0 are tool dependencies (upstream notices retained in the portable dependency archive). LetMeMove 1.1.0 is an MIT fixture for local/CI tests, not Rebalance runtime content. Dokucraft Dungeons-Mod-Kit at `c30e88ec5e99e401eadedddbe82af0265a056fe7` supplies the MIT u4pak test packager.

Better Ancient Hunt collector-stage status: credited Onetoeisenough; permission verified October 5. No assets had been copied at that stage. The later upload and actual adaptation below supersede this status. See NATIVE_ASSET_RESEARCH.md.


## Better Ancient Hunt 1.0 actual adaptation

Original author: **Onetoeisenough**. Source PAK supplied by user from Nexus mod 173/file 378. Adapted the mob groups, default mob configuration, finite arena wave/timing changes and side-path definitions from `ancientdungeons.json` and ten `hm_*.json` files. Output is composed against the user's installed level originals. Density/count/path requests are bounded. Native routes, objectives, triggers, gates and rewards are preserved. Original singular-path st_gold entry and extra hyperpermission level are excluded pending a verified reference. No third-party native code is included; the upstream PAK contains JSON only. Raw game files/generated PAKs remain private and are not checked into git.

## Supplemental / upgrade-choice continuation

UpgradeCore and read-only Lua reflection inventory are project-authored. No native transaction ABI, game family database, item icon or SDK implementation copied. Official UE4SS API documentation was consulted; no UE4SS code/binary is bundled or enabled. QoL identity-capture/state-guard design informed this model; no QoL source function was copied in this pass.

## Camp smith staging — October 6

CampSmithStager is project-authored using pinned UAssetAPI 1.1.0. Reviewed the same-owner QoL CookedAssetRelocator name-map/write-reopen pattern as a reuse reference; no wholesale source or runtime asset from QoL was copied. Six user-supplied native game packages are cloned privately for NPC/screen reuse; no raw game assets are committed to git. Better Ancient Hunt code/assets are not used in this smith stage.


## External declaration research — October 6

NativeContractReader and its synthetic arena are project-authored. Same-owner QoL bc996fe's independently observed profile-call contracts inform validation; no whole function or asset was copied. UEDumper 5b2b5264a66aa9edb28619c5ff654b16d3b9e038 (MIT) was inspected as a layout reference only; no source/binary copied or bundled. MCD-PE be646dcd82a689e24709b7abd4cff30fb60b7c9f (root Apache-2.0) was inspected as historical evidence; no reconstructed game code, executables or protection tools used. The collector has no third-party package dependencies. Official Microsoft runtime is downloaded separately with the existing checksum pin. Full provenance: NATIVE_DECLARATION_RESEARCH.md.


## Retail collector correction

Memory-region filtering and regression fixtures are project-authored using Microsoft's VirtualQueryEx documentation. Reviewed same-owner QoL 3914fec's newer LegacyNativeEvidence reader as a comparison, without copying code or assuming retail compatibility. No game bytes or user-path logs are published; the supplied capture is summarized as sanitized diagnostics.


## Source-grounded layout correction

Reused the same-owner QoL research/validation approach for inline name storage, 128/256 capacities, alternate child/target layouts and reserved object capacity; implementation and fixtures remain project-authored. Reviewed Epic-authored UE4.22.3 NameTypes.h/UObjectArray.h at 99a530d4ccbe6bea1e8f49df20acfeb294006962 as primary evidence; no engine source, SDK or protected executable copied/bundled. Related private QoL capture contributed its failure-stage evidence only; no accepted native declarations were present.


## Captured native presentation implementation

At the user's explicit request to reuse same-owner Minecraft-Dungeons-QoL, adapted its Graph.cs and FeatureValidation.RepairAdded property/function/import/preload construction approach (reviewed local ce7002779f0b47db8986a2a230628399027f5586 and origin/main e535cab78a11999329ff3ae5d5c5608dd7986f07). UniquePresentation.cs is a focused project-authored adaptation with capture-specific signatures and validators, rather than a wholesale helper copy. QoL has no root license granting general third-party reuse; authorization here is the owner's explicit instruction for these two repositories. This does not confer permission for unrelated third-party assets. Original-derived cooked game output stays private. Better Ancient Hunt remains used only in the previously documented Hunt adaptation.


## Selected record/dependency continuation

Reviewed same-owner QoL 24d3d39c1c58e1f28ee5b806a48c1b291e3c18a6 and its completed 134806 capture as permitted research. Reused its bounded nested-array declaration approach, retaining Rebalance's independent header/property bounds and existing process rights. SelectedItemReaders is project-authored using the earlier graph/import/preload approach authorized by the user. No game source/assets or third-party loader binaries published.

## October 6 v4 native record research

No additional third-party source or retail assets are bundled in collector v5. The record ABI and enum values are from the user's successful native declaration capture; new validators and bounded closure code are project-owned. Existing Camp graph construction continues the documented QoL pattern under the previously recorded permission.

Historical mcd-pe source at be646dcd82a689e24709b7abd4cff30fb60b7c9f was consulted for SerializableItemId's optional cached ID and InventoryItemData's optional subitem/store-count state and delegate. It supports investigating hidden native state, but its older source does not prove the current retail layout or save behavior. Epic UE4.22.3 Class.h at 99a530d4ccbe6bea1e8f49df20acfeb294006962 defines default WithCopy for non-POD structs. This explains why native struct operations matter; it does not certify current retail CppStructOps. No source text or guessed private fields were copied into the runtime.

## V5 merchant screen integration

MerchantScreens.cs is project-owned. It modifies private copies of the supplied UMG_Merchant package and refers to the game's existing UMG_MerchantItemDecision widget, preserving its original decision/input graph and game assets. These private retail outputs are excluded from git and no new gameplay package is redistributed in this pass. Exact self-name relocation avoids accidentally renaming the original decision widget. No new third-party source or loader was added.

## Camp placement preview — October 6

SpawnGraph.cs adapts the same-owner Minecraft-Dungeons-QoL graph-building pattern under the user's explicit reuse authorization. CampPlacement's operation ordering/guards are authored here from observed retail calls and pinned engine declarations. Native meshes/materials/animations stay referenced by cloned retail actors; no game assets or engine header code are committed to the repository. The private user test PAK includes cloned supplied cooked assets. Better Ancient Hunt adaptation is unchanged and remains credited to Onetoeisenough. This preview requires no BlueprintLoader or UE4SS deployment.

## Loading correction provenance

Native Function archetype/dependency requirements were recovered by comparing supplied original packages and reading pinned UE4.22.3/UAssetAPI1.1.0 loading/writer code. FunctionLoadContract and minidump summary script are authored here; no engine code, SDK code or raw game binary is redistributed. Public SDK was naming-only research.

## October 7 v6 reuse

Under the user's explicit same-owner QoL reuse instruction, reviewed Minecraft-Dungeons-QoL ce7002779f0b47db8986a2a230628399027f5586: tools/CookedInventoryFeatures/Graph.cs's one-byte native Boolean factory and FunctionLayoutContracts.cs's parameter/local ordering and function flag validation. Rebalance's SpawnGraph extensions and FunctionLoadContract adapt that approach. PaidUpgradeGateway and PaidUpgradeButtons are authored here against the existing user capture and pinned engine declarations. No QoL cooked assets, SDK implementation, engine source or additional third-party loader is bundled. Native smith visuals/UI are private copies/references from the user's supplied retail assets. Better Ancient Hunt credit and the accepted v2 adaptation remain unchanged.
