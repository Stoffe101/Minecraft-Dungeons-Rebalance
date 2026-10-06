# Test plan

## Test philosophy

Economy/item mutation mods can appear correct while silently corrupting state or duplicating currency.

Every destructive or economy-changing feature needs:

- precondition test
- success test
- failure test
- save/reload test
- co-op test where relevant

## Baseline regression

Before feature testing:

- launch game normally
- load hero
- enter Camp
- open inventory
- start ordinary mission
- return to Camp
- start Ancient Hunt
- finish/exit Hunt
- close/relaunch game

No feature should break unrelated vanilla flows.

---

## Camp smith placement

For each Tower smith:

- NPC appears in intended Camp location.
- No overlap with vanilla NPCs/props.
- No blocking of player navigation.
- Correct idle animation.
- Correct interaction prompt.
- Correct audio where available.
- Interaction works after mission return.
- Interaction works after full game restart.
- Multiple players do not create duplicate NPC instances.

## Common -> Rare

- Common supported gear can be upgraded.
- Correct 750 emerald price is shown.
- Insufficient funds blocks action.
- Failed/stale item blocks action without spending.
- Result remains same base gear family.
- Enchantments/state are preserved according to design.
- Favorite/QoL metadata is not assumed to exist.
- Save/reload preserves result.
- Equipped and storage edge cases tested.

## Rare -> Unique

- Rare supported gear can be converted.
- Correct 2,500 emerald price.
- Insufficient funds blocks safely.
- Native Unique variant behavior documented.
- Multi-variant families tested.
- Unique property/name/appearance valid.
- No duplicate/lost item.
- Power behavior documented.
- Save/reload valid.

## Gildsmith

- Eligible item can be gilded for 150 gold.
- Gilded enchantment is valid for item category.
- Gilded tier is valid.
- Insufficient gold blocks safely.
- Failed action does not spend.
- Existing enchantments remain valid.
- Enchantment points do not become negative.
- Save/reload preserves gilded metadata.

## Gilded reroll/improvement

- Only valid gilded items are accepted.
- 250 gold price.
- Result has exactly one valid intended built-in gilded enchant.
- No duplicate hidden gild data.
- Failure does not spend.
- Repeated use remains stable.

## Powersmith

Final cases depend on native behavior research.

At minimum:

- power increases legally
- cap rules obeyed
- price displayed
- no downgrade
- no invalid over-cap item
- enchantments preserved
- save/reload valid

---

## Ancient Hunts+

### Generation

Across at least 20 generated Hunts:

- all three sub-missions generate.
- portals/doors remain reachable.
- Gold Rooms reachable.
- Ancient rooms reachable.
- no softlocked doors.
- no missing completion exit.
- extra waves complete correctly.

### Density / stability

Stress cases:

- multiple minibosses in one encounter
- extra Ancient wave
- raid captains
- four-player co-op
- high particle/effect builds
- repeated Hunts without restart

Record:

- crash
- severe hitch
- AI stall
- mission objective stall
- enemy stuck outside arena
- excessive encounter duration

### Gold measurement

For at least 10 representative full runs record:

- completion gold
- Gold Room count
- normal Gold Chest count
- Rare Gold Chest count
- mob gold
- total gold
- run duration

Pass target:

- typical runs broadly near 150-220
- lucky/exploration runs can reach ~250-300
- unlucky runs are not routinely near vanilla ~50-ish experience

---

## Emerald Economy+

For at least 10 ordinary missions:

- emeralds from urns
- emeralds from mobs
- emeralds from other containers
- total emeralds
- duration

Repeat comparable tests with Prospector.

Pass target:

- effective normal income approximately 2x baseline
- Prospector still gives a meaningful advantage
- no absurd screen-filling pickup spam
- no wallet desync

---

## Multiplayer

Test at minimum:

### Two modded players

- modded host + modded client
- extra Hunt rooms/mobs visible to both
- chest state synchronized
- Ancient wave synchronized
- mission completion synchronized
- currency totals correct

