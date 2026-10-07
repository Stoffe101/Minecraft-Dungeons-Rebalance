# Test plan

**Current (October 7): v5 crashed with `Unsupported UBoolProperty ReturnValue size 0` and is withdrawn. v6 fixes that serialized Boolean and implements interactable native smith buttons, guarded native upgrade transactions and temporary 1 emerald / 1 gold fees. It retains Gift Wrapper-relative placement, names and the accepted v2 reward/Hunt subset. Fifteen private integration tests and all 37 PAK entries pass structural checks. Gameplay, native Camp eligibility and save/reload are unverified; full prices, the custom Unique picker and shared gold remain unfinished. v4 NPC loading/spawning was user-confirmed. See [CAMP_UPGRADE_TEST_V6.md](CAMP_UPGRADE_TEST_V6.md) for implementation, tests, limitations and next work.**

Historical milestone notes below are superseded by the current v6 status.

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


## First retail native capture and discovery correction — October 6

Received native-contracts-20261006-131039.zip. All 19 original self-tests passed. The game query/read handle and PE/data reads succeeded: 6,345,728 bytes, nine reads. Collection stopped at “Too many name-table candidates” before validating any name table; zero classes/control contracts were collected and no game functions/writes occurred. This is a collector discovery-filter failure, not evidence of denied process access. Sanitized result: research/native-capture-20261006.json.

Corrected discovery to use cached, sorted VirtualQueryEx region queries and filter committed readable non-executable data before the existing 200,000 candidate bound. Complete candidate headers must fit their region. Retained byte/read/time bounds; added a 4,096-query bound and 1,000,000 raw-entry workspace bound. No new process rights or protection fallback. Reports include counts and at most eight distinct rejected object-layout reasons, without process addresses or game values. Twenty-one local checks now pass, including >210,000 numeric-noise candidates, memory protection states and boundary rejection; Windows adds its own-process read/query check. Native upgrade work remains blocked on an actual declaration capture. A new corrected capture is needed; no repeat cooked asset collection is requested.


## Corrected collector Windows result — October 6

At d55fe531e12a84a39d87407d4942b89656ba17a5, Windows native-reader workflow 37469426624 passed normal compilation, all 22 checks (including own-process VirtualQueryEx/ReadProcessMemory), PowerShell parsing, report preservation and execution/archive checks for the unpacked bundle. This validates the filter implementation; the corrected collector has not yet run on Dungeons.

NativeCollector-v2.zip: 113,119 bytes; SHA-256 8068ef6a006f39f0dd389bcc531eda4e3a86bce43fce988588e0dc3019c592f0. DLL SHA-256 16b03d99f63ddc0cbe22f15a9c88faad7cbdd4e40cde8e7d7efd497c3d80d36c. Verified the downloaded Actions wrapper against its advertised digest before extracting the new bundle. No gameplay PAK or paid upgrade is enabled. Next: run the corrected collector from its separate extracted folder with Dungeons at Camp and inspect the new report.


## Second retail capture and source-grounded layout correction — October 6

Received native-contracts-20261006-132200.zip. v2 filtering succeeded: 211,959 raw candidates reduced to 153,213 mapped-data candidates; 7,571,532 bytes, 153,233 reads, 1,054 region queries. No name table validated and no class declarations were collected. Sanitized report: research/native-capture-20261006-v2.json.

Read the related QoL native-favorites-20261006-132111-71d697.zip privately: revision legacy-names-256-v2 progressed past name validation but reported zero object-array shape candidates. It contains no accepted declarations. Reviewed latest same-owner QoL e535cab78a11999329ff3ae5d5c5608dd7986f07, including reserved-object capacity correction, as a reuse reference. Reviewed Epic-authored UE4.22.3 NameTypes.h and UObjectArray.h at 99a530d4ccbe6bea1e8f49df20acfeb294006962 through the GitHub connector. The engine name-array maximum/chunk size implies 256 pointer slots. Object preallocation rounds its reservation to an extra chunk; 2 Mi elements can reserve 33 chunks (2,162,688 slots), which the original max-capacity guard rejected. Reserved capacity is independent of live-object work.

Implemented inline and pointer-backed name tables, 128/256 capacities, reserved name chunks, bounded ANSI/UTF-16 text, and independently verified child/target layouts. Native function header selection remains controlled by four getters, and all six profile contracts must match. Object capacity now permits up to 4 Mi reserved slots/64 chunks while actual traversal remains bounded by the unchanged 1M live-object limit. The old 500K call ceiling was insufficient for two header candidates plus that live walk; now 2M calls, with the original 128 MiB/60-second and 4,096-query limits retained. No process rights, game calls, writes or protection fallback added.

