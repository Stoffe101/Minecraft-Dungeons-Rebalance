# Guaranteed random Ancient — October 5 continuation

## Required behavior

Every Hunt successfully started by the game should contain at least one reachable, randomly selected Ancient encounter, even with one sacrificed item, zero enchantment points, or a rune combination matching no Ancient. Sacrificing more items must not be required for this minimum. Existing targeted opportunities and the adapted extra waves should remain useful; the guaranteed slot must not remove gold rooms. This changes encounter selection, not guaranteed gilded rarity or unique drops.

This requirement is accepted but **not implemented in test v2**. Native mission admission/payment remains unchanged for now; zero-item launch support is not established. Neither a 100% displayed chance nor larger waves proves an encounter exists.

## Direct evidence

- The received retail catalog contains `Dungeons/Content/data/lovika/levels/netherhypermission-hyper.json`. It was omitted from the initial 72-target collection. Earlier notes about an unverified upstream file meant no same-named *collected source*, not absence from the catalog.
- Better Ancient Hunt's copy has `hyperdungeons`, `levels`, `definition-levels`, `mission-mapping`, and `mobs`. Its 29 hyperdungeon entries include 26 Ancient entries and three gold entries. Ancient entries carry native-looking `archetype-requirements` rune counts; this is eligibility evidence, not a proven spawn-probability field.
- The installed `ancientdungeons.json` defines encounter rooms and waves. It has no offering-count/rune/chance control. The biome files use `@hyperlevel` portals and native `BP_HyperDungeonDoor` references.
- The initial native package collection has no offering/chance/door-selection Blueprint. The catalog identifies exact supplemental paths; `config/evidence-ancient-targets.json` records 14 packages plus one JSON, all present and none repeated from the original set.
- An old SDK mentions MissionChancesUtil/MissionOfferingsUtil, but has void/empty parameter metadata. This remains a navigation lead only and cannot establish current native signatures.

Removing all rune requirements in the author's config would at most broaden eligibility. It would not establish a nonzero encounter count, a reachable room, or whether native launch checks accept an empty requirement. Do not ship that as the guarantee. Injecting random Ancient ambient mobs also would not establish normal encounter loot/progression.

## Next implementation gate

Inspect the original hypermission configuration and the actual calls in BPL_MissionOfferings, offering widgets, merchant content and HyperDungeonDoor. Identify where the authoritative generated Hunt receives its Ancient candidates and roll/count. If those delegate entirely to native code, record that result and obtain a runtime reflection/diagnostic contract before calling or replacing them. Do not patch chance labels alone.

The implementation must select a supported Ancient using the Hunt's native random context, reserve one reachable room/slot before optional selections, and retain native encounter setup/loot. Validate all IDs against installed room and wave definitions. Avoid consuming another sacrifice or changing already-started Hunts. Verify the host makes the selection once and clients use that same result.

## Acceptance tests

| Case | Required observation |
| --- | --- |
| One item / no enchantment points | At least one reachable Ancient room |
| One item / no matching rune combination | Random fallback Ancient exists |
| Two, three, four items | Same minimum; targeted opportunities remain |
| Every offering category | No empty pool or invalid room |
| Repeated seeds / all ten biomes | No zero-Ancient successful Hunt; no softlock |
| Ancient defeated | Normal loot and adapted next waves complete |
| Host + client | Same Ancient/room, no duplicate generation/grant |
| Leave/rejoin/reload generated Hunt | No extra re-roll or duplicate spawn |
| Gold rooms | Still generated and their exits/rewards work |

A statistical average alone cannot prove the guarantee: generation-level validation should assert the reserved encounter for every tested seed and fail on zero count. Retail tests must then confirm the generated definition becomes a usable encounter.

## Supplemental collection

Extract `Minecraft-Dungeons-Rebalance-Ancient-Collector.zip` outside the game, double-click `Collect-AncientSpawnEvidence.cmd`, and return the newly generated `.research/rebalance-ancient-evidence-*.zip`. The bundle uses the already-tested pinned collector executable; the new allowlist is validated. It does not modify the game, read saves/executables, or collect the previous 72 assets. This collection may expose native-only delegation rather than a patchable graph; that limitation will be documented.
