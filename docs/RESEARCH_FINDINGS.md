# Research findings

Last updated: 2026-10-05

This file separates **confirmed findings** from assumptions and research targets.

## Confirmed: Ancient Hunt structure and gold

Community documentation states:

- Ancient Hunt consists of three randomly selected sub-missions connected by Nether portals.
- Hunt doors can lead to an Ancient dungeon or a Gold Room.
- Gold Rooms contain piglin chests that reward gold.
- Ancient Hunt is the mission type where gold is obtained from mobs and chests.
- The final mission chest always awards 30 gold.
- Version 1.17.0.0 increased:
  - completion gold from 10 -> 30
  - normal Gold Chest from 2-5 -> 4-6
  - Rare Gold Chest from 4-7 -> 8-10

Implication: there are multiple tunable gold sources. We should identify and patch the native reward definitions instead of adding a wallet multiplier.

## Confirmed: Tower smiths exist as native merchant floors

The Tower has:

- Powersmith
- Uniquesmith
- Gildsmith

Their native purposes are:

- Powersmith: increase selected gear power.
- Uniquesmith: make selected gear into a Unique variant.
- Gildsmith: make selected gear Gilded.

### Known IDs

Community documentation identifies:

- Uniquesmith level ID: **Artisan**
- Gildsmith level ID: **Gilder**

### Gildsmith behavior

The Gildsmith applies:

- a random gilded enchantment
- a random gilded tier

Gilded gear is not simply a rarity flag. It has an extra built-in enchantment at tier I, II or III.

Implication: reusing the native Gildsmith path is strongly preferred over hand-constructing gilded item metadata.

### Uniquesmith behavior

Historical patch notes confirm the Uniquesmith also upgrades the selected item's power to match the player's strongest gear.

Implication: when bringing it to Camp, decide deliberately whether to preserve this extra power normalization or separate it from the Unique conversion.

## Confirmed: rarity model

Community documentation states:

- all Common equipment has a Rare variant
- Rare gear has a slightly higher power cap and higher value
- most gear families have one or more Unique variants
- Unique gear has special built-in properties/attributes

Implication: Common -> Rare should preserve item family, while Rare -> Unique may need variant selection/randomization depending on the native Uniquesmith implementation.

## Confirmed: emerald containers

Loot Urns drop **15-30 emeralds**.

The Camp Emerald Chest contains **50 emeralds**.

Implication: container reward tables are useful first targets for a visible, vanilla-feeling emerald rebalance.

## Confirmed: Better Ancient Hunt reuse permission

Nexus Mods page for **Better Ancient Hunt**, author **Onetoeisenough**, version 1.0, last updated 2024-10-03:

Documented behavior:

- longer side paths
- harder mobs including minibosses
- more mobs
- second wave after the first Ancient containing two Ancients
- raid captains after gold and Ancient battles

The page's current permissions explicitly state:

- modification is allowed
- conversion is allowed with creator credit
- asset reuse is allowed without permission
- redistribution/upload requires creator credit

Author note: usage must not result in harm of thinking things and must be appropriate for Minecraft Dungeons audiences.

Project policy: credit Onetoeisenough regardless of the minimum permission wording.

## Confirmed from Minecraft-Dungeons-QoL research

Our sibling project already established useful inventory-facing data and tooling:

- physical inventory item resolution
- item type
- rarity
- power
- enchantments
- gilded/netherite data
- cooked Blueprint/package editing
- native inventory/salvage integration patterns

This reduces the amount of infrastructure that Rebalance needs to rediscover.

## Research targets

### Camp NPC transplant

Need to locate:

- Tower merchant actor/class assets
- merchant floor spawn/setup assets
- interaction widgets
- item selection data flow
- Tower-only state assumptions
- audio/animation dependencies
- Camp spawn/registration path

### Item mutation

Need exact native calls for:

- Common -> Rare
- Unique conversion
- Gilding
- power upgrade
- currency spend/validation
- refresh/save notification after mutation

### Ancient Hunts

Need exact assets controlling:

- Gold Room frequency/selection
- hunt-door destination weighting
- gold chest reward ranges
- mob gold drops
- mob population
- Ancient waves
- raid captain waves
- spawn limits

### Emeralds

Need exact assets/functions controlling:

- Loot Urn rewards
- Camp Emerald Chest
- mob emerald bundles
- emerald drop chance
- boss/powerful-mob reward behavior
- pickup replication

### Multiplayer

Need to establish:

- which reward actors are server/host authoritative
- whether all clients receive emerald pickup rewards
- whether gold is collector-only in current retail behavior
- whether a host mod can safely award rewards to unmodded clients
- duplicate-award and reconnect edge cases


## 2026-10-05 native input audit

See [NATIVE_ASSET_RESEARCH.md](NATIVE_ASSET_RESEARCH.md) for actual Store catalog paths, primary-source permission checks and the distinction between path evidence and runtime contracts. No native smith/reward constant or replication signature has been established from package contents yet. The prior inventory upload is insufficient for these different systems.


## Actual package contents supersede earlier assumptions

Initial source collection is now complete. See NATIVE_ASSET_RESEARCH.md's superseding findings. The urn base amount is 3–7, so the observed first-pass field tuning is 6–14. This is a component amount range, not a measured guarantee of final emeralds per urn or 2x whole-mission income. Tower transactions and currency ownership reside in native classes; supplying actors/widgets did not reveal a safe paid Camp transaction or shared award API. The implemented PAK therefore covers the proven Hunt/chest/urn subset only.

## Supplemental / upgrade-choice continuation

Supplemental inputs confirm chance calculation is a native MissionChancesUtil call used by UI; native encounter selection is not supplied Kismet. Upgrade option/pricing models are developer source only, with no real family/name/icon mapping invented.

## Camp smith asset implementation — October 6, 2026

Implemented CampSmithStager: six native actor/widget clones in an isolated Rebalance namespace; original sources and Tower flags preserved; all outputs re-opened and semantically verified. Four private-source integration checks passed. Native Powersmith uses TowerBlacksmith enum and equipped-gear view; Artisan/Gilder use owning-player inventory selection. Upgrade buttons bind TransactionClassPrio to native transaction classes, not a demonstrated price/selected-result API. Tower NPC objectgroup contains complete floor tiles rather than portable props. See CAMP_SMITH_IMPLEMENTATION.md and research/camp-smith-stage.json. No live Camp placement, payment, persistent mutation or Unique picker was enabled; latest gameplay v2 unchanged. Local SDK/MSBuild process-information failures were bypassed with SDK Roslyn compilation; Windows CI now builds the normal project. Next: compatible native transaction route, paid selected-result mutation and native presentation binding, then Camp spawn/dispatch and real saved-item tests.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.
