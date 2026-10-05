# Current state

Updated 2026-10-05 after receiving the targeted collection and Better Ancient Hunt.

**First Hunts/Economy test PAK built and structurally verified. Full design is unfinished; no retail gameplay validation yet.**

## Input and research results

All 72 targets collected without errors. All 97 raw package/companion/JSON hashes verified against the collection manifest. Input lists 47 top-level game archives and no apparent directly installed mod archive. Better Ancient Hunt 1.0 contains 13 JSON files, no cooked Blueprints or source scripts.

Gold chest native defaults confirm ranges 4–6 and 8–10. Loot Urn base defaults specify 3–7 (not the earlier assumed 15–30); native unit/bundle semantics and subclass overrides still need testing. Smith actors point to native `/Script/Dungeons` merchant definition classes. Their widgets select native GildItem, UniqueCollectItem and UpgradeTowerItem transactions. Those implementations are not editable Blueprint graphs supplied by these packages.

## Implemented build: HuntsEconomy-Test-v2

- Normal/rare chest component ranges patched to 10–15 / 20–30.
- Base Loot Urn component amount range doubled to 6–14.
- Camp emerald chest native `EmeraldsReward` increased from 50 to 100.
- Three economy integration tests pass, including rejecting modified Camp defaults/invalid price ranges before output.
- Eleven original Hunt levels adapted with permitted mod enemy groups, arena waves and side paths.
- Twenty-six Ancient encounter definitions contain first wave of one, extra wave of two, and one raid captain.
- Finite generation-request bounds and exact native route/trigger/gate/reward preservation.
- Standalone 19-entry PAK produced, integrity checked and unpack-compared byte-for-byte.
- Four cooked package outputs re-opened; all imports, property schema and Blueprint scripts match their originals.
- Eight Hunt tests pass, including all eleven real levels and negative route/gate/reference/identity checks.

Windows CI run 37364457448 failed with the collector job cancelled before any steps or runner assignment. This is not a compiler/test failure result; Windows CI verification remains outstanding.

See BUILD_AND_TEST.md for installation, actual scope and runtime checklist.

## Full design status

| Feature | State |
| --- | --- |
| Three Tower NPCs and paid smith services in Camp | Unfinished; native definition/transaction adaptation required |
| Chest gold ranges | Implemented, retail untested |
| Completion 50 gold | Unfinished; reward writer not identified in supplied Blueprint graphs |
| More enemies / Ancient waves / longer side paths | Implemented adaptation, retail untested |
| Gold Room opportunities +50–75% | Still a tuning goal; explicit native room weighting unresolved |
| Base urn drop amount doubling | Implemented; effective income/variant coverage unmeasured |
| Camp emerald chest | Implemented scalar 50 -> 100, retail untested |
| Global mob emerald/gold income | Unfinished |
| Guaranteed random Ancient per started Hunt | Goal accepted; supplementary generation/offerings inputs needed |
| Party-wide gold awards | Unfinished; native pickup/store authority contract unresolved |
| Typical 150–220 / lucky 250–300 gold | Target only; not measured |

## Next work

Retail-test the playable subset while researching native merchant transaction/currency classes, native pickup/store authority and completion rewards. Determine safe charge-before-mutation/commit-on-success semantics, persistent item preservation and repeatability before enabling Camp smiths. Avoid placing free Tower merchants into Camp and calling that a completed paid upgrade system. Source package collection is complete for the initial targets; do not ask for the same upload again.

## Latest continuation

Supplemental Ancient collection received: 15/15 targets, zero errors, 29/29 source hashes verified. The offering/chance/door inspection resolves to native probability/request paths and presentation, not an exposed guaranteed encounter writer. Guaranteed Ancient selection remains unimplemented. No repeat collection is needed.

Started upgrade choice/confirmation source foundation in src/rebalance/upgrades.py, including selected Unique outcomes with name/icon/description/effect/stat row data, agreed prices and item/ownership/catalog/balance/receipt guards. Seventeen synthetic-adapter policy tests pass. **No native adapter or in-game picker is implemented**; this Python module is not loaded by Minecraft Dungeons. Two developer-only Lua reflection API-shim tests pass; UE4SS compatibility remains unverified and no runtime loader is enabled. See UPGRADES.md.

Next: current native selected-variant/transaction contracts, native presentation/family resolution, native UMG picker and persistent Camp integration; native authoritative Hunt generation; shared-gold authority. Latest game test remains v2.
