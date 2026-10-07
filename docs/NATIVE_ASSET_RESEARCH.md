# Native asset research — 2026-10-05

**Current (October 7): v5 crashed with `Unsupported UBoolProperty ReturnValue size 0` and is withdrawn. v6 fixes that serialized Boolean and implements interactable native smith buttons, guarded native upgrade transactions and temporary 1 emerald / 1 gold fees. It retains Gift Wrapper-relative placement, names and the accepted v2 reward/Hunt subset. Fifteen private integration tests and all 37 PAK entries pass structural checks. Gameplay, native Camp eligibility and save/reload are unverified; full prices, the custom Unique picker and shared gold remain unfinished. v4 NPC loading/spawning was user-confirmed. See [CAMP_UPGRADE_TEST_V6.md](CAMP_UPGRADE_TEST_V6.md) for implementation, tests, limitations and next work.**

Historical milestone notes below are superseded by the current v6 status.

## Evidence actually available

Inspected the user's October 4 Store archive catalog (131,164 entries), the prior inventory-only raw-source upload, and the QoL repository at `ce7002779f0b47db8986a2a230628399027f5586`. The archive catalog establishes paths, not package contents, callable native signatures, reward constants, or replication behavior. The 31-package inventory metadata export does not include Tower smith transactions or chest reward graphs. No retail game archives/executable are available in this workspace.

## Confirmed package paths

All paths below begin with `Dungeons/Content/`.

| Purpose | Observed path |
| --- | --- |
| Uniquesmith actor | `Content_Season1/Decor/Prefab/Merchants/BP_TowerArtisanMerchant.uasset` |
| Powersmith actor | `Content_Season1/Decor/Prefab/Merchants/BP_TowerBlacksmithMerchant.uasset` |
| Gildsmith actor | `Content_Season1/Decor/Prefab/Merchants/BP_TowerGilderMerchant.uasset` |
| Smith content widgets | `Content_Season1/UI/Merchant/UMG_TowerMerchant{Artisan,Blacksmith,Gilder}Content.uasset` (three actual files) |
| Tower merchant container | `UI/Merchant/UMG_TowerMerchant.uasset` |
| Normal gold chest | `Content_DLC4/Decor/Prefab/Functional/GoldChests/BP_GoldChest_Small.uasset` |
| Rare gold chest | `Content_DLC4/Decor/Prefab/Functional/GoldChests/BP_GoldChest_Rare.uasset` |
| Gold item | `Content_DLC4/Actors/Items/Gold/BP_GoldStorable.uasset` |
| Emerald item | `Actors/Items/Emerald/BP_EmeraldStorable.uasset` |
| Loot Urn base | `Decor/Prefabs/_Urns/LootUrnsBlueprints/BP_LootUrnBase.uasset` |
| Camp emerald chest | `Decor/Prefabs/RewardChest/BP_EmeraldChest_Reward.uasset` |
| Camp generator | `data/lovika/levels/lobby.json` |
| Camp object definitions | `data/lovika/objectgroups/Lobby/objectgroup.json` |
| Ancient Hunt generator | `data/lovika/levels/ancientdungeons.json` |
| Hunt biome generators | `data/lovika/levels/hm_*.json` (ten catalog paths) |
| Gold room definitions | `data/lovika/objectgroups/st_gold/objectgroup.json` |
| Ancient encounter definitions | `data/lovika/objectgroups/st_*/objectgroup.json` |
| Tower NPC object definitions | `data/lovika/objectgroups/thetower_npc/objectgroup.json` |

`config/evidence-targets.json` records the exact 25 cooked package and 47 JSON targets, all checked against that catalog. The wildcard/braced notation in this table is explanatory; the collector uses exact paths only.

## Implementation consequences

- Hunt layout and Camp placement have native Lovika JSON inputs. Inspect these before choosing Blueprint spawning or replacing a Camp map.
- Read smith actor defaults and merchant widgets together. Native actors alone do not prove persistent inventory mutation works outside Tower state.
- Inspect gold/emerald item defaults, chest bytecode and controller events to determine award authority and sharing. Do not double all positive currency changes: that would also alter refunds, salvage, gifts or purchased currency.
- Old SDK `InventoryItem.TryUpgradeItem` and merchant/currency class names are leads only; void-only/zero-size mirrors are not callable ABI evidence.
- Powersmith pricing is still explicitly TBD in the accepted design. Do not silently invent a final price/cap.
- More Gold Room opportunities (1.5–1.75 target) are not yet tied to a verified generator field. Extra waves require finite triggers and a cap based on actual generator semantics.