Forty-nine local synthetic checks pass, including 24 inline/pointer/name/child/target combinations, distinct valid table ambiguity, 256-capacity tables with 33 reserved object chunks, and inconsistent-capacity rejection. The first compile caught a local variable/constructor-parameter name collision; renamed the parameter before testing. These fixtures validate declaration consistency and rejection behavior, not retail upgrade ABI or payment semantics. Windows validation and a new compiled capture bundle follow. Native Camp upgrade integration remains unfinished; v2 gameplay PAK unchanged.


## Native collector v3 Windows validation — October 6

At 6ab37c672d1cde522ce1997fcbddaa0e5aab10b0, native-reader Windows run 37471659764 passed normal compilation (zero warnings/errors), all 50 checks, PowerShell parsing, fresh/incomplete/report-preservation behavior and execution of the unpacked ZIP runner. Upgrade policy run 37471659806 and existing tooling run 37471659602 also passed. Retail v3 declaration discovery remains untested.

NativeCollector-v3.zip: 118,244 bytes; SHA-256 9b21fc20fd416a76939662a9da381b99be6afba9f589be549b5b0b76fe3fb252. DLL SHA-256 a78ea33df90e54cc41a6daa935a0d39bb2b951312d7c5fb5a6fb4b7a476e6783. The downloaded Actions wrapper matched its advertised digest before extraction. Compiled bundle saved for private testing. Next: capture once with this version at Camp, then use accepted declarations (or precise failure stage) to continue native transaction work. No native paid upgrade or new gameplay PAK is claimed.


## Native Unique presentation increment (October 6)

Five Camp private-source tests pass, including seven rejected graph mutations: missing out-parameter function flag, wrong owner, missing argument, wrong input field, wrong native declaring owner, missing preload dependency, and duplicate patch. Six package pairs are written/reopened and source hashes retained; three presentation functions are added only to Uniquesmith content. These are asset checks, not in-game widget calls. Future retail acceptance must cover localized names/icons/descriptions, choice list completeness, selection cancellation, native transaction item identity, exact balance changes, failure/refusal, save/reload preservation and online host/join roles. No upgrade gameplay test has passed.


## Selected-item native integration — October 6

Added native physical-slot/item and full-record read functions to all three cloned smith screens, validated against captured InventoryItemSlot.Item and InventoryItem.Item declarations. Six private asset tests pass (11 deliberate graph rejection cases); no currency/item/save mutation is enabled. The newer QoL capture succeeded and confirms item-record field names/nested array types, but lacks transaction parent classes and field sizes/offsets.

Prepared collector v4 with 11 additional exact parent/struct targets and bounded nested array declarations; 54 local checks pass. Full paid Camp services still require that added capture and native behavior validation. No repeat v3 or asset collection is required. Detailed findings, source pins and next work: [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md). Gameplay PAK remains v2.

## October 6 v4 capture accepted

All 29 requested native types and 16 enum declarations captured successfully. Camp staging now rejects changed native record shapes and sparse enum values; six real-source integration tests include 15 rejection checks. Native pricing/selection dependencies revealed by this capture are being resolved through bounded parent/struct closure. No paid upgrade, picker, Camp placement or shared-gold runtime is enabled. See CAMP_SMITH_IMPLEMENTATION.md and NATIVE_DECLARATION_RESEARCH.md for exact findings, tests and next work. Retail acceptance still requires actual payment, selected outcome, item-state preservation, repeated upgrades and save/reload tests.

## Transitive collector Windows validation and v5 bundle

At implementation commit de006824189a335033958f084ac1d98737ece46f, native reader workflow 37481190026 passed (job 112329420111): normal Windows build with zero warnings/errors, 63 checks including self-process reads, PowerShell parsing, no-game incomplete reporting, existing-report preservation, bundle preparation and packaged runner execution. Evidence workflow 37481190032 also passed (job 112329420366), including normal CampSmithStager build with zero warnings/errors. Policy workflow 37481190004 passed. These are tooling/source results; retail merchant transactions are not tested.

