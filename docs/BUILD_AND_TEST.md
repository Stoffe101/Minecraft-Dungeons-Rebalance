# Hunts / Economy test build v2

**Current (October 7): v5 crashed with `Unsupported UBoolProperty ReturnValue size 0` and is withdrawn. v6 fixes that serialized Boolean and implements interactable native smith buttons, guarded native upgrade transactions and temporary 1 emerald / 1 gold fees. It retains Gift Wrapper-relative placement, names and the accepted v2 reward/Hunt subset. Fifteen private integration tests and all 37 PAK entries pass structural checks. Gameplay, native Camp eligibility and save/reload are unverified; full prices, the custom Unique picker and shared gold remain unfinished. v4 NPC loading/spawning was user-confirmed. See [CAMP_UPGRADE_TEST_V6.md](CAMP_UPGRADE_TEST_V6.md) for implementation, tests, limitations and next work.**

## Historical milestones through October 6

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


## October 7: confirmed v4 NPC load, central placement, names and UI preview

The user confirms all three v4 smiths are visible in Camp. The screenshots show overlapping chairs below the bridge beside Ancient Hunt, and request the clearing beside the Gift Wrapper instead. This accepts v4 NPC loading/spawning only; it does not accept interaction, prices, item mutation, saves or multiplayer. The current test-price request is **1 emerald per upgrade and 1 gold for gilding**. Production prices remain separate.

Implemented Gift Wrapper-relative spawning using the exact collected native class path `/Game/Decor/Prefabs/Merchants/BP_LobbyVillager_GiftWrapper`. The prototype offsets are right 900 and forward -650/0/+650 in that actor's basis; these are authored test positions, not measured coordinates from the screenshots. All three seats have 650-unit spacing rather than 220. If no valid Gift Wrapper is present, no smiths spawn; there is no fallback to the old bridge position. Reload Camp after swapping builds. Host-only, nonreplicated behavior and per-class duplicate checks remain.

Added independent engine TextRenderActor labels: Uniquesmith, Powersmith and Gildsmith. Labels sit 70 units above each smith's actor bounds and face the available player camera when created, with 40-unit text size. This uses reflected `TextRenderActor.TextRender`, not its non-UFUNCTION C++ getter. Names are separate world text, not a complete native balloon/TTS implementation; camera orientation is sampled once. Font visibility, exact location, overlap and navigation need retail confirmation.

Added `--interaction-preview`: native interaction is allowed to open the owned merchant screen. In the three owned content copies, emptied and disabled **all 11 TransactionClassPrio-bearing widget/template configurations**:2 Unique,7 Power (including collect/bullet/slot views),2 Gild. Original Tower assets are untouched. No action in the owned graph calls TryExecute, Deduct, ClientAdd, GildItem or TryUpgradeItem. The intent is a read-only dialogue/selection test; native runtime behavior is not certified by serialization. If an upgrade action unexpectedly becomes enabled, do not treat that as the paid feature passing.

Added `RebalanceCanAffordTestUpgrade` to each owned root screen. Its native path validates the supplied player, requires authority, obtains WalletComponent using Actor.GetComponentByClass, validates the wallet, and checks captured WalletComponent.Balance against 1. It accepts the actual native SerializableItemId from its future caller, avoiding a guessed currency ID or player wallet field. Invalid inputs return false. This is a native preflight foundation **not wired to buttons, not a price override, and not a charge**. Emerald/Gold selection, an exclusive action gateway, chosen-Unique confirmation, success-only charging and normal-inventory persistence remain unfinished. Captured GetPrice is not a Blueprint event; post-success Deduct alone cannot enforce affordability before mutation. No further collector is requested.

Final local validation: 13 private-source integration tests pass; all 10 package pairs reopen with exact parsed equality and unchanged source hashes. Placement negative cases now 10, affordability 4, read-only bindings 2; prior presentation/record/selection/screen/loading checks remain. The actual crashed v3 still fails the loading-contract regression check. Compiler is the pinned direct Roslyn route with the known Newtonsoft net6/net8 warning. A first UI test found Powersmith has 7 transaction configurations rather than 2; corrected the source-specific count and reran all checks. These tests do not launch Dungeons.

Built `MinecraftDungeonsRebalance-CampUI-Test-v5.pak`:37 entries, 6133071 bytes, SHA-256 `eb7aa6324eb4bf9f47942234ab036e8ed28449a47a1d8a98c11a7137be9c080c`. Integrity test and exact path/byte unpack comparison pass. All17 non-chest v2 entries stay byte-identical; the replacement chest keeps100 emeralds. v5 is untested in-game. Reports: research/camp-central-ui-stage-v5.json and research/camp-central-ui-build-v5.json. Windows CI follows the source commit.

Next work: connect a single owned action button to the preflight and native transaction decision/success path; recheck funds at confirmation; charge once after verified success; establish persistent Camp inventory behavior and reloading. Do not re-enable the original free Tower action as a substitute. Shared gold, full economy/Hunt design and custom Unique picker are still incomplete.


## v5 Windows validation — October 7

Implementation commit ff7f772d8179a141fae0879efbb76209515f7f8b passed all three workflows: evidence tooling 37591860858 (collector job 112695000966 includes successful normal Windows CampSmithStager compilation), native reader 37591860830, and policy models 37591860834. Evidence is completed job/step metadata; no compiler warning count is claimed. CI has neither private retail source inputs nor a Dungeons runtime. The 13 private integration tests, all 10 package round trips, and 37-entry PAK integrity/unpack comparison were performed locally. No retail confirmation of v5 positions, TextRender labels or merchant UI has been received.

Placement requires an existing Gift Wrapper actor when the chest begin-play hook runs. There is no timer/retry if the Gift Wrapper is spawned later. Camp lifecycle ordering and the Gift Wrapper actor basis remain runtime acceptance points; the screenshot does not establish those. Charging, currency selection, persistent upgrades and custom Unique choice remain unfinished.
