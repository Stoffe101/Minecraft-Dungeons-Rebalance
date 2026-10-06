# Research sources

Last reviewed: 2026-10-05

These sources support the bootstrap research. URLs are recorded so findings can be rechecked later.

## Better Ancient Hunt

**Nexus Mods — Better Ancient Hunt**
https://www.nexusmods.com/minecraftdungeons/mods/173

Author: Onetoeisenough

Used for:

- documented feature set
- modification permission
- asset-use permission
- redistribution/credit requirements

Observed Nexus lines/sections include:

- About this mod
- Credits and distribution permission
- Author notes

## Ancient Hunts / gold

**Minecraft Wiki — Ancient Hunt**
https://minecraft.wiki/w/Dungeons%3AAncient_Hunt

Used for:

- hunt structure
- Gold Rooms
- hunt doors
- 30-gold completion reward
- current Gold Chest and Rare Gold Chest values

**Minecraft Wiki — Minecraft Dungeons 1.17.0.0**
https://minecraft.wiki/w/Dungeons%3A1.17.0.0

Used for historical balance change:

- completion 10 -> 30
- Gold Chest 2-5 -> 4-6
- Rare Gold Chest 4-7 -> 8-10

**Minecraft Wiki — Gold**
https://minecraft.wiki/w/Dungeons%3AGold

Used for:

- gold acquisition context
- Ancient Hunt chest/mob sources
- completion reward context

## Tower smiths

**Minecraft Wiki — The Tower**
https://minecraft.wiki/w/Dungeons%3AThe_Tower

Used for:

- three merchant floor variants
- Powersmith/Uniquesmith/Gildsmith purposes

**Minecraft Wiki — Uniquesmith**
https://minecraft.wiki/w/Dungeons%3AUniquesmith

Used for:

- Uniquesmith identity
- level ID: Artisan

**Minecraft Wiki — Gildsmith**
https://minecraft.wiki/w/Dungeons%3AGildsmith

Used for:

- Gildsmith identity
- level ID: Gilder
- random gilded enchantment/tier behavior

**Minecraft Wiki — Minecraft Dungeons 1.13.1.0**
https://minecraft.wiki/w/Dungeons%3A1.13.1.0

**Minecraft Wiki — Minecraft Dungeons 1.14.1.0**
https://minecraft.wiki/w/Dungeons%3A1.14.1.0

Used for:

- Uniquesmith power normalization behavior
- historical fixes around Gildsmith/Uniquesmith

## Item model

**Minecraft Wiki — Rarity**
https://minecraft.wiki/w/Dungeons%3ARarity

Used for:

- Common/Rare/Unique behavior
- Rare variants
- Unique item families

**Minecraft Wiki — Enchantment**
https://minecraft.wiki/w/Dungeons%3AEnchantment

Used for:

- gilded built-in enchantments
- gilded tiers
- gilded metadata behavior

## Emeralds

**Minecraft Wiki — Loot Urn**
https://minecraft.wiki/w/Dungeons%3ALoot_Urn

Used for:

- 15-30 emerald reward

**Minecraft Wiki — Emerald**
https://minecraft.wiki/w/Dungeons%3AEmerald

Used for:

- Camp Emerald Chest 50-emerald reward

## Sibling project

**Stoffe101/Minecraft-Dungeons-QoL**
https://github.com/Stoffe101/Minecraft-Dungeons-QoL

Used as the source of reusable internal tooling/research once a formal code audit begins.


## Sources checked 2026-10-05

- User-provided Store catalog in `game-evidence-4.zip` (October 4): exact path evidence, 131,164 entries.
- User-provided `game-evidence-legacy.zip` and inventory-patch-source archive: existing inventory ABI/metadata only.
- https://github.com/Stoffe101/Minecraft-Dungeons-QoL/tree/ce7002779f0b47db8986a2a230628399027f5586 — actual reused collector source and prior evidence limitations.
- https://www.nexusmods.com/minecraftdungeons/mods/173 — Better Ancient Hunt author, permission terms and advertised features.
- https://www.nexusmods.com/minecraftdungeons/mods/173?tab=files — v1.0 PAK file listing, file id 378.
- https://github.com/Saad5400/minecraft-dungeons-arabic/blob/c1a8c20ea714ab63ea33b04025bc84c08747f12a/tools/pak.js — primary-source published archive key; same key already verified by user catalog collection.
- https://github.com/StainlessStasis/LetMeMove/releases/tag/1.1.0 — MIT UE4.22 test fixture.
- https://github.com/Dokucraft/Dungeons-Mod-Kit/tree/c30e88ec5e99e401eadedddbe82af0265a056fe7 — MIT test packager.


## October 6 native declaration research

- Same-owner [QoL profile call contracts at bc996fe](https://github.com/Stoffe101/Minecraft-Dungeons-QoL/blob/bc996fe/tools/CookedInventoryFeatures/HeroProfileCallContracts.cs).
- [UEDumper pinned source](https://github.com/Spuckwaffel/UEDumper/tree/5b2b5264a66aa9edb28619c5ff654b16d3b9e038): candidate legacy layouts, not Store ABI certification.
- [MCD-PE pinned source](https://github.com/Minecraforever/MCD-PE/tree/be646dcd82a689e24709b7abd4cff30fb60b7c9f): historical declarations only; no source or protection tool reuse.
- Microsoft [ReadProcessMemory](https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory) and [process security/access rights](https://learn.microsoft.com/en-us/windows/win32/procthread/process-security-and-access-rights).

- Microsoft [VirtualQueryEx](https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualqueryex) and [MEMORY_BASIC_INFORMATION](https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-memory_basic_information): read-only memory-region filtering; no additional handle rights.

- Epic-authored UE4.22.3 [NameTypes.h](https://github.com/folgerwang/UnrealEngine/blob/99a530d4ccbe6bea1e8f49df20acfeb294006962/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h) and [UObjectArray.h](https://github.com/folgerwang/UnrealEngine/blob/99a530d4ccbe6bea1e8f49df20acfeb294006962/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectArray.h): 256-slot name table and reserved-capacity behavior; layouts still require current independent native-control validation.