### Host/client mismatch research

- modded host + unmodded client
- unmodded host + modded client

Do not publish these as supported configurations until proven.

### Shared gold

- host collects
- client collects
- simultaneous collection attempts
- dead player present
- joining late
- disconnect/reconnect

No player should receive unintended duplicate gold.

---

## Compatibility with Minecraft-Dungeons-QoL

With both mods installed:

- inventory opens
- favorites/selection UI still renders
- multi-salvage still works
- smith item selection works
- smith mutations do not confuse physical-item identity
- no duplicate patched function collisions
- Ancient Hunts unaffected by inventory UI patches

Neither mod should require the other.


## 2026-10-05 tooling results

Five local integration tests passed: valid/invalid exact manifests; real UE4.22 fixture archive collection with synthetic JSON, tagged defaults and exact SHA/byte preservation; missing-target failure with retained partial evidence; refusal to overwrite output; refusal to write within the game directory. Traversal, duplicate/case ambiguity and unsupported extensions are included in negative cases. PowerShell syntax parsing passed for both scripts. Ordinary local MSBuild was blocked by a container dotnet CLI process-information failure; direct Roslyn compilation succeeded. Windows CI added, pending separately verified results.

These are collector tests. All gameplay, economy-distribution, item preservation, save/reload and multiplayer tests remain pending. No Rebalance PAK currently exists.


## HuntsEconomy-Test-v1 actual results

All 97 collected-source hashes matched. Three cooked reward packages patched and re-opened with full semantic equality to expected mutation; imports, field schema and every Blueprint script separately matched originals. Eight Hunt tests passed: eleven real levels with bounded adaptation and input preservation; preserved original reward fields; rejected identity mismatch, unknown added wave groups, excessive wave count, native route mutation and gate mutation; JSON comment markers inside strings preserved. Builder packaged 17 entries, u4pak integrity passed, and every extracted byte/path equaled the staging input. Retail loading, reward field units, urn subclass coverage, arena timing/completion, final income and multiplayer behavior remain pending; see BUILD_AND_TEST.md.

## Test v2 continuation

Three economy integration tests pass against the supplied private packages. They verify four-package semantic re-read, output preservation, invalid Camp reward rejection and previously patched Camp-default rejection before output creation. Final v2 PAK has 19 entries; integrity and exact unpack comparison passed. Hunt data is unchanged from v1; its eight existing tests remain applicable. Test Camp wallet delta when chest is available; do not interpret native daily/unlock availability as a failed amount patch. Guaranteed Ancient acceptance tests are in ANCIENT_GUARANTEE.md and remain pending.

## Supplemental / upgrade-choice continuation

Seventeen synthetic-adapter upgrade policy tests and two Lua 5.4 API-shim tests passed. These are not native upgrade/charge, UMG, loader, Hunt guarantee or retail tests. Actual game tests remain pending; v2 PAK unchanged.

## October 6 Drive and withdrawn diagnostic checks

Eight original archives mounted; six targeted reads completed without export errors; ten preserved hashes verified, four gold-chest files exactly match previous sources. Three local Lua shim tests pass, including withdrawn-default no-key/no-query/no-queue behavior. No game runtime, native loader compatibility, new chance/paid upgrade/shared-gold feature or retail PAK execution is tested. Complete archive contents remain unaudited.

## Retail result — October 6, 2026

Camp emerald chest reward check passed by user observation: 100 emeralds instead of 50. Accept this as feature-specific confirmation in the tested setup. Test count and multiplayer role are unspecified. Other reward changes, Hunt progression, aggregate income and multiplayer behavior remain pending; synthetic checks do not substitute for these observations. No code or artifact changed, so tests were not rerun for this documentation-only update.

## Additional user results — October 6, 2026

