# Hunts / Economy test build v1

This is the first playable test subset, **not the complete Rebalance design**. Structural validation passed; actual game loading, generated mission progression, item drops and co-op remain untested.

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
- Hunt completion reward, global mob currency bundle sizes/chance, Camp emerald chest.
- A verified direct increase in Gold Room weighting separate from extra side-path opportunities.
- Shared gold grants and host-only installation support.

Do not present the above as features in this PAK.

## Retail test sequence

1. Launch, select a hero, enter Camp and inventory; confirm no new loading crash.
2. Break ordinary loot urns and record drop totals. Check different urn colors/levels for inherited-versus-overridden behavior.
3. Start a **new** Ancient Hunt after installing. Existing generated missions may retain earlier generation state.
4. Open normal and rare gold chests. Record count, wallet gain and who collected them.
5. Defeat the first Ancient, verify the two-Ancient wave and later raid captain, then verify exits/mission completion. Record any long delay or stuck challenge.
6. Record Hunt duration, deaths, Ancients, rooms/chests, gold from mobs, completion award and total gold for multiple runs.
7. After solo passes, test host + client with the same PAK. Check both wallets per chest/pickup and mission completion. No sharing is claimed yet; this establishes the native baseline.

Report the failing step, map/biome, whether solo/co-op, and any game crash output. Increase/decrease tuning based on measured runs rather than projected totals.

## Rebuild from private inputs

Use .NET 8 to build `tools/CookedEconomyPatcher/CookedEconomyPatcher.csproj` (UAssetAPI 1.1.0). Unpack the permitted original Better Ancient Hunt PAK with the pinned Mod Kit u4pak. Then:

```text
python scripts/build_test_pak.py --sources <collection/Metadata/PatchSources> --upstream <unpacked-mod> --balance config/balance.json --output <fresh-build-directory> --dotnet <dotnet> --patcher <CookedEconomyPatcher.dll> --packager <pinned-u4pak.py>
```

The builder verifies package serialization/re-read, validates level identities/protected routes/gates/wave references, checks PAK integrity, unpacks the result and compares all 17 entries byte-for-byte. Reports stay outside the game PAK. Raw retail inputs and generated game packages stay out of git.

## Delivered artifact

PAK SHA-256: `8abe4d1ebab1fbdcb0562468b51ea68d0b535852df7df3162fab5801f48710f3`. The ZIP includes installation/test instructions, attribution and the build/feature reports. It does not include the source collection or an original upstream mod copy.