## Better Ancient Hunt

Author's primary page: https://www.nexusmods.com/minecraftdungeons/mods/173

Checked live on October 5. Version 1.0, author Onetoeisenough, released October 3, 2024. It advertises longer side paths, stronger/more enemies, a second wave of two Ancients, and raid captains after gold/Ancient battles. Modification and asset reuse are allowed; upload permission requires creator attribution. Author notes require appropriate all-audiences use and no harm to thinking things.

The Files tab identifies `zAncient_Hunt_Mod`, 157 KB, a PAK containing all contents (file id 378). The actual binary is not available here; no contents have been inspected or copied. A description and permission statement are not a source implementation. Obtain the author's download, then list it and compare its JSON/package changes against the installed game before adaptation.

## Concrete unblocker

Run the provided Windows collector and send its output ZIP plus the original Better Ancient Hunt `.pak`. The collector reads the active game's top-level archives and preserves the allowlisted package headers/bytecode companions and JSON, hashes each output, and includes defaults/imports/Kismet metadata. It does not read saves or executables or write to the installation. Keep the resulting raw game assets private.


## Superseding findings after the actual uploads

The previously missing inputs were supplied in this turn. All 72 target reads succeeded and all 97 preserved source hashes match. The Better Ancient Hunt PAK contains 13 JSON entries. Its `data/lovika/objectgroup/st_gold` path is singular while the installed reference is `objectgroups/st_gold`; its extra `netherhypermission-hyper` file has no same-named target in this collection. Neither entry is adopted without a proven native reference.

Gold chest ranges are confirmed in `ConsumableDrop_GEN_VARIABLE.DropData`, Gold category: small 4–6, rare 8–10. Base urn `EmeraldDrop_GEN_VARIABLE.DropData` is Emerald category 3–7. The first patch changes only those six integers; all imported objects, property metadata and Blueprint scripts remain identical after output re-read.

Smith actor `MerchantDef.MerchantDefinition` references native TowerArtisanMerchantDef, TowerBlacksmithMerchantDef and TowerGilderMerchantDef classes. CDOs set both one-transaction flags true. Content widgets prioritize native GildItem, UniqueCollectItem and UpgradeTowerItem transactions. Their implementation and pricing/commit semantics are not serialized Kismet in these widgets. Gold item derives from native StorableItem; its supplied graph concerns initialization/visibility/effects, not an exposed party currency award function. A path/class name alone still does not authorize an invented ABI.

Better Ancient Hunt densities reach 3.5 and side-path probabilities exceed 1 in places. Its Ancient waves use `[1, group], [2, group], [1, raid]` and native arena interval/rest controls. The adaptation retains these finite waves, bounds density/path counts, preserves original routing/gates/rewards, and rejects new unresolved wave-group references. Mod array-shaped `arena.reward` additions are not adopted: native supplied challenge rewards use different observed structures, and their semantics remain unproven. Main-route length changes are also excluded to bound mission expansion; permitted harder enemy groups and side-path changes are retained.

## Continuation: guarantee and Camp reward

The installed catalog **does** include netherhypermission-hyper.json; it was not collected previously. Its permitted upstream copy lists 29 hyperdungeons (26 Ancients and three gold rooms) and archetype-requirements. Broader eligibility alone does not guarantee an encounter roll. See ANCIENT_GUARANTEE.md for exact 15-target supplemental evidence and acceptance criteria.

BP_LobbyChest Default__BP_LobbyChest_C contains the native EmeraldsReward integer=50. v2 patches it to 100. BPL_Currency only provides currency presentation data/icons/sounds, not a party wallet writer. Continued merchant metadata inspection still shows native transaction classes rather than a verified atomic price/mutation callable contract. Neither Camp services nor gold sharing is claimed implemented.