Artifact 11421416403 outer digest d79a12cc53980ce0170cf88e4adba8061bf9be29d9aa67d71dc8ebc51942c611 matched before extraction. NativeCollector-v5.zip: 127,660 bytes, nine entries, SHA-256 4e76ac087cff0ce2fee3175e64bc8ee5a93425aa9f2c21eea1be3127236a6ef5; DLL SHA-256 8bab5ecd7b6f1df493fdccce2badf786920d78e9ef8b8cb4c9817d6c1b92fb02. Both ZIP integrity checks pass. Bundle made available for the newly exposed merchant dependency capture; no game assets, loader or upgrades included.

Required next input: run the capture command from a fresh extracted v5 folder with Dungeons at Camp and return its output ZIP. The accepted v4 result is retained; this targets merchant/selection roots and their transitive native parent/struct dependencies. This is the concrete declaration blocker before native paid service integration. No new gameplay PAK or full-project completion is claimed.

## V5 integration and collector stop

The user supplied the successful v5 capture: 53 types/26 enums, requested dependency closure complete. No further capture requested. Camp actor CDOs now point to owned merchant roots whose GetSoftContentWidget overrides open the matching cloned content; all other native decision/input graphs are retained. Seven asset integration tests pass, including 18 negative graph/record/dispatch checks. This is configuration/graph integration, not runtime payment or save validation. No new gameplay PAK is released. Pricing customization and normal-inventory upgrade behavior need an exposed implementation route or a validated native runtime/toolchain before paid services and world placement can be completed. See CAMP_SMITH_IMPLEMENTATION.md.

## Native screen integration CI verification

Implementation commit 0285e2b9c8199ad4bab0fc96bd74c86c4a7e98c3 is published to main. Evidence workflow 37485292520, native reader workflow 37485292521 and policy workflow 37485292512 all succeeded. Windows evidence builds CampSmithStager normally; private retail source integration tests are the seven locally passing tests, not part of public CI. No new collector or playable PAK is delivered. The uploaded v5 capture is accepted and no further command/capture request is pending.

Additional public searches for existing Camp Tower-merchant mods and MerchantPricingComponent source did not produce an inspectable primary implementation in the returned results. Results included unrelated projects and general game pages; this search does not prove absence of an existing mod or pricing API. No unverified source was reused or recommended.

## Camp placement preview Windows validation — October 6, 2026

At implementation commit 7972a9127c4ebaf1c90ad51452618afe9b4c9fc4, evidence run 37521291248 passed normal Windows CampSmithStager compilation, existing permitted-fixture integration checks and Python syntax validation of the preview builder/tests. Native declaration reader run 37521290998 and upgrade policy run 37521290989 also passed. Local private-source checks passed nine tests, including eight placement corruption rejections; an additional packager check preserved an existing output directory. Windows CI does not have private retail assets or a Dungeons runtime. The final 37-entry PAK hash is 6f13bc4f04f2f5d4a4782803158e41943c29d647b98968cae2066a9495fd409a (6,075,200 bytes). Test artifact is saved and source/documentation published; retail NPC placement remains pending.

## v4 Windows validation — October 6

Implementation commit f25d8fbc351e293eeebaa2d512d1ac77debef4a0 passed evidence run37524820391 (normal Windows CampSmithStager build, zero warnings/errors, permitted-fixture checks and Python syntax), native reader run37524820045 and policy run37524820052. Eleven private-source tests and the actual-crashed-v3 rejection ran locally; CI does not have those private inputs or a game runtime. v4 remains a runtime-unverified loading correction candidate, with prices/upgrades disabled. Final PAK hash1fc846ee8c0633cd10322a14ac4dedf23f67971ddfc2147a60cd6a1edbd3b527;6,075,360bytes. Saved the private candidate PAK and published source/docs. Targeted full collected asset-name inventory also yielded no merchant/pricing JSON/INI candidate or exposed price-definition asset; this is a filename inventory result, not proof that all native pricing configuration is absent.


## October 7 v5 UI test acceptance

Replace v4 with CampUI-Test-v5; keep only one Rebalance PAK installed. Enter Camp in single-player with Gift Wrapper unlocked. Check all three seats are nearby, separated and reachable, and that their names are visible. Open each NPC's dialogue, inspect native inventory selection, then cancel/close/reopen and reload Camp. Check for duplicate seats/labels. The native upgrade action should be unavailable in this read-only build. This is not a1-emerald/1-gold spending test: no charging or upgrade is implemented. Later acceptance must test 0 funds, exact 1 funds, cancellation, each successful action, Unique choice, repeat attempts, and item/balance persistence after reload. Preserve v2 reward observations separately from these new tests.
