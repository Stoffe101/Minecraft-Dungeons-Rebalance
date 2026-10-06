# Hunts / Economy test build v2

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
