# Hunts / Economy test build v2

**Latest runtime result: v3 crashes and is withdrawn. v4 restores generated Function archetypes/creation prerequisites and passes eleven private-source checks plus 37-entry PAK verification; this correction is retail-unverified. Prices and paid upgrades remain unfinished. Use v2 as the known reward baseline. See [CAMP_PLACEMENT_TEST.md](CAMP_PLACEMENT_TEST.md). Earlier milestone notes below are historical.**

**Current milestone (October 6): CampPlacement-Test-v3 packages a host-only, non-interactive Camp spawn preview, retaining v2 economy/Hunt changes. Ten package pairs and nine integration tests pass; all 37 PAK entries integrity-test and unpack-compare exactly. Paid upgrades/picker/persistence/shared gold remain unfinished and no retail placement result exists. See [CAMP_PLACEMENT_TEST.md](CAMP_PLACEMENT_TEST.md). Earlier sections below record earlier passes.**

This is the first playable test subset, **not the complete Rebalance design**. Structural validation passed. On October 6, the user confirmed the Camp chest pays 100 emeralds instead of 50 in game. The user subsequently marked gold chests complete and observed +25 emeralds from an urn (241 -> 266). Generated mission progression, broader urn coverage and co-op remain unverified.

## Install on the known Store installation

Close Minecraft Dungeons. Extract the test ZIP and place its `.pak` in:

```text
C:\XboxGames\Minecraft Dungeons\Content\Dungeons\Content\Paks\~mods
```

Create `~mods` if needed. Use the installation launched by Minecraft Launcher. Remove the original `zAncient_Hunt_Mod.pak` and any earlier Rebalance variant from that folder: both override the same Hunt files. QoL can remain installed because this build does not patch inventory UI. This subset overrides existing game files and does not need Blueprint Loader itself.

To uninstall, remove this PAK and restart the game. It does not edit saves directly; rewards acquired normally during play persist through the game's normal save process.

## Implemented

| Feature | Exact implementation |
| --- | --- |
| Normal gold chest | `ConsumableDrop_GEN_VARIABLE.DropData` Gold amount range 4–6 -> 10–15 |
| Rare gold chest | Same verified Gold component range 8–10 -> 20–30 |
| Camp emerald chest | Native `BP_LobbyChest` CDO `EmeraldsReward`: 50 -> 100; user confirmed 100 emeralds in game on October 6; native availability unchanged |
| Base Loot Urn | `EmeraldDrop_GEN_VARIABLE.DropData` amount range 3–7 -> 6–14; field/bundle units and subclass coverage still require retail measurement |
| Hunt enemies and paths | Selected Better Ancient Hunt changes applied onto 11 original retail levels |
| Extra Ancients | 26 native encounter definitions retain first Ancient and add a wave of two plus one raid captain |
| Gold arenas | More finite enemy waves from the mod; original native trigger/gate/reward data preserved |
| Bounds | Density <=1.75, per-type min/max <=4, arena wave request <=24, <=10 waves per arena, side-path probability <=1, side-path maximum length <=4 |

These are bounds on serialized generator requests. They are not a guarantee of a global concurrent-actor limit, and native summoning/enchantment behaviors are untouched.

No fixed 150–220 run payout is promised. That target still needs measured tuning. The completion award is still native, not raised to 50 by this build.

## Still unfinished

- Powersmith/Uniquesmith/Gildsmith in Camp and paid persistent-item transactions.
- Common -> Rare (750 emeralds), Rare -> Unique (2,500), gilding (150 gold) and rerolling (250).
- Powersmith price/power-cap policy (explicitly TBD in the accepted design).
- Hunt completion reward and global mob currency bundle sizes/chance.
- Increased native Ancient encounter-selection chance (October 6 goal; guarantee withdrawn).
- A verified direct increase in Gold Room weighting separate from extra side-path opportunities.
- Shared gold grants and host-only installation support.

Do not present the above as features in this PAK.

## Retail test sequence

1. Launch, select a hero, enter Camp and inventory; confirm no new loading crash.
2. Break ordinary loot urns and record drop totals. Check different urn colors/levels for inherited-versus-overridden behavior.
3. Open the available Camp emerald chest and record the wallet delta; the patch does not reset its native availability.
4. Start a **new** Ancient Hunt after installing. Existing generated missions may retain earlier generation state.
5. Open normal and rare gold chests. Record count, wallet gain and who collected them.
6. Defeat the first Ancient, verify the two-Ancient wave and later raid captain, then verify exits/mission completion. Record any long delay or stuck challenge.
7. Record Hunt duration, deaths, Ancients, rooms/chests, gold from mobs, completion award and total gold for multiple runs.
8. After solo passes, test host + client with the same PAK. Check both wallets per chest/pickup and mission completion. No sharing is claimed yet; this establishes the native baseline.