Emerald urn: wallet 241 -> 266, observed +25 emeralds. Gold chests: user requests marking the feature complete; recorded as gameplay acceptance passed by user confirmation, with exact per-chest measurements unspecified. These observations do not establish aggregate income, all urn subclasses, Prospector behavior or multiplayer distribution. Camp NPCs and paid upgrade mechanics, including the Unique variant picker, are the user’s highest priority.

## Camp smith asset implementation — October 6, 2026

Implemented CampSmithStager: six native actor/widget clones in an isolated Rebalance namespace; original sources and Tower flags preserved; all outputs re-opened and semantically verified. Four private-source integration checks passed. Native Powersmith uses TowerBlacksmith enum and equipped-gear view; Artisan/Gilder use owning-player inventory selection. Upgrade buttons bind TransactionClassPrio to native transaction classes, not a demonstrated price/selected-result API. Tower NPC objectgroup contains complete floor tiles rather than portable props. See CAMP_SMITH_IMPLEMENTATION.md and research/camp-smith-stage.json. No live Camp placement, payment, persistent mutation or Unique picker was enabled; latest gameplay v2 unchanged. Local SDK/MSBuild process-information failures were bypassed with SDK Roslyn compilation; Windows CI now builds the normal project. Next: compatible native transaction route, paid selected-result mutation and native presentation binding, then Camp spawn/dispatch and real saved-item tests.

## Camp smith CI validation — October 6, 2026

At implementation commit e5ed4a8ee150f9ce3cfed500d34b5bd8130182d9, Windows workflow 37401665954 passed normal project compilation of CampSmithStager, the existing collector/fixture checks, economy compilation and Python syntax. Policy workflow 37401665905 passed. Four Camp smith integration tests were run locally against the private originals; CI does not have those retail inputs. These results validate tooling and asset staging, not gameplay or paid native transactions.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## First native reader Windows run — October 6

At source commit 4c90eb3929e288bd33908701fe053df6429b2184, run 37467815494 compiled with zero warnings/errors and passed all 19 checks, including the own-process query/read marker. No-game reporting and existing-output preservation behaved as expected. The job then failed because the intentional negative-test exit code remained in PowerShell LASTEXITCODE and the Actions epilogue treated it as a failure. Reset LASTEXITCODE only after asserting the expected failure and output conditions; apply the same fix to the packaged-runner negative test. Bundle preparation did not run in this first attempt. Local Linux incomplete-report and output-preservation checks also passed. This is a CI exit-handling fix, not a retail compatibility result.


## Native collector Windows validation — October 6, 2026

At source commit 2630d8732cbcc25828b18e87e7c16a5946525032, Native declaration reader run 37468028504 passed: normal Windows .NET build (zero warnings/errors), all 19 synthetic/self-process checks, PowerShell parsing, no-game incomplete-report behavior, existing-report preservation, compiled ZIP preparation and execution of the unpacked capture runner from paths containing spaces. The runner's failure ZIP contained NativeContracts.json, REPORT.json, Capture.log and SelfTest.log. Upgrade policy run 37468028478 and existing evidence tooling run 37468028467 also passed. None has a Dungeons runtime.

The downloadable compiled research bundle is Minecraft-Dungeons-Rebalance-NativeCollector.zip, 109,031 bytes, SHA-256 f6e3803ee5e9f8564274dbc7bb18b1b790ad795fed6681b2577f1a3fdc11e642. DLL SHA-256 bc5ab21c4b76a580e0a0b50ccc809beeca970c3edf6c6171a36b02c8ad14b74e. Downloaded Actions wrapper digest matched GitHub's advertised checksum; the nested bundle includes the runner, configuration and compiled tools, with no game-owned assets/PAK. The capture bundle is research tooling, not a new mod release. Dungeons compatibility and native transaction semantics remain unverified.

Next required input: one new declaration capture with Dungeons running at Camp, returned even if incomplete. Instructions are in tools/NativeContractReader/README.md. Do not repeat the existing asset collection or re-enable the withdrawn UE4SS loader. Paid Camp services and Unique choice remain unfinished; delivered gameplay v2 and its hash are unchanged.
