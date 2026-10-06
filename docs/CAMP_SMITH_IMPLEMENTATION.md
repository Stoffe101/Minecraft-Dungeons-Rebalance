# Camp smith implementation

Updated October 6, 2026. Camp smiths are the user's highest implementation priority.

## Implemented asset foundation

`tools/CampSmithStager` now stages three separate native NPC actor copies and three separate native content widget copies from the already supplied private original packages. All six are relocated into `/Game/Mods/MinecraftDungeonsRebalance/Camp`. Native definitions, meshes, animation references, sounds, widget trees, property values and script behavior are retained; self/cross names are relocated together. Native Tower assets are not overwritten.

| Service | New actor | New content widget | Existing native transaction |
| --- | --- | --- | --- |
| Uniquesmith | BP_RebalanceCampUniquesmith | UMG_RebalanceCampUniquesmithContent | UniqueCollectItem |
| Powersmith | BP_RebalanceCampPowersmith | UMG_RebalanceCampPowersmithContent | UpgradeTowerItem |
| Gildsmith | BP_RebalanceCampGildsmith | UMG_RebalanceCampGildsmithContent | GildItem |

The local private stage contains twelve cooked files (six `.uasset`/`.uexp` pairs). [Validation report](research/camp-smith-stage.json) contains checksums and structural results, not raw game assets. Each package is written/reopened and compared with the expected relocated parsed package. Original input hashes are checked again after staging. All originals must validate before output creation; a failed stage is removed, and existing/nested output is rejected.

Four integration checks passed against the real supplied originals: six-package staging and unchanged originals, missing sources with no output, existing output preservation, and nested output rejection. These are asset-tool tests; no NPC was loaded or transaction executed in a game. The tool compiled through SDK 8.0.415's Roslyn compiler locally because this workspace's process-information restrictions prevent the normal .NET/MSBuild front ends from starting. Windows CI now builds the normal project.

## Exact native bindings traced

The actor `MerchantDef.MerchantDefinition` properties reference TowerArtisanMerchantDef, TowerBlacksmithMerchantDef and TowerGilderMerchantDef. Native merchant enum values are TowerUniquesmith, **TowerBlacksmith** and TowerGildsmith; the Powersmith's internal enum is not named TowerPowersmith. Both original one-transaction flags remain true in the copies until repeatable paid Camp transactions are verified.

The Artisan/Gilder widgets' `OnOpened` graphs get the owning player, cast to BP_PlayerController, obtain GetControlledPlayerCharacter, and pass that character to `UMG_SelectInventoryPlayerView.SetPlayerCharacter`. The Powersmith passes the owning controlled character to `UMG_MerchantEquippedGearWithPlayerView.SetPlayerCharacter`. It therefore needs additional adaptation for the agreed inventory selection flow.

Native action buttons bind **TransactionClassPrio**, not a serialized fixed price or selected-result parameter. In the original Artisan/Gilder metadata, `Upgrade` exports 3409/3410 hold UniqueCollectItem/GildItem respectively. In the Powersmith, `UpgradeItem` exports 3597/3598 hold UpgradeTowerItem. The native class imports do not supply transaction implementations. Showing a custom price in UMG would not establish that it is charged.

Camp's `lobby.json` uses region-based prop groups such as `*.*.blacksmith` and `*.*.enchantermerchant`, with native unlock alternatives. The supplied `thetower_npc/objectgroup.json` contains three complete 59-by-59 floor tiles, not three portable NPC prop objects. Adding those entire floor tiles to Camp is not an NPC placement implementation. Their baked block/resource-pack prefab mapping has not been resolved. New positions and navigation/interaction clearance remain unvalidated.

## Remaining implementation

The isolated assets are **not bound to a Camp spawner or controller dispatch**. Their native merchant types still refer to native Tower dispatch; the cloned screens are not automatically selected. There is no paid persistent-item transaction, Common-to-Rare mutation, gild reroll or native Unique variant picker yet. The Python policy model remains a separate reference implementation and is not loaded by the PAK.

The next concrete work is to establish a compatible native transaction route, including the exact selected-result item operation and currency writer, then bind Camp interactions to the isolated screens. Native family/presentation data must feed the names/icons/details picker and the explicit choice must reach the authoritative mutation. Payment, preservation, repeatability and failure handling must be verified together. Common-to-Rare costs 750 emeralds; selected Unique conversion 2,500 emeralds; gilding 150 gold; gild reroll 250 gold. Powersmith price/power rules remain undecided.

Current assets cannot establish that native Tower transactions work on persistent Camp items. No live service or new gameplay PAK is enabled by this pass; v2 remains unchanged. The withdrawn UE4SS probe remains disabled. No new collection of previously supplied sources is requested.