Report the failing step, map/biome, whether solo/co-op, and any game crash output. Increase/decrease tuning based on measured runs rather than projected totals.

## Rebuild from private inputs

Use .NET 8 to build `tools/CookedEconomyPatcher/CookedEconomyPatcher.csproj` (UAssetAPI 1.1.0). Unpack the permitted original Better Ancient Hunt PAK with the pinned Mod Kit u4pak. Then:

```text
python scripts/build_test_pak.py --sources <collection/Metadata/PatchSources> --upstream <unpacked-mod> --balance config/balance.json --output <fresh-build-directory> --dotnet <dotnet> --patcher <CookedEconomyPatcher.dll> --packager <pinned-u4pak.py>
```

The builder verifies package serialization/re-read, validates level identities/protected routes/gates/wave references, checks PAK integrity, unpacks the result and compares all 19 entries byte-for-byte. Reports stay outside the game PAK. Raw retail inputs and generated game packages stay out of git.

## Delivered artifact

PAK SHA-256: `349181e2efaa8dfa6861133dab7286b03920dd469196de8b65cd5189f7c77b00`. Includes 19 package entries. ZIP includes installation/test instructions, attribution and reports. Native Ancient encounter-selection chance is unchanged in this PAK; the guarantee goal was withdrawn on October 6. Remove v1 before installing v2.

## Retail results — October 6, 2026

**Camp emerald chest: PASS (user observation).** The user reports 100 emeralds instead of 50. This is sufficient confirmation for the reward change in the tested setup; no test count or multiplayer role was supplied. Remaining priority checks are normal gold chests (10–15), rare gold chests (20–30), extra Ancient waves with working exits, and urn reward deltas. The result does not establish overall income targets or unimplemented features. No rebuild is required for this documentation update.

## Additional user results — October 6, 2026

Emerald urn: wallet 241 -> 266, observed +25 emeralds. Gold chests: user requests marking the feature complete; recorded as gameplay acceptance passed by user confirmation, with exact per-chest measurements unspecified. These observations do not establish aggregate income, all urn subclasses, Prospector behavior or multiplayer distribution. Camp NPCs and paid upgrade mechanics, including the Unique variant picker, are the user’s highest priority.


## Native contract research bundle (separate from v2)

The October 6 Windows-validated NativeCollector ZIP is a research tool, not a PAK and not installed in ~mods. Extract it outside the game folder, start Dungeons normally at Camp, then run the command in its README from normal Windows x64 PowerShell. Return the generated native-contracts-<timestamp>.zip, including an incomplete capture if reported. It runs checks before query/read access, invokes no game/save functions and writes no game memory. Access denial stops capture; do not elevate or use a protection workaround. No Dungeons capture or paid service acceptance is claimed. Full validation, checksum and next integration steps: NATIVE_DECLARATION_RESEARCH.md.

## Camp placement preview Windows validation — October 6, 2026

At implementation commit 7972a9127c4ebaf1c90ad51452618afe9b4c9fc4, evidence run 37521291248 passed normal Windows CampSmithStager compilation, existing permitted-fixture integration checks and Python syntax validation of the preview builder/tests. Native declaration reader run 37521290998 and upgrade policy run 37521290989 also passed. Local private-source checks passed nine tests, including eight placement corruption rejections; an additional packager check preserved an existing output directory. Windows CI does not have private retail assets or a Dungeons runtime. The final 37-entry PAK hash is 6f13bc4f04f2f5d4a4782803158e41943c29d647b98968cae2066a9495fd409a (6,075,200 bytes). Test artifact is saved and source/documentation published; retail NPC placement remains pending.

## v4 Windows validation — October 6

Implementation commit f25d8fbc351e293eeebaa2d512d1ac77debef4a0 passed evidence run37524820391 (normal Windows CampSmithStager build, zero warnings/errors, permitted-fixture checks and Python syntax), native reader run37524820045 and policy run37524820052. Eleven private-source tests and the actual-crashed-v3 rejection ran locally; CI does not have those private inputs or a game runtime. v4 remains a runtime-unverified loading correction candidate, with prices/upgrades disabled. Final PAK hash1fc846ee8c0633cd10322a14ac4dedf23f67971ddfc2147a60cd6a1edbd3b527;6,075,360bytes. Saved the private candidate PAK and published source/docs. Targeted full collected asset-name inventory also yielded no merchant/pricing JSON/INI candidate or exposed price-definition asset; this is a filename inventory result, not proof that all native pricing configuration is absent.
