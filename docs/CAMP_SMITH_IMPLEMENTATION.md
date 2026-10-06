# Camp smith implementation

Updated October 6, 2026. Camp smiths are the user's highest implementation priority.

## Implemented asset foundation

`tools/CampSmithStager` now stages three separate native NPC actor copies and three separate native content widget copies from the already supplied private original packages. All six are relocated into `/Game/Mods/MinecraftDungeonsRebalance/Camp`. Native definitions, meshes, animation references, sounds, widget trees, property values and original script behavior are retained; three captured native presentation functions are added to the Uniquesmith widget; self/cross names are relocated together. Native Tower assets are not overwritten.

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


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## Successful retail declarations and native presentation — October 6

Received native-contracts-20261006-133711.zip. Completed=true, all six controls passed, all 18 allowlisted classes present, no runner issues. Accepted layout: name capacity 256, characters +12, children +0x48, property target +0x70, function header +0x98, parameter-count delta 4. Read 51,841,974 bytes in 1,292,194 calls and 1,045 region queries. Capture invoked no gameplay function and wrote no game memory. All 50 Windows self-tests passed. Sanitized summary: research/native-capture-20261006-v3.json; selected declarations and input SHA-256: research/native-upgrade-contracts-v1.json. Historical failed-capture sections above describe earlier attempts. A repeat of the same v3 capture is not needed.

Native ItemFunctionLibrary provides SerializableItemId-based name, description and Texture2D icon calls. MerchantTransactionBase exposes OnTransactionDecisionMade(InventoryItemData), CanExecute() and TryExecute(). WalletComponent.Balance returns int32; Deduct has no return value. GildItem, UniqueCollectItem and UpgradeTowerItem declare no direct functions and inherit InventoryItemSlotTransactionBase, which this capture did not collect. The selected-result declaration does not establish native outcome generation, transaction lifetime, payment override, mutation or persistent Camp behavior. InventoryItem.TryUpgradeItem must not be assumed to implement the agreed rarity/power policy from its name.

CampSmithStager now embeds the selected captured contract manifest and adds RebalanceUniqueName, RebalanceUniqueDescription and RebalanceUniqueIcon to the cloned Uniquesmith content widget. Each accepts the native item ID and calls the exact captured static native owner. Added function fields, ownership/function maps, native property archetypes and preload dependencies are validated before writing and after reopening. Original graphs and six source package pairs are retained; the three functions do not execute paid upgrades. Full choice list, native family enumeration, confirmation, Camp spawning/dispatch, persistent payment and shared gold remain unfinished.

Five private-source integration tests pass, including seven deliberate graph/signature/preload rejection checks. All six packages write/reopen with exact expected parsed equality; exactly three functions are added. Initial compilation caught an ArrayDim enum assignment; first round-trip caught nonserialized ElementSize/Next fields. Corrected both and reran successfully. SDK Roslyn compiled locally; one existing Newtonsoft net6/net8 reference-unification warning remains in this direct-compiler path. Report: research/camp-smith-presentation-stage.json. No Unreal/gameplay execution occurred.

Reviewed related QoL native-favorites-20261006-133827-9942ba.zip: incomplete at the 2M call budget, empty declarations. It supplies no missing superclass/struct contracts. Reused the same-owner QoL graph/property/preload construction approach at the user's explicit request; no third-party unlicensed source or game-owned assets published.

Next work: obtain the missing InventoryItemSlotTransactionBase/MerchantSubobjectBase/MerchantDef and InventoryItemData/SerializableItemId/MerchantDisplayPrice/EnchantmentData declarations, including nested array/enum types; trace native outcome enumeration and chosen-result handling; wire the presentation functions into an owned picker and Camp screen dispatch; establish persistent item/currency transaction behavior before gameplay acceptance. No new collector or NPC PAK is delivered in this increment.

Existing upgrade policy tests (17) and native call indexing tests (5) pass after this increment. Windows normal build is pending at publication; no retail presentation/upgrade test is claimed.
