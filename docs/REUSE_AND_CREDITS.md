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
