# Current state

Updated 2026-10-05.

**Targeted asset-collection tooling implemented and locally tested. No Rebalance gameplay `.pak` has been produced.**

## Completed in this pass

- Recovered exact accepted design; centralized it in `config/balance.json` without asserting those values are installed-game facts.
- Inspected the prior Store catalog and inventory sources; identified actual smith, chest, currency, Camp and Ancient Hunt generator paths.
- Verified Better Ancient Hunt reuse permissions and exact downloadable file identity. Its binary remains unavailable.
- Adapted QoL's legacy UE4.22 exporter and Windows bootstrap into a standalone collector for 25 cooked packages and 47 JSON files.
- Added tagged export defaults (needed for reward/replication research), bytecode metadata, raw companion preservation, per-file hashes, bounded exact target validation and partial failure reporting.
- Added five passing integration tests and a Windows CI build/test workflow; CI status is separate from local test status.
- Prepared a portable one-click collector using compiled tools and checksum-pinned dependencies.

## Gameplay status

| Feature | State |
| --- | --- |
| Three native Tower smith NPCs in Camp | Asset paths found; placement and runtime adaptation not implemented |
| 750/2,500 emerald rarity upgrades | Accepted prices only; mutation contracts unresolved |
| 150/250 gold gild services | Accepted prices only; persistent-item transactions unresolved |
| Powersmith | Native paths found; pricing/cap TBD as agreed |
| 50 completion / 10–15 normal / 20–30 rare chest gold | Targets preserved; native reward writers not available |
| More Gold Rooms / mobs / Ancients | Generator paths found; original JSON and Better Ancient Hunt binary needed |
| Approximately 2x natural emerald income | Targets preserved; reward writers not available |
| Multiplayer shared gold | Goal preserved; native award/replication contract not established |
| Gameplay packaging / retail testing | Not reached |

## Current blocker

Available raw game sources cover inventory UI only. This workspace has neither the retail game archives nor the targeted smith/reward/generation package contents. A path listing, old SDK method name, or mod description cannot establish safe currency/item mutation or replication semantics. The concrete next input is the collector ZIP output plus Better Ancient Hunt's original PAK; see EVIDENCE_COLLECTION.md.

## Next implementation pass

Inspect original defaults/Kismet/JSON and compare the third-party mod; bind centralized prices/rewards to observed fields/functions; implement finite encounter changes and native Camp placement; validate transaction success/currency order and ownership; build and re-open a standalone PAK; then test solo and co-op on retail. Keep runtime and structural validation results distinct.
